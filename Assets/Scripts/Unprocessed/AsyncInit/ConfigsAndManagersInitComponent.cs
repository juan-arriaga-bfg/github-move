using System.Collections.Generic;
using BfgAnalytics;
using IW.Content.ContentModule;
using UnityEngine;

public class ConfigsAndManagersInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        LoadConfigsAndManagers();
    }

    public void LoadConfigsAndManagers()
    {
        //Time.timeScale = 0.25f;
        // init all project components and managers
                
        GameObject.DontDestroyOnLoad(UIService.Get);
        UIService.Get.DontDestroyPoolOnSceneChange = true;
        
        // create and register ECS manager
        ECSSystemProcessor ecsSystemProcessor = new GameObject("_ECSProcessor").AddComponent<ECSSystemProcessor>();
        GameObject.DontDestroyOnLoad(ecsSystemProcessor);
        ECSManager ecsManager = new ECSManager();
        ECSService.Instance.SetManager(ecsManager);
        ecsManager.AddSystemProcessor(ecsSystemProcessor);
        
        //init shopmanager
        ShopManager shopManager = new ShopManager();
        shopManager.InitStorage(
            new ResourceConfigDataMapper<IEnumerable<ShopItem>>("configs/shopitems.data",
                NSConfigsSettings.Instance.IsUseEncryption),
            (shopItems) => { });
        
        ShopService.Instance.SetManager(shopManager);
        
        //init notificationmanager
        BfgLocalNotificationsManagerBase notifyManager = new BfgLocalNotificationsManagerBase();
        notifyManager.CancelNotifications();
        LocalNotificationsService.Instance.SetManager(notifyManager);
        
        // init audiomanager
        NSAudioManager audioManager = new DefaultAudioManager();
        NSAudioService.Instance.SetManager(audioManager);
        audioManager.LoadData(new ResourceConfigDataMapper<List<NSAudioData>>("iw/audio.data", false));
        AudioListener audioListener = new GameObject("AudioListener").AddComponent<AudioListener>();
        GameObject.DontDestroyOnLoad(audioListener);
        audioManager.DontDestroyPoolOnSceneChange = true;
        
        // init animationdatamanager
        AnimationOverrideDataManager animationOverrideManager = new AnimationOverrideDataManager();
        AnimationOverrideDataService.Instance.SetManager(animationOverrideManager);
        
        isCompleted = true;
        OnComplete(this);
    }
}