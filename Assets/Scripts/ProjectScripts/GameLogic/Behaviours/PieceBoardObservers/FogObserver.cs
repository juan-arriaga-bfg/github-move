using System.Collections.Generic;
using DG.Tweening;
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

    public bool IsRemoved = false;
    
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

        PrepareFogToClear();
    }

    public virtual void PrepareFogToClear()
    {
        if (!CanBeCleared()) return;
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
        var levelAccess = RequiredLevelReached();
        
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

        PrepareFogToClear();
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
        // todo: fix and enable cache. Now it initializing BEFORE all fogs are created and contains incorrect value
        // if (canBeReachedCached.HasValue)
        // {
        //     return canBeReachedCached.Value;
        // }
        
        canBeReachedCached = Context.Context.PathfindLocker.HasPath(Context);
        return canBeReachedCached.Value;
    }

    public bool CanBeCleared()
    {
        var pathExists = CanBeReached();
        var resourcesEnough = RequiredLevelReached();

        return pathExists && resourcesEnough;
    }
    
    public bool RequiredLevelReached()
    {
        return storageItem.Amount >= level;
    }

    public bool IsActive
    {
        get
        {
            var canPath = CanBeReached();
            return canPath || RequiredLevelReached();
        }
    }
    
    private void OnClick(Piece piece)
    {
        if(CurrencyHelper.IsCanPurchase(Def.Condition, true, () => OnClick(piece)) == false) return;

        bubble.CleanOnClick();
        Context.Context.HintCooldown.RemoweView(bubble);

        bubble.OnHide = () =>
        {
            CurrencyHelper.Purchase(Currency.Fog.Name, 1, Def.Condition, success =>
            {
                if (success == false) return;
                
                piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClearFog, Def);
                AddResourceView.Show(Def.GetCenter(piece.Context), Def.Reward);
                
                IsRemoved = true;
                piece.Context.ActionExecutor.AddAction(new CollapseFogToAction
                {
                    To = piece.CachedPosition,
                    Positions = new List<BoardPosition> {piece.CachedPosition},
                    FogObserver = this,
                    OnComplete = DevTools.UpdateFogSectorsDebug
                });
            });
        };
            
        bubble.Priority = 1;
        bubble.Change(false);
    }
}