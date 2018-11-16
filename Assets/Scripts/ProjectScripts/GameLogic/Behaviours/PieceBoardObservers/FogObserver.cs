using System.Collections.Generic;
using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver, IResourceCarrierView
{
    private int level;
    private FogDef def;
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
    
    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        Key = new BoardPosition(position.X, position.Y);
        
        def = GameDataService.Current.FogsManager.GetDef(Key);
        
        if(def == null) return;
        
        Mask = def.Positions;
        viewDef = Context.ViewDefinition;

        if (viewDef != null)
        {
            viewDef.OnAddToBoard(position, context);
            level = def.Level;
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
        if(def == null) return;
        
        Debug.Log($"[FogObserver] => Clear fog with uid: {def.Uid}");
        
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.ClearFog, def);
        
        AddResourceView.Show(def.GetCenter(Context.Context), def.Reward);
        GameDataService.Current.FogsManager.RemoveFog(Key);
        
        if(def.Pieces != null)
        {
            foreach (var piece in def.Pieces)
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
        
        var weights = def.PieceWeights == null || def.PieceWeights.Count == 0
            ? GameDataService.Current.FogsManager.DefaultPieceWeights
            : def.PieceWeights;
        
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
    }
    
    public void RegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UnRegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UpdateResource(int offset)
    {
        var canPath = CanBeReached();
        var levelAccess = storageItem.Amount >= level;
        
        if ((canPath ^ levelAccess) && lockView == null)
        {
            lockView = viewDef.AddView(ViewType.Lock) as LockView;
            lockView.Value = level.ToString();
            lockView.transform.position = def.GetCenter(Context.Context);
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
        }
        
        bubble = viewDef.AddView(ViewType.Bubble) as BubbleView;
        
        if(bubble.IsShow) return;
        
        bubble.SetData("Clear fog", def.Condition.ToStringIcon(), OnClick);
        bubble.SetOfset(def.GetCenter(Context.Context) + new Vector3(0, 0.1f));
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
        var pathExists = Context.Context.Pathfinder.CanPathToCastle(Context);
        return pathExists;
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
        if(CurrencyHellper.IsCanPurchase(def.Condition, true, () => OnClick(piece)) == false) return;
        
        CurrencyHellper.Purchase(Currency.Fog.Name, 1, def.Condition, success =>
        {
            if (success == false) return;
			
            Context.Context.HintCooldown.RemoweView(bubble);

            bubble.OnHide = () =>
            {
                piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                {
                    To = piece.CachedPosition,
                    Positions = new List<BoardPosition> {piece.CachedPosition}
                });
            };
            
            bubble.Priority = 1;
            bubble.Change(false);
        });
    }
}