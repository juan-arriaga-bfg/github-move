using UnityEngine;
using Object = UnityEngine.Object;

public class BfgSdkUnityMessageHandlerInitComponent : AsyncInitItemBase
{
    public override void Execute()
    {
        UnityMessageHandler messageHandler = new GameObject("[BFGSDK_UnityMessageHandler]").AddComponent<UnityMessageHandler>();
        Object.DontDestroyOnLoad(messageHandler);

        isCompleted = true;
        OnComplete(this);
    }
}