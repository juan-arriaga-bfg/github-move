//#define REAL_IAP

public class IapInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        InitIapService();
        InitCashService();

        isCompleted = true;
        OnComplete(this);
    }

    private void InitCashService()
    {
        SellForCashManager manager = new SellForCashManager();
        SellForCashService.Instance.SetManager(manager);
        
        manager.Init();
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
#if REAL_IAP
                                .Add(new IapDefinition
                                 {
                                     Id = "iap1",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier2",
                                     AppleAppStoreId  = "vi.tier2.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "iap2",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier5",
                                     AppleAppStoreId  = "vi.tier5.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "iap3",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier10",
                                     AppleAppStoreId  = "vi.tier10.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "iap4",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier20",
                                     AppleAppStoreId  = "vi.tier20.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "iap5",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier50",
                                     AppleAppStoreId  = "vi.tier50.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "iap6",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier60",
                                     AppleAppStoreId  = "vi.tier60.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "iap7",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier70",
                                     AppleAppStoreId  = "vi.tier70.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
                                .Add(new IapDefinition
                                 {
                                     Id = "iap8",
                                     GooglePlayId     = "com.bigfishgames.mergetalesgoog.tier80",
                                     AppleAppStoreId  = "vi.tier80.com.bigfishgames.mergetalesios",
                                     Consumable = true
                                 })
#endif
                ))
#endregion
           .Init();
    }
}