﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using IW.Content.ContentModule;
using UnityEditor;
using Random = UnityEngine.Random;

public class DefaultApplicationInitilizer : ApplicationInitializer 
{
    public override void Init(Action onComplete)
    {
        Application.targetFrameRate = 60;

        DOTween.SetTweensCapacity(200, 125);

        StartCoroutine(InitCoroutine(onComplete));
    }
    
    private IEnumerator InitCoroutine(Action onComplete)
    {
        // Wait at least for one frame to render loading screen
        yield return new WaitForEndOfFrame();
        
        AsyncInitManager asyncInitManager = new AsyncInitManager();
        AsyncInitService.Instance.SetManager(asyncInitManager);
        asyncInitManager
           .AddComponent(new InternetMonitorInitComponent())
           .AddComponent(new BfgSdkUnityMessageHandlerInitComponent())
            
           .AddComponent(new IapInitComponent()
                .SetDependency(typeof(BfgSdkUnityMessageHandlerInitComponent))
                .SetDependency(typeof(ConfigsAndManagersInitComponent)))
            
           .AddComponent(new IapRestoreInitComponent()
               .SetDependency(typeof(IapInitComponent)))
            
           .AddComponent(new SecuredTimeServiceInitComponent())
           .AddComponent(new BfgSdkGdprInitComponent())
           .AddComponent(new ConfigsAndManagersInitComponent())
           .AddComponent(new UIInitProgressListenerComponent())
            
           .AddComponent(new LocalBundlesInitComponent()
               .SetDependency(typeof(ConfigsAndManagersInitComponent)))
            
           .AddComponent(new ShowLoadingWindowInitComponent()
               .SetDependency(typeof(LocalBundlesInitComponent)))

           .AddComponent(new MainSceneLoaderComponent()
               .SetDependency(typeof(LocalBundlesInitComponent))
               .SetDependency(typeof(SecuredTimeServiceInitComponent)))
            
           .Run(onComplete);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        var energyLogic = BoardService.Current?.FirstBoard?.GetComponent<EnergyCurrencyLogicComponent>(EnergyCurrencyLogicComponent.ComponentGuid);
        if (pauseStatus)
        {
            ProfileService.Instance.Manager.UploadCurrentProfile();
            
#if UNITY_EDITOR
            ProfileService.Instance.Manager.SaveLocalProfile();
            LocalNotificationsService.Current.RefreshNotifications();
#endif
            energyLogic?.Timer.Stop();
        }
        else
        {
            energyLogic?.InitInSave();
            
            LocalNotificationsService.Current.ClearNotifications();
        }
    }

    void OnApplicationQuit()
    {
        ProfileService.Instance.Manager.UploadCurrentProfile();

#if UNITY_EDITOR
        LocalNotificationsService.Current.RefreshNotifications();
        
        ProfileService.Instance.Manager.SaveLocalProfile();
        AssetDatabase.Refresh();
#endif
    }
}