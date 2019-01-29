using System;
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
           .AddComponent(new ConfigsAndManagersInitComponent()) 

           .AddComponent(new LocalBundlesInitComponent()
               .SetDependency(typeof(ConfigsAndManagersInitComponent)))
            
           .AddComponent(new ProfileInitComponent()  
                .SetDependency(typeof(ConfigsAndManagersInitComponent)))
            
           .AddComponent(new GameDataInitComponent()  
               .SetDependency(typeof(ProfileInitComponent)))
            
           .AddComponent(new LocalizationInitComponent()
               .SetDependency(typeof(GameDataInitComponent)))
            
           .AddComponent(new IapInitComponent()                                     // In-app purchases implementation
                .SetDependency(typeof(BfgSdkUnityMessageHandlerInitComponent))
                .SetDependency(typeof(LocalizationInitComponent)))
            
           .AddComponent(new IapRestoreInitComponent()                              // Handler for restored In-app purchases 
               .SetDependency(typeof(IapInitComponent)))
            
           .AddComponent(new ShowLoadingWindowInitComponent()
               .SetDependency(typeof(LocalBundlesInitComponent))
               .SetDependency(typeof(LocalizationInitComponent)));

        if (SceneManager.GetActiveScene().name != "Main") // Handle case when we start the game from the Main scene in the Editor.  
        {
            asyncInitManager.AddComponent(new UIInitProgressListenerComponent());
            
            asyncInitManager.AddComponent(new MainSceneLoaderComponent()
                .SetDependency(typeof(LocalBundlesInitComponent))
                .SetDependency(typeof(LocalizationInitComponent))
                .SetDependency(typeof(SecuredTimeServiceInitComponent)));
        }

        asyncInitManager.Run(onComplete);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        var energyLogic = BoardService.Current?.FirstBoard?.GetComponent<EnergyCurrencyLogicComponent>(EnergyCurrencyLogicComponent.ComponentGuid);
        if (pauseStatus)
        {
            if (ProfileService.Instance != null)
            {
                ProfileService.Instance.Manager.UploadCurrentProfile();

#if UNITY_EDITOR
            ProfileService.Instance.Manager.SaveLocalProfile();
            LocalNotificationsService.Current.ScheduleNotifications();
#endif
            }

            energyLogic?.Timer.Stop();
        }
        else
        {
            energyLogic?.InitInSave();
            
            LocalNotificationsService.Current.CancelNotifications();
        }
    }

    void OnApplicationQuit()
    {
        ProfileService.Instance.Manager.UploadCurrentProfile();

#if UNITY_EDITOR
        LocalNotificationsService.Current.ScheduleNotifications();
        
        ProfileService.Instance.Manager.SaveLocalProfile();
        AssetDatabase.Refresh();
#endif
    }
}