using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IW.Content.ContentModule;
using UnityEditor;
using Random = UnityEngine.Random;

public class DefaultApplicationInitilizer : ApplicationInitializer 
{
    public override void Init(Action onComplete)
    {
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
           .AddItem(new BfgSdkUnityMessageHandlerInitComponent())
           .AddItem(new BfgSdkGdprInitComponent())
           .AddItem(new ConfigsAndManagersInitComponent())
           .AddItem(new UIInitProgressListenerComponent())
            
           .AddItem(new LocalBundlesInitComponent().SetDependency(typeof(ConfigsAndManagersInitComponent)))
            
           .AddItem(new ShowLoadingWindowInitComponent().SetDependency(typeof(LocalBundlesInitComponent)))

           .AddItem(new MainSceneLoaderComponent().SetDependency(typeof(LocalBundlesInitComponent)))
            
           .Run();
            
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ProfileService.Instance.Manager.UploadCurrentProfile();
            
#if UNITY_EDITOR
            ProfileService.Instance.Manager.SaveLocalProfile();
#endif
        }
    }

    void OnApplicationQuit()
    {
        ProfileService.Instance.Manager.UploadCurrentProfile();

#if UNITY_EDITOR
        ProfileService.Instance.Manager.SaveLocalProfile();
        AssetDatabase.Refresh();
#endif
    }

}