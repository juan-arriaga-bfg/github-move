using System.Collections.Generic;

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
        
        Locker.Lock(this);
        
        Save = ProfileService.Current.GetComponent<TutorialSaveComponent>(TutorialSaveComponent.ComponentGuid)?.Complete ?? new List<int>();
        
        if(Save.Count > 0) Locker.Unlock(this);
        
        for (var i = 0; i < TutorialBuilder.Amount; i++)
        {
            if(Save.Contains(i)) continue;

            var tutorial = TutorialBuilder.BuildTutorial(i, Context);
            
            if(tutorial == null) continue;
            
            RegisterComponent(tutorial, true);
        }
        
        GameDataService.Current.QuestsManager.OnActiveQuestsListChanged += Start;
    }
    
    private void Start()
    {
        var target = GameDataService.Current.QuestsManager.ActiveQuests.Find(entity => entity.Id == "1");

        if (target == null) return;
        
        Locker.Unlock(this);
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
            
            if(condition.IsStart() == false) continue;
            
            if (isOn) condition.PauseOn();
            else condition.PauseOff();
        }
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