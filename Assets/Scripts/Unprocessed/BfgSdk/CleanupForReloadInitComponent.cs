using System.Collections.Generic;

public class CleanupForReloadInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        var manager = GameDataService.Current.QuestsManager;
        manager.DisconnectFromBoard();
            
        BoardService.Instance.SetManager(null);

        GameDataService.Instance.SetManager(null);
         
        ProfileService.Current.QueueComponent.StopAndClear();
        
        ProfileService.Instance.SetManager(null);
        
        var ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);
            
        foreach (var system in ecsSystems)
        {
            ECSService.Current.SystemProcessor.UnRegisterSystem(system);
        }

        isCompleted = true;
        OnComplete(this);
    }
}