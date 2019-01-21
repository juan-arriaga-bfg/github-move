using System.Linq;
using UnityEngine;

public class RestoredPurchasesProvider : MonoBehaviour
{
    private void Start()
    {
        ScheduleProvide();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            return;
        }
        
        ScheduleProvide();
    }

    private void ScheduleProvide()
    {
        var iapService = IapService.Current;

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
                HardCurrencyHelper.ProvideReward(id);
            });
            
            //todo: add more conditions to sequence many pendingIap
            
            ProfileService.Current.QueueComponent.AddAction(action, true);
        }
    }
}