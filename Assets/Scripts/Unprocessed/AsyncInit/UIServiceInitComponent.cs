using BfgAnalytics;
using IW.Content.ContentModule;
using UnityEngine;

public class UIServiceInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        GameObject.DontDestroyOnLoad(UIService.Get);
        UIService.Get.DontDestroyPoolOnSceneChange = true;

        isCompleted = true;
        OnComplete(this);
    }
}