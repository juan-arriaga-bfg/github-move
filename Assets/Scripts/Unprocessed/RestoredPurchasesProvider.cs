using System.Linq;
using UnityEngine;

public class RestoredPurchasesProvider : MonoBehaviour
{
    private void Start()
    {
        var service = IapService.Current;
        
        service.OnRestoreCompleted += OnRestoreCompleted;
        service.OnPurchaseOK += OnPurchaseOk;

        if (service.IsInitialized)
        {
            ScheduleProvide();
            IapService.Current.RestorePurchases();
        }
        else
        {
            service.OnInitialized += OnServiceInitialized;
        }
    }

    private string GetActionId(string productId)
    {
        return "RESTORE_" + productId;
    }
    
    private void OnServiceInitialized()
    {
        var service = IapService.Current;
        
        service.OnInitialized -= OnServiceInitialized;
        
        service.RestorePurchases();
    }
    
    private void OnPurchaseOk(string productId, string receipt, bool restore)
    {
        ProfileService.Current?.QueueComponent.RemoveAction(GetActionId(productId));// Avoid double provide
    }

    private void OnDestroy()
    {
        var service = IapService.Current;
        
        service.OnRestoreCompleted -= OnRestoreCompleted; 
        service.OnInitialized -= OnServiceInitialized;
        service.OnPurchaseOK -= OnPurchaseOk;
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
        if (ProfileService.Current == null)
        {
            Debug.LogWarning($"[RestoredPurchasesProvider] => ScheduleProvide: ProfileService.Current == null");
            return;
        }
        
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
            var productId = pendingIap.Key;
            var actionId = GetActionId(productId);

            var action = DefaultSafeQueueBuilder.Build(actionId, true, () =>
            {
                if (ProfileService.Current == null)
                {
                    Debug.LogError($"[RestoredPurchasesProvider] => Action: ProfileService.Current == null");
                    return;
                }
                
                if (SellForCashService.Current == null)
                {
                    Debug.LogError($"[RestoredPurchasesProvider] => Action: SellForCashService.Current == null");
                    return;
                }
                
                if (!AsyncInitService.Current.IsAllComponentsInited())
                {
                    Debug.LogError($"[RestoredPurchasesProvider] => Action: AsyncInitService.IsAllComponentsInited() == false");
                    return;
                }
                
                var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

                model.Title = LocalizationService.Get("window.restore.purchase.title",       "window.restore.purchase.title");
                model.Message = LocalizationService.Get("window.restore.purchase.message",   "window.restore.purchase.message");
                model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");

                model.OnClose = () =>
                {
                    CurrencyHelper.FlyPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
                    SellForCashService.Current.ProvideReward(productId);
                    ProfileService.Current.QueueComponent.RemoveAction(actionId); // For case if we have scheduled the action once again while Restore window is in progress
                };

                // model.OnCancel = model.OnAccept;

                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            });
            
            //todo: add more conditions to sequence many pendingIaps?
            
            ProfileService.Current.QueueComponent.AddAction(action, true);
        }
    }
}