﻿using System.Collections.Generic;

public class TutorialLogicComponent : ECSEntity, ILockerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private LockerComponent locker;
    public virtual LockerComponent Locker => locker ?? (locker = GetComponent<LockerComponent>(LockerComponent.ComponentGuid));
    
    public BoardController Context;
    public List<int> Save;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        Context = entity as BoardController;
        
        Save = ProfileService.Current.GetComponent<TutorialSaveComponent>(TutorialSaveComponent.ComponentGuid)?.Complete ?? new List<int>();
        
        for (var i = 0;; i++)
        {
            if(Save.Contains(i)) continue;

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
        
        UIService.Get.OnShowWindowEvent += OnShowWindow;
        UIService.Get.OnCloseWindowEvent += OnCloseWindow;
        GameDataService.Current.QuestsManager.OnActiveQuestsListChanged += Update;
        GameDataService.Current.QuestsManager.OnQuestStateChanged += OnQuestStateChanged;
    }
    
    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        UIService.Get.OnShowWindowEvent -= OnShowWindow;
        UIService.Get.OnCloseWindowEvent -= OnCloseWindow;
        GameDataService.Current.QuestsManager.OnActiveQuestsListChanged -= Update;
        GameDataService.Current.QuestsManager.OnQuestStateChanged -= OnQuestStateChanged;
    }
    
    private void OnShowWindow(IWUIWindow window)
    {
        if(UIWindowType.IsIgnore(window.WindowName)) return;

        if (Locker.IsLocked == false) Pause(true);
		
        Locker.Lock(this);
    }
	
    private void OnCloseWindow(IWUIWindow window)
    {
        if(UIWindowType.IsIgnore(window.WindowName)) return;
		
        Locker.Unlock(this);
		
        if(Locker.IsLocked) return;
		
        Pause(false);
        Update();
    }
    
    public void Pause(bool isOn)
    {
        if (Locker.IsLocked) return;
        
        var collection = GetComponent<ECSComponentCollection>(BaseTutorialStep.ComponentGuid);
        var components = collection?.Components;
        
        if(components == null) return;

        for (var i = components.Count - 1; i >= 0; i--)
        {
            var condition = (BaseTutorialStep) components[i];
            
            if(condition.IsPerform == false) continue;
            
            if (isOn) condition.PauseOn();
            else condition.PauseOff();
        }
    }

    private void OnQuestStateChanged(QuestEntity quest, TaskEntity task)
    {
        Update();
    }
    
    public void Update()
    {
        if (Locker.IsLocked) return;
        
        var collection = GetComponent<ECSComponentCollection>(BaseTutorialStep.ComponentGuid);
        var components = collection?.Components;
        
        if(components == null) return;
        
        for (var i = components.Count - 1; i >= 0; i--)
        {
            var condition = (BaseTutorialStep) components[i];

            if (!condition.IsComplete()) continue;
            
            UnRegisterComponent(condition);
            components.Remove(condition);
            Save.Add(condition.Id);
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
                Context.BoardLogic.LockCell(new BoardPosition(i, j, Context.BoardDef.PieceLayer), this);
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
                Context.BoardLogic.UnlockCell(new BoardPosition(i, j, Context.BoardDef.PieceLayer), this);
            }
        }
    }
}