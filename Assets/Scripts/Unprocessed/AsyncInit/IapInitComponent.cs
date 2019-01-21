using UnityEngine;

public class IapInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        // InitIapService();
        // InitRestoredPurchasesProvider();
        
        isCompleted = true;
        OnComplete(this);
    }

    private void InitRestoredPurchasesProvider()
    {
        RestoredPurchasesProvider go = new GameObject("[RestoredPurchasesProvider]").AddComponent<RestoredPurchasesProvider>();
        Object.DontDestroyOnLoad(go);
    }

    private static void InitIapService()
    {
        // Init IAP service
        IapManager iapManager = new IapManager();
        IapService.Instance.SetManager(iapManager);

        iapManager
            // .AddIapValidator(new DummyIapValidator())
            // .AddIapValidator(new BackendIapValidator()
            //     .SetEndpoint("http://127.0.0.1:8080/utils/validateiap"))
            // .AddIapValidator(new LocalReceiptValidator())   
           .SetIapProvider(new BfgIapProvider()
               .SetIapCollection(new IapCollection()
#region IAPS
                                .Add(new IapDefinition
                                 {
                                     Id = "coins1",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier2",
                                     AppleAppStoreId  = "vi.tier2.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "coins2",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.5",
                                     AppleAppStoreId  = "vi.tier5.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "coins3",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier10",
                                     AppleAppStoreId  = "vi.tier10.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "coins4",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier20",
                                     AppleAppStoreId  = "vi.tier20.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "coins5",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier50",
                                     AppleAppStoreId  = "vi.tier50.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "coins6",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier60",
                                     AppleAppStoreId  = "vi.tier60.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                ))
#endregion
           .Init();
    }
}

public class RestoredPurchasesProvider : MonoBehaviour
{
    private void Start()
    {
        // IapService.Current.
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            return;
        }
    }
    
    // private void Schedule
}