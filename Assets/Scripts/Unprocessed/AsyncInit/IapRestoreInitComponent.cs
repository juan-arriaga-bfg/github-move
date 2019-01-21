using UnityEngine;

public class IapRestoreInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        InitRestoredPurchasesProvider();
        
        isCompleted = true;
        OnComplete(this);
    }

    private void InitRestoredPurchasesProvider()
    {
        RestoredPurchasesProvider go = new GameObject("[RestoredPurchasesProvider]").AddComponent<RestoredPurchasesProvider>();
        Object.DontDestroyOnLoad(go);
    }
}