using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver, IResourceCarrierView
{
    private int level;
    public FogDef Def { get; private set; }
    public CurrencyPair AlreadyPaid { get; private set; }
    
    private StorageItem storageItem;
    private ViewDefinitionComponent viewDef;
    private LockView lockView;
    private FogProgressView bar;
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

        if (Def == null) return;
        
        var save = ProfileService.Current.GetComponent<FogSaveComponent>(FogSaveComponent.ComponentGuid)?.GetRewardsSave(Key);

        AlreadyPaid = new CurrencyPair {Currency = Def.Condition.Currency, Amount = save?.AlreadyPaid ?? 0};
        
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
    
    public void RegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UnRegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UpdateResource(int offset)
    {
        if (bubble != null && bubble.IsShow) return;

        if (AlreadyPaid.Amount == Def.Condition.Amount)
        {
            OpenBubble();
            return;
        }
        
        canBeReachedCached = null;
        
        var canPath = CanBeReached();
        var levelAccess = RequiredLevelReached();
        
        if(lockView == null && (Def.IsActive && canPath ^ levelAccess) || Def.IsActive == false && canPath)
        {
            lockView = viewDef.AddView(ViewType.Lock) as LockView;
            lockView.Value = Def.IsActive ? level.ToString() : "?";
            lockView.transform.position = Def.GetCenter(Context.Context);
        }

        lockView?.SetGrayscale(canPath == false || Def.IsActive == false);
        
        var fog = Context.ActorView as FogPieceView;

        if (fog != null) fog.UpdateBorder();
        if (CanBeCleared() == false) return;

        if (lockView != null)
        {
            lockView.Change(false);
            viewDef.RemoveView(ViewType.Lock);
            lockView = null;

            var pos = Def.GetCenter();

            pos.Z = 4;
            
            ParticleView.Show(R.FogExplosionParticleSystem, pos);
        }
        
        bar = viewDef.AddView(ViewType.FogProgress) as FogProgressView;
        
        if(bar.IsShow) return;
        
        bar.SetOffset(Def.GetCenter(Context.Context) + new Vector3(0, 0.1f));
        bar.Priority = -1;
        bar.Change(true);
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

        return pathExists && resourcesEnough && Def.IsActive;
    }
    
    public bool CanBeFilled()
    {
        return AlreadyPaid.Amount < Def.Condition.Amount;
    }
    
    public bool RequiredLevelReached()
    {
        return storageItem.Amount >= level;
    }

    public bool IsActive => CanBeReached() || (RequiredLevelReached() && Def.IsActive);

    public void Filling(int value)
    {
        Action onComplete = null;
        
        AlreadyPaid.Amount += Mathf.Clamp(value, 0, Def.Condition.Amount - AlreadyPaid.Amount);

        if (CanBeFilled() == false)
        {
            onComplete = () =>
            {
                bar.Priority = 1;
                bar.Change(false);
                OpenBubble();
            };
        }
        
        bar.UpdateProgress(onComplete);
    }
    
    public void FillingFake(int value)
    {
        if (bar != null) bar.UpdateFakeProgress(value);
    }
    
    private void OnClick(Piece piece)
    {
        bubble.OnHide = () =>
        {
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
        };
            
        bubble.Priority = 1;
        bubble.Change(false);
    }

    private void OpenBubble()
    {
        bubble = viewDef.AddView(ViewType.Bubble) as BubbleView;
        bubble.SetOffset(Def.GetCenter(Context.Context) + new Vector3(0, 0.1f));
                
        bubble.SetData(LocalizationService.Get("gameboard.bubble.message.fog", "gameboard.bubble.message.fog"),
            LocalizationService.Get("gameboard.bubble.button.fog", "gameboard.bubble.button.fog"), OnClick);
                
        bubble.Priority = -2;
        bubble.Change(true);
    }
}