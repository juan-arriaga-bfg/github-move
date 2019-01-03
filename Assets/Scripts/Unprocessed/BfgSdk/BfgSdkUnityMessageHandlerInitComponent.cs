using UnityEngine;

public class BfgSdkUnityMessageHandlerInitComponent : AsyncInitComponent
{
    public override bool IsCompleted => true;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        UnityMessageHandler messageHandler = new GameObject("[BFGSDK_UnityMessageHandler]").AddComponent<UnityMessageHandler>();
        Object.DontDestroyOnLoad(messageHandler);
    }
}