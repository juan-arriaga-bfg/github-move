using System.Collections.Generic;

public class TutorialDataManager: IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    private  HashSet<int> completed;
    private  HashSet<int> started;

    public HashSet<int> Completed => completed;
    public HashSet<int> Started => started;

    private ECSEntity context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;
        
        var tutorialSave = ((GameDataManager)context).UserProfile.GetComponent<TutorialSaveComponent>(TutorialSaveComponent.ComponentGuid);
        if (tutorialSave != null)
        {
            completed = tutorialSave.Complete ?? new HashSet<int>();
            started   = tutorialSave.Started  ?? new HashSet<int>();
        }
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
    }
    
    public bool CheckUnlockWorker()
    {
        return completed.Contains(TutorialBuilder.WorkerStepIndex);
    }
    
    public bool CheckLockOrders()
    {
        return completed.Contains(TutorialBuilder.LockOrderStepIndex);
    }
    
    public bool CheckFirstOrder()
    {
        return completed.Contains(TutorialBuilder.FirstOrderStepIndex);
    }

    public bool CheckMarket()
    {
        return completed.Contains(TutorialBuilder.LockMarketStepIndex);
    }

    public bool CheckUnlockEventGame()
    {
        return completed.Contains(TutorialBuilder.LockEventGameStepIndex);
    }

    public bool IsCompeted(int id)
    {
        return completed.Contains(id);
    }

    public void SetCompleted(int id)
    {
        if (completed.Contains(id)) return;
        
        IW.Logger.Log($"[TutorialDataManager] => SetCompleted({id})");
        completed.Add(id);
    }
    
    public bool IsStarted(int id)
    {
        return started.Contains(id);
    }
    
    public void SetStarted(int id)
    {
        IW.Logger.Log($"[TutorialDataManager] => SetStarted({id})");
        started.Add(id);
    }
}