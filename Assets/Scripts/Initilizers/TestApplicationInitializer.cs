using Debug = IW.Logger;
using System;
using System.Collections;
using UnityEngine;

public class TestApplicationInitializer : ApplicationInitializer 
{
    public override void Init(Action onComplete)
    {
#if !DEBUG
    Debug.IsEnabled = false;
#endif
        
        Application.targetFrameRate = 60;
        StartCoroutine(InitCoroutine(onComplete));
    }

    private IEnumerator InitCoroutine(Action onComplete)
    {
        // Wait at least for one frame to render loading screen
        yield return new WaitForEndOfFrame();
        
        AsyncInitManager asyncInitManager = new AsyncInitManager();
        AsyncInitService.Instance.SetManager(asyncInitManager);
        asyncInitManager
            .AddComponent(new ContentAndIconManagersInitComponent())
            .AddComponent(new LocalAssetBundlesCacheInitComponent());
            

        asyncInitManager.Run(onComplete);
    }
}