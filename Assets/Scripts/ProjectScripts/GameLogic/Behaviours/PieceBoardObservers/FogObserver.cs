using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FogObserver : MulticellularPieceBoardObserver, IResourceCarrierView
{
    public FogDef Def { get; private set; }
    public CurrencyPair AlreadyPaid { get; private set; }
    
    private StorageItem storageItem;
    private ViewDefinitionComponent viewDef;
    public LockView LockView;
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

        if (AlreadyPaid.Amount >= Def.Condition.Amount)
        {
            OpenBubble();
            return;
        }
        
        canBeReachedCached = null;
        
        var canPath = CanBeReached();
        
        if(LockView == null && (Def.IsActive && canPath ^ RequiredConditionReached() || Def.IsActive == false && canPath))
        {
            LockView =  Context.Context.RendererContext.CreateBoardElement<LockView>((int)ViewType.Lock);
            LockView.Init(Context.Context.RendererContext);
            LockView.SetSortingOrder(Def.GetCenter());
            LockView.transform.position = Def.GetCenter(Context.Context);
        }

        if (LockView != null)
        {
            LockView.SetCondition(Def.IsActive ? Def.Level.ToString() : "?", Def.Hero, IsLevelReached, IsHeroReached, IsTwoLock);
            LockView.SetGrayscale(canPath == false || Def.IsActive == false);
        }
        
        var fog = Context.ActorView as FogPieceView;

        if (fog != null) fog.UpdateBorder();
        if (CanBeCleared() == false) return;

        if (LockView != null)
        {
            Context.Context.RendererContext.DestroyElement(LockView);
            LockView = null;

            var pos = Def.GetCenter();

            pos.Z = 4;
            
            ParticleView.Show(R.FogExplosionParticleSystem, pos);
        }
        
        bar = viewDef.AddView(ViewType.FogProgress) as FogProgressView;
        
        if(bar.IsShow) return;
        
        bar.Priority = -1;
        bar.Change(true);
        
        if(Context.Context.Manipulator.CameraManipulator.CameraMove.IsLocked) return;
        
        Context.Context.Manipulator.CameraManipulator.MoveTo(Def.GetCenter(Context.Context));
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
        var resourcesEnough = RequiredConditionReached();

        return pathExists && resourcesEnough && Def.IsActive;
    }
    
    public bool CanBeFilled()
    {
        return AlreadyPaid.Amount < Def.Condition.Amount;
    }
    
    public bool RequiredConditionReached()
    {
        return IsLevelReached && IsHeroReached;
    }

    public bool IsLevelReached => storageItem.Amount >= Def.Level;
    
    public bool IsHeroReached => Def.HeroId == PieceType.None.Id || GameDataService.Current.CodexManager.IsPieceUnlocked(Def.HeroId);

    public bool IsTwoLock => Def.HeroId != PieceType.None.Id;

    public bool IsActive => CanBeReached() || (RequiredConditionReached() && Def.IsActive);

    public void Filling(int value, out int change)
    {
        Action onComplete = null;
        var target = Def.Condition.Amount - AlreadyPaid.Amount;
        
        change = value - target;
        AlreadyPaid.Amount += Mathf.Clamp(value, 0, target);

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
        if (bar != null)
        {
            bar.UpdateFakeProgress(value);
            
            FogPieceView fog = Context.ActorView as FogPieceView;
            if (fog != null)
            {
                fog.ToggleHighlightWhenReadyToClear(value > 0);
            }
        }
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
                OnComplete = () =>
                {
                    ProfileService.Instance.Manager.UploadCurrentProfile();
                    DevTools.UpdateFogSectorsDebug();
                }
            });
        };
            
        bubble.Priority = 1;
        bubble.Change(false);
    }

    private void OpenBubble()
    {
        bubble = viewDef.AddView(ViewType.Bubble) as BubbleView;
                
        bubble.SetData(LocalizationService.Get("gameboard.bubble.message.fog", "gameboard.bubble.message.fog"),
            LocalizationService.Get("gameboard.bubble.button.fog", "gameboard.bubble.button.fog"), OnClick);
                
        bubble.Priority = -2;
        bubble.Change(true);
    }
}