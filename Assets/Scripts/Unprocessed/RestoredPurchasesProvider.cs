using System.Linq;
using UnityEngine;

public class RestoredPurchasesProvider : MonoBehaviour
{
    private void Start()
    {
        ScheduleProvide();

        var service = IapService.Current;
        
        service.OnRestoreCompleted += OnRestoreCompleted;

        if (service.IsInitialized)
        {
            IapService.Current.RestorePurchases();
        }
        else
        {
            service.OnInitialized += OnServiceInitialized;
        }
    }

    private void OnServiceInitialized()
    {
        var service = IapService.Current;
        
        service.OnInitialized -= OnServiceInitialized;
        
        service.RestorePurchases();
    }

    private void OnDestroy()
    {
        var service = IapService.Current;
        
        service.OnRestoreCompleted -= OnRestoreCompleted; 
        service.OnInitialized -= OnServiceInitialized;
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            return;
        }
        
        ScheduleProvide();
    }

    private void OnRestoreCompleted(bool isOk)
    {
        ScheduleProvide();
    }
    
    private void ScheduleProvide()
    {
        var iapService = IapService.Current;

        if (iapService == null)
        {
            Debug.LogWarning($"[RestoredPurchasesProvider] => ScheduleProvide: IapService.Current == null");
            return;
        }
        
        if (iapService.PendingIaps.Count > 0)
        {
            Debug.Log($"[RestoredPurchasesProvider] => ScheduleProvide: {string.Join(" | ", iapService.PendingIaps.Keys.ToList())}");
        }
        else
        {
            Debug.Log($"[RestoredPurchasesProvider] => ScheduleProvide: No pending iap");
            return;
        }
        
        foreach (var pendingIap in iapService.PendingIaps)
        {           
            var id = pendingIap.Key;

            var action = DefaultSafeQueueBuilder.Build(id, true, () =>
            {
                if (SellForCashService.Current == null)
                {
                    Debug.LogError($"[RestoredPurchasesProvider] => Action: SellForCashService.Current == null");
                    return;
                }
                
                var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

                model.Title = LocalizationService.Get("window.restore.purchase.title",       "window.restore.purchase.title");
                model.Message = LocalizationService.Get("window.restore.purchase.message",   "window.restore.purchase.message");
                model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");

                model.OnClose = () =>
                {
                    SellForCashService.Current.ProvideReward(id);
                    ProfileService.Current.QueueComponent.RemoveAction(id); // For case if we have scheduled the action once again while Restore window is in progress
                };

                // model.OnCancel = model.OnAccept;

                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            });
            
            //todo: add more conditions to sequence many pendingIaps?
            
            ProfileService.Current.QueueComponent.AddAction(action, true);
        }
    }
}