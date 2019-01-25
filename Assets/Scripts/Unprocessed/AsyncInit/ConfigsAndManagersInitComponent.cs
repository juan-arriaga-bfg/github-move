using System.Collections.Generic;
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
        
        // create and register ECS manager
        ECSSystemProcessor ecsSystemProcessor = new GameObject("_ECSProcessor").AddComponent<ECSSystemProcessor>();
        GameObject.DontDestroyOnLoad(ecsSystemProcessor);
        ECSManager ecsManager = new ECSManager();
        ECSService.Instance.SetManager(ecsManager);
        ecsManager.AddSystemProcessor(ecsSystemProcessor);
        
        //init profile 
        var profileManager = new ProfileManager<UserProfile> { SystemVersion = IWVersion.Get.BuildNumber };
#if UNITY_EDITOR
        profileManager.Init(new ResourceConfigDataMapper<UserProfile>("configs/profile.data", false), new DefaultProfileBuilder(), new DefaultProfileMigration());
#else
        profileManager.Init(new StoragePlayerPrefsDataMapper<UserProfile>("user.profile"), new DefaultProfileBuilder(), new DefaultProfileMigration());
#endif
        ProfileService.Instance.SetManager(profileManager);
        
        //init assetbundle
        IWAssetBundleManager assetBundleManager = new IWAssetBundleManager();
        IDataMapper<List<IWAssetBundleData>> assetBundleManagerDataMapper = new ResourceConfigDataMapper<List<IWAssetBundleData>>("iw/assetbundles.data", false);
        assetBundleManager.LoadData(assetBundleManagerDataMapper);
        IWAssetBundleService.Instance.SetManager(assetBundleManager);
        
        //init content manager
        ContentManager contentManager = new DefaultContentManager();
        IDataMapper<List<ContentData>> contentManagerDataMapper = new ResourceConfigDataMapper<List<ContentData>>("iw/content.data", false);
        contentManager.LoadData(contentManagerDataMapper);
        ContentService.Instance.SetManager(contentManager);
        
        //init iconservice
        IconResourceManager iconManager = new DefaultSpriteManager();
        IconService.Instance.SetManager(iconManager);
        iconManager.LoadData(new ResourceConfigDataMapper<List<IconData>>("iw/icons.data", false));
        
        //init shopmanager
        ShopManager shopManager = new ShopManager();
        shopManager.InitStorage(
            new ResourceConfigDataMapper<IEnumerable<ShopItem>>("configs/shopitems.data",
                NSConfigsSettings.Instance.IsUseEncryption),
            (shopItems) => { });
        
        ShopService.Instance.SetManager(shopManager);
        
        //init notificationmanager
        LocalNotificationsManager notifyManager = new LocalNotificationsManager();
        notifyManager.ClearNotifications();
        LocalNotificationsService.Instance.SetManager(notifyManager);
        
        // init audiomanager
        NSAudioManager audioManager = new DefaultAudioManager();
        NSAudioService.Instance.SetManager(audioManager);
        audioManager.LoadData(new ResourceConfigDataMapper<List<NSAudioData>>("iw/audio.data", false));
        AudioListener audioListener = new GameObject("AudioListener").AddComponent<AudioListener>();
        GameObject.DontDestroyOnLoad(audioListener);
        
        // load local base profile
        ProfileService.Instance.Manager.LoadBaseProfile((baseProfile) =>
        {
            // condition to reset profile
            if (baseProfile == null || profileManager.SystemVersion > baseProfile.SystemVersion)
            {
                if (baseProfile == null)
                {
                    Debug.LogWarning($"[Reset progress] reset progress by: baseProfile == null:{baseProfile == null}");
                }
                else
                {
                    Debug.LogWarning($"[Reset progress] profileManager.SystemVersion > baseProfile.SystemVersion:{profileManager.SystemVersion > baseProfile.SystemVersion}");

                }
                
                var profileBuilder = new DefaultProfileBuilder();
                ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());

                LoadConfigsAndManagersAfterProfile();
            }
            else
            {
                // load local profile
                ProfileService.Instance.Manager.LoadCurrentProfile((profile) =>
                {
                    if (profile == null)
                    {
                        Debug.LogWarning($"[Reset progress] reset progress by: profile == null:{profile == null}");

                        var profileBuilder = new DefaultProfileBuilder();
                        ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());
                    }
                    else
                    {
                        new DefaultProfileBuilder().SetupComponents(profile);
                        ProfileService.Instance.Manager.CheckMigration();
                    }
                    
                    LoadConfigsAndManagersAfterProfile();
                });
            }
        });

        // gamedata configs
        GameDataManager dataManager = new GameDataManager();
        GameDataService.Instance.SetManager(dataManager);
        
        dataManager.SetupComponents();
    }

    public void LoadConfigsAndManagersAfterProfile()
    {
        LocalizationManager localizationManager = new BaseLocalizationManager();
        LocalizationService.Instance.SetManager(localizationManager);
        localizationManager.SupportedLanguages = NSLocalizationSettings.Instance.SupportedLanguages;

        if (localizationManager.IsLanguageSupported(ProfileService.Current.Settings.Language))
        {
            localizationManager.SwitchLocalization(ProfileService.Current.Settings.Language);
        }
        else
        {
            ProfileService.Current.Settings.Language = SystemLanguage.English.ToString();
            localizationManager.SwitchLocalization(ProfileService.Current.Settings.Language);
        }

        isCompleted = true;
        OnComplete(this);
    }
}