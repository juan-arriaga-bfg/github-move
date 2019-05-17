using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialLogicComponent : ECSEntity, ILockerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private LockerComponent locker;
    public virtual LockerComponent Locker => locker ?? (locker = GetComponent<LockerComponent>(LockerComponent.ComponentGuid));
    
    public BoardController Context;

    private TutorialDataManager tutorialDataManager;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        Context = entity as BoardController;

        tutorialDataManager = GameDataService.Current.TutorialDataManager;
        
        UnlockFirefly(false);
        UnlockOrders(false);
    }
    
    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        UIService.Get.OnShowWindowEvent -= OnShowWindow;
        UIService.Get.OnCloseWindowEvent -= OnCloseWindow;
        GameDataService.Current.QuestsManager.OnActiveQuestsListChanged -= Update;
        GameDataService.Current.QuestsManager.OnQuestStateChanged -= OnQuestStateChanged;
    }

    public void Run()
    {
        for (var i = 0;; i++)
        {
            if(tutorialDataManager.IsCompeted(i)) continue;

            var tutorial = TutorialBuilder.BuildTutorial(i, Context);

            if (tutorial == null)
            {
                var collection = GetComponent<ECSComponentCollection>(BaseTutorialStep.ComponentGuid);
                if (collection?.Components == null)
                {
                    Locker.Lock(this);
                    return;
                }
                
                break;
            }
            
            RegisterComponent(tutorial, true);
        }

        UnlockFirefly(true);
        UnlockOrders(true);
        
        Context.BoardLogic.EventGamesLogic.Unlock();
        
        UIService.Get.OnShowWindowEvent += OnShowWindow;
        UIService.Get.OnCloseWindowEvent += OnCloseWindow;
        GameDataService.Current.QuestsManager.OnActiveQuestsListChanged += Update;
        GameDataService.Current.QuestsManager.OnQuestStateChanged += OnQuestStateChanged;
        Update();
    }
    
    private void OnShowWindow(IWUIWindow window)
    {
        if (UIWindowType.IsIgnore(window.WindowName)) return;
        if (Locker.IsLocked == false) Pause(true);
		
        Locker.Lock(this);
    }
	
    private void OnCloseWindow(IWUIWindow window)
    {
        if (UIWindowType.IsIgnore(window.WindowName)) return;
		
        Locker.Unlock(this);

        if (Locker.IsLocked) return;

        DOTween.Sequence()
            .AppendInterval(0.25f)
            .AppendCallback(() =>
            {
                Pause(false);
                Update();
            });
    }
    
    public void Pause(bool isOn)
    {
        if (Locker.IsLocked) return;
        
        var collection = GetComponent<ECSComponentCollection>(BaseTutorialStep.ComponentGuid);
        var components = collection?.Components;

        if (components == null) return;

        for (var i = components.Count - 1; i >= 0; i--)
        {
            var step = (BaseTutorialStep) components[i];

            if (step.IsPerform == false) continue;
            
            if (isOn) step.PauseOn();
            else step.PauseOff();
        }
    }

    private void OnQuestStateChanged(QuestEntity quest, TaskEntity task)
    {
        Update();
        UpdateHard();
    }
    
    public void Update()
    {
        if (Locker.IsLocked) return;
        
        var collection = GetComponent<ECSComponentCollection>(BaseTutorialStep.ComponentGuid);
        
        UpdateComponents(collection?.Components);
    }

    public void UpdateHard()
    {
        var collection = GetComponent<ECSComponentCollection>(BaseTutorialStep.ComponentGuid);
        var components = collection?.Components?.FindAll(component => (component as BaseTutorialStep)?.IsIgnoreUi == true);

        UpdateComponents(components);
    }

    private void UpdateComponents(List<IECSComponent> components)
    {
        if(components == null) return;
        
        for (var i = components.Count - 1; i >= 0; i--)
        {
            var condition = (BaseTutorialStep) components[i];

            if (!condition.IsComplete()) continue;
            
            UnRegisterComponent(condition);
            components.Remove(condition);
        }
        
        for (var i = components.Count - 1; i >= 0; i--)
        {
            var condition = (BaseTutorialStep) components[i];
            
            if(condition.IsStart() == false) continue;
            
            condition.Perform();
        }
    }
    
    public void LockAll()
    {
        for (var i = 0; i < Context.BoardDef.Height; i++)
        {
            for (var j = 0; j < Context.BoardDef.Width; j++)
            {
                Context.BoardLogic.LockCell(new BoardPosition(i, j, BoardLayer.Piece.Layer), this);
            }
        }
    }

    public void UnlockCell(BoardPosition cell)
    {
        Context.BoardLogic.UnlockCell(cell, this);
    }

    public void UnlockCells(List<BoardPosition> cells)
    {
        foreach (var cell in cells)
        {
            Context.BoardLogic.UnlockCell(cell, this);
        }
    }
    
    public void UnlockAll()
    {
        for (var i = 0; i < Context.BoardDef.Height; i++)
        {
            for (var j = 0; j < Context.BoardDef.Width; j++)
            {
                Context.BoardLogic.UnlockCell(new BoardPosition(i, j, BoardLayer.Piece.Layer), this);
            }
        }
    }

    public void FadeAll(float alpha, List<BoardPosition> exclude)
    {
        var points = Context.BoardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Default);

        foreach (var point in points)
        {
            if (exclude != null && exclude.Contains(point)) continue;
            
            var pieceEntity = Context.BoardLogic.GetPieceAt(point);
            
            if (pieceEntity == null || pieceEntity.PieceType == PieceType.Fog.Id) continue;
                
            var pieceView = Context.RendererContext.GetElementAt(point) as PieceBoardElementView;

            pieceEntity.ViewDefinition?.SetFade(alpha > 0.99f ? 1f : 0f, 1f);

            if (pieceView != null) pieceView.SetFade(alpha, 1f);

            var timer = pieceEntity.ViewDefinition?.GetView<BoardTimerView>(ViewType.BoardTimer)?.Timer;

            if (timer != null) timer.IsPaused = !(alpha > 0.99f);
        }
    }

    public void ResetStartTime()
    {
        BoardService.Current.FirstBoard.BoardLogic.FireflyLogic.ResetTutorialStartTime();
        
        var collection = GetComponent<ECSComponentCollection>(BaseTutorialStep.ComponentGuid);
        var components = collection?.Components;

        if (components == null) return;

        for (var i = 0; i < components.Count; i++)
        {
            var step = components[i] as BaseTutorialStep;

            if (step?.IsPerform != true) continue;
            
            step.StartTime = DateTime.UtcNow;
        }
    }
    
    private void UnlockFirefly(bool isRun)
    {
        if (isRun == false && tutorialDataManager.IsCompeted(TutorialBuilder.LockFireflyStepIndex) == false) return;
        
        var firefly = Context.BoardLogic.FireflyLogic;
        firefly.Locker.Unlock(firefly);
    }
    
    private void UnlockOrders(bool isRun)
    {
        if (isRun == false && tutorialDataManager.CheckLockOrders() == false) return;
        
        var orders = GameDataService.Current.OrdersManager;
        orders.Locker.Unlock(orders);
    }
}