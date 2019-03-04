using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CleanupForReloadInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        DOTween.KillAll();
        
        var manager = GameDataService.Current.QuestsManager;
        manager.Cleanup();
            
        BoardService.Current.Cleanup();
        BoardService.Instance.SetManager(null);

        Object.Destroy(ResourcesViewManager.Instance.gameObject);
        
        ShopService.Current.Cleanup();
        ShopService.Instance.SetManager(null);

        GameDataService.Instance.SetManager(null);

        ProfileService.Current.QueueComponent.StopAndClear();
        ProfileService.Instance.SetManager(null);

        LocalNotificationsService.Current.Cleanup();
        LocalNotificationsService.Instance.SetManager(null);
        
        var ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);
            
        foreach (var system in ecsSystems)
        {
            ECSService.Current.SystemProcessor.UnRegisterSystem(system);
        }

        InternetMonitorService.Current.Cleanup();
        InternetMonitorService.Instance.SetManager(null);
        
        isCompleted = true;
        OnComplete(this);
    }
}