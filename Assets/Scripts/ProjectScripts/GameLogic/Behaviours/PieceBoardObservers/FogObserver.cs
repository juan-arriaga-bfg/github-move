using System.Collections.Generic;
using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver, IResourceCarrierView
{
    private int level;
    public FogDef Def { get; private set; }
    private StorageItem storageItem;
    private ViewDefinitionComponent viewDef;
    private LockView lockView;
    private BubbleView bubble;
    public BoardPosition Key { get; private set; }
    
    public RectTransform GetAnchorRect()
    {
        throw new System.NotImplementedException();
    }
    
    public Vector3 GetAcnhorPosition()
    {
        throw new System.NotImplementedException();
    }

    public Camera RenderCamera => Context.Context.BoardDef.ViewCamera;

    public List<IResourceCarrier> Carriers { get; private set; }

    private bool? canBeReachedCached;
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        Key = new BoardPosition(position.X, position.Y);
        
        Def = GameDataService.Current.FogsManager.GetDef(Key);
        
        if(Def == null) return;
        
        Mask = Def.Positions;
        viewDef = Context.ViewDefinition;

        if (viewDef != null)
        {
            viewDef.OnAddToBoard(position, context);
            level = Def.Level;
            storageItem = ProfileService.Current.GetStorageItem(GetResourceId());
            ResourcesViewManager.Instance.RegisterView(this);
        }
        
        base.OnAddToBoard(position, context);
        
        GameDataService.Current.FogsManager.RegisterFogObserver(this);
    }

    public override void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        base.OnRemoveFromBoard(position, context);
        
        ResourcesViewManager.Instance.UnRegisterView(this);
        
        var views = ResourcesViewManager.Instance.GetViewsById(Currency.Level.Name);

        if (views == null) return;
        
        foreach (var carrierView in views)
        {
            carrierView.UpdateResource(0);
        }
        
        GameDataService.Current.FogsManager.UnregisterFogObserver(this);
    }

    public override BoardPosition GetPointInMask(BoardPosition position, BoardPosition mask)
    {
        return new BoardPosition(mask.X, mask.Y, position.Z);
    }

    public void Clear()
    {
        if(Def == null) return;
        
        Debug.Log($"[FogObserver] => Clear fog with uid: {Def.Uid}");
                
        AddResourceView.Show(Def.GetCenter(Context.Context), Def.Reward);
        GameDataService.Current.FogsManager.RemoveFog(Key);
        
        if(Def.Pieces != null)
        {
            foreach (var piece in Def.Pieces)
            {
                foreach (var pos in piece.Value)
                {
                    var pieceId = GameDataService.Current.MinesManager.GetMineTypeById(piece.Key);

                    if (pieceId == PieceType.None.Id)
                    {
                        pieceId = PieceType.Parse(piece.Key);
                    }
                    
                    Context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
                    {
                        At = pos,
                        PieceTypeId = pieceId
                    });
                }
            }
        }
        
        var weights = Def.PieceWeights == null || Def.PieceWeights.Count == 0
            ? GameDataService.Current.FogsManager.DefaultPieceWeights
            : Def.PieceWeights;
        
        foreach (var point in Mask)
        {
            var piece = ItemWeight.GetRandomItem(weights).Piece;
            
            if(piece == PieceType.Empty.Id) continue;
            
            Context.Context.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = point,
                PieceTypeId = piece
            });
        }
        
        Context.Context.ActionExecutor.AddAction(new CallbackAction
        {
            Callback = controller => controller.TutorialLogic.Update()
        });
    }
    
    public void RegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UnRegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UpdateResource(int offset)
    {
        canBeReachedCached = null;
        
        var canPath = CanBeReached();
        var levelAccess = storageItem.Amount >= level;
        
        if ((canPath ^ levelAccess) && lockView == null)
        {
            lockView = viewDef.AddView(ViewType.Lock) as LockView;
            lockView.Value = level.ToString();
            lockView.transform.position = Def.GetCenter(Context.Context);
        }

        lockView?.SetGrayscale(!canPath);
        
        var fog = Context.ActorView as FogPieceView;
        
        if(fog != null) fog.UpdateBorder();
        
        if(canPath == false || storageItem.Amount < level) return;

        if (lockView != null)
        {
            lockView.Change(false);
            viewDef.RemoveView(ViewType.Lock);
            lockView = null;

            var pos = Def.GetCenter();

            pos.Z = 4;
            
            ParticleView.Show(R.FogExplosionParticleSystem, pos);
        }
        
        bubble = viewDef.AddView(ViewType.Bubble) as BubbleView;
        
        if(bubble.IsShow) return;

        bubble.SetData(LocalizationService.Get("gameboard.bubble.message.fog", "gameboard.bubble.message.fog"), Def.Condition.ToStringIcon(), OnClick);
        bubble.SetOfset(Def.GetCenter(Context.Context) + new Vector3(0, 0.1f));
        bubble.Priority = -1;
        bubble.Change(true);
        
        Context.Context.HintCooldown.AddView(bubble);
    }
    
    public string GetResourceId()
    {
        return Currency.Level.Name;
    }
    
    public Vector2 GetScreenPosition()
    {
        return Context.Context.BoardDef.GetSectorCenterWorldPosition(
            Context.CachedPosition.X,
            Context.CachedPosition.Y,
            Context.CachedPosition.Z);
    }

    public bool CanBeReached()
    {
        if (canBeReachedCached.HasValue)
        {
            return canBeReachedCached.Value;
        }
        
        canBeReachedCached = Context.Context.Pathfinder.CanPathToCastle(Context);
        return canBeReachedCached.Value;
    }

    public bool CanBeCleared()
    {
        var pathExists = CanBeReached();
        var resourcesEnought = storageItem.Amount >= level;

        return pathExists && resourcesEnought;
    }

    public bool IsActive
    {
        get
        {
            var canPath = CanBeReached();
            return canPath || storageItem.Amount >= level;
        }
    }
    
    private void OnClick(Piece piece)
    {
        if(CurrencyHellper.IsCanPurchase(Def.Condition, true, () => OnClick(piece)) == false) return;
        
        CurrencyHellper.Purchase(Currency.Fog.Name, 1, Def.Condition, success =>
        {
            if (success == false) return;
			  
            BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.ClearFog, Def);
            
            Context.Context.HintCooldown.RemoweView(bubble);

            bubble.OnHide = () =>
            {
                piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                {
                    To = piece.CachedPosition,
                    Positions = new List<BoardPosition> {piece.CachedPosition},
                    OnComplete = () =>
                    {
                        DevTools.UpdateFogSectorsDebug();
                    }
                });
            };
            
            bubble.Priority = 1;
            bubble.Change(false);
        });
    }
}