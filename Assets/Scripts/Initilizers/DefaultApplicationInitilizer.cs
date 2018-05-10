using System;
using System.Collections.Generic;
using UnityEngine;
using IW.Content.ContentModule;

public class DefaultApplicationInitilizer : ApplicationInitializer 
{
    public override void Init(Action onComplete)
    {
        // load configs
        LoadConfigsAndManagers();

        IWAssetBundleService.Instance.Manager.CacheLocalBundles( (_) => 
        {
            base.Init(onComplete);
        });
    }


    public virtual void LoadConfigsAndManagers()
    {
        // init all project components and managers
        
        // create and register ECS manager
        ECSSystemProcessor ecsSystemProcessor = new GameObject("_ECSProcessor").AddComponent<ECSSystemProcessor>();
        DontDestroyOnLoad(ecsSystemProcessor);
        ECSManager ecsManager = new ECSManager();
        ECSService.Instance.SetManager(ecsManager);
        ecsManager.AddSystemProcessor(ecsSystemProcessor);
        
        //init profile 
        var profileManager = new ProfileManager<UserProfile> { SystemVersion = 1 };
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
        
        // load local profile
        ProfileService.Instance.Manager.LoadCurrentProfile((profile) =>
        {
            new DefaultProfileBuilder().SetupComponents(profile);
            ProfileService.Instance.Manager.CheckMigration();
            
#if UNITY_EDITOR
            ProfileService.Instance.Manager.SaveLocalProfile();
#endif
            
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
        });
        
        // gamedata configs
        GameDataManager dataManager = new GameDataManager();
        GameDataService.Instance.SetManager(dataManager);
        
        dataManager.ChestsManager.LoadData(new ResourceConfigDataMapper<List<ChestDef>>("configs/chests.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.EnemiesManager.LoadData(new ResourceConfigDataMapper<List<EnemyDef>>("configs/enemies.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.HeroesManager.LoadData(new ResourceConfigDataMapper<List<HeroDef>>("configs/heroes.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.PiecesManager.LoadData(new ResourceConfigDataMapper<List<PieceDef>>("configs/pieces.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.ObstaclesManager.LoadData(new ResourceConfigDataMapper<List<ObstacleDef>>("configs/obstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.SimpleObstaclesManager.LoadData(new ResourceConfigDataMapper<List<SimpleObstaclesDef>>("configs/simpleObstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.QuestsManager.LoadData(new ResourceConfigDataMapper<List<QuestDef>>("configs/quests.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.FogsManager.LoadData(new ResourceConfigDataMapper<FogsDataManager>("configs/fogs.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.CollectionManager.LoadData(new ResourceConfigDataMapper<CollectionDataManager>("configs/collection.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.LevelsManager.LoadData(new ResourceConfigDataMapper<List<LevelsDef>>("configs/levels.data", NSConfigsSettings.Instance.IsUseEncryption));
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
#endif
    }

}
