﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using IW.Content.ContentModule;
using UnityEditor;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DefaultApplicationInitilizer : ApplicationInitializer 
{
    public override void Init(Action onComplete)
    {
#if !DEBUG
        Debug.unityLogger.logEnabled = false;
#endif
        
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
           .AddComponent(new InternetMonitorInitComponent())                        // Monitors current state of internet connection
           .AddComponent(new BfgSdkUnityMessageHandlerInitComponent())              // Used by BFG SDK to make calls to make calls to native code 
           .AddComponent(new SecuredTimeServiceInitComponent())                     // Anti-cheat protection for timers
           .AddComponent(new BfgSdkGdprInitComponent())                             // Listener for BFG SDK's GDPR popup events
           .AddComponent(new BfgSdkAnalyticsInitComponent()) 
           .AddComponent(new ContentAndIconManagersInitComponent()) 
           .AddComponent(new EcsSystemProcessorInitComponent()) 
           .AddComponent(new AudioServiceInitComponent()) 
           .AddComponent(new ShopServiceInitComponent()) 
           .AddComponent(new LocalAssetBundlesCacheInitComponent())
            
           .AddComponent(new ProfileInitComponent())

           .AddComponent(new UIServiceInitComponent()
               .SetDependency(typeof(LocalAssetBundlesCacheInitComponent)))  

           .AddComponent(new GameDataInitComponent()  
            .SetDependency(typeof(ProfileInitComponent)))
            
           .AddComponent(new LocalizationInitComponent()
               .SetDependency(typeof(ProfileInitComponent)))

           .AddComponent(new NotificationServiceInitComponent()
               .SetDependency(typeof(LocalizationInitComponent))
               .SetDependency(typeof(ProfileInitComponent)))
            
           .AddComponent(new IapInitComponent()                                     // In-app purchases implementation
                .SetDependency(typeof(BfgSdkUnityMessageHandlerInitComponent))
                .SetDependency(typeof(LocalizationInitComponent)))
            
           .AddComponent(new IapRestoreInitComponent()                              // Handler for restored In-app purchases 
               .SetDependency(typeof(IapInitComponent)))
            
           .AddComponent(new ShowLoadingWindowInitComponent()
               .SetDependency(typeof(LocalAssetBundlesCacheInitComponent))
               .SetDependency(typeof(LocalizationInitComponent)));

        if (SceneManager.GetActiveScene().name != "Main") // Handle case when we start the game from the Main scene in the Editor.  
        {
            asyncInitManager.AddComponent(new UIInitProgressListenerComponent());
            
            asyncInitManager.AddComponent(new MainSceneLoaderComponent()
                .SetDependency(typeof(LocalAssetBundlesCacheInitComponent))
                .SetDependency(typeof(LocalizationInitComponent))
                .SetDependency(typeof(ProfileInitComponent))
                .SetDependency(typeof(SecuredTimeServiceInitComponent)));
        }

        asyncInitManager.Run(onComplete);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log($"OnApplicationPause: {(pauseStatus ? "PAUSE" : "UNPAUSE")}");
        
        var energyLogic = BoardService.Current?.FirstBoard?.GetComponent<EnergyCurrencyLogicComponent>(EnergyCurrencyLogicComponent.ComponentGuid);
        if (pauseStatus)
        {
            if (ProfileService.Instance != null && ProfileService.Instance.Manager != null)
            {
                ProfileService.Instance.Manager.UploadCurrentProfile();

#if UNITY_EDITOR
                ProfileService.Instance.Manager.SaveLocalProfile();
#endif
                LocalNotificationsService.Current.ScheduleNotifications();
            }

            energyLogic?.Timer.Stop();
        }
        else
        {
            energyLogic?.InitInSave();
            
            BoardService.Current?.FirstBoard?.TutorialLogic?.ResetStartTime();
            LocalNotificationsService.Current.CancelNotifications();
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log($"OnApplicationQuit");
        
        ProfileService.Instance.Manager.UploadCurrentProfile();

        LocalNotificationsService.Current.ScheduleNotifications();
        
#if UNITY_EDITOR
        ProfileService.Instance.Manager.SaveLocalProfile();
#endif
    }
}