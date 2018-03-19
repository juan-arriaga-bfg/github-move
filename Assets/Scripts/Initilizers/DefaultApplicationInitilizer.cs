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
        profileManager.Init(new StoragePlayerPrefsDataMapper<UserProfile>("user.profile"), new DefaultProfileBuilder(), new DefaultProfileMigration());
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
        shopManager.InitStorage(new ResourceConfigDataMapper<IEnumerable<ShopItem>>("configs/shopitems.data", NSConfigsSettings.Instance.IsUseEncryption),
        (shopItems) =>
        {
            Currency.RegisterCurrency(Currency.Coins);
            Currency.RegisterCurrency(Currency.Cash);
            Currency.RegisterCurrency(Currency.Crystals);
            
            Currency.RegisterCurrency(Currency.RobinCard);
            Currency.RegisterCurrency(Currency.JohnCard);
            
            Currency.RegisterCurrency(Currency.Enemy);
            Currency.RegisterCurrency(Currency.Chest);
            Currency.RegisterCurrency(Currency.PieceA1);
            Currency.RegisterCurrency(Currency.PieceA2);
            Currency.RegisterCurrency(Currency.PieceA3);
            Currency.RegisterCurrency(Currency.PieceB1);
            Currency.RegisterCurrency(Currency.PieceB2);
            Currency.RegisterCurrency(Currency.PieceB3);
            
            Currency.RegisterCurrency(Currency.LevelRobin);
            Currency.RegisterCurrency(Currency.LevelJohn);
            
            Currency.RegisterCurrency(Currency.Quest);
            
            Currency.RegisterCurrency(Currency.LevelCastle);
            Currency.RegisterCurrency(Currency.LevelTavern);
        });
        
        ShopService.Instance.SetManager(shopManager);
        
        // gamedata configs
        GameDataManager dataManager = new GameDataManager();
        GameDataService.Instance.SetManager(dataManager);
        
        dataManager.ChestsManager.LoadData(new ResourceConfigDataMapper<List<ChestDef>>("configs/chests.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.EnemiesManager.LoadData(new ResourceConfigDataMapper<List<EnemyDef>>("configs/enemies.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.HeroesManager.LoadData(new ResourceConfigDataMapper<List<HeroDef>>("configs/heroes.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.PiecesManager.LoadData(new ResourceConfigDataMapper<List<PieceDef>>("configs/pieces.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.ObstaclesManager.LoadData(new ResourceConfigDataMapper<List<ObstacleDef>>("configs/obstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
        
        // load local profile
        ProfileService.Instance.Manager.LoadCurrentProfile((profile) =>
        {
            ProfileService.Current.StorageItems.Clear();
            ProfileService.Instance.Manager.CheckMigration();

#if UNITY_EDITOR
            ProfileService.Instance.Manager.SaveLocalProfile();
#endif
            
            LocalizationManager localizationManager = new BaseLocalizationManager();
            LocalizationService.Instance.SetManager(localizationManager);
            localizationManager.SupportedLanguages = NSLocalizationSettings.Instance.SupportedLanguages;

            if (localizationManager.IsLanguageSupported(ProfileService.Current.UserSettings.Language))
            {
                localizationManager.SwitchLocalization(ProfileService.Current.UserSettings.Language);
            }
            else
            {
                ProfileService.Current.UserSettings.Language = SystemLanguage.English.ToString();
                localizationManager.SwitchLocalization(ProfileService.Current.UserSettings.Language);
            }
            
        });

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
