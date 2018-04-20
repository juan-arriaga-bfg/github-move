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
            
            Currency.RegisterCurrency(Currency.Enemy);
            
            Currency.RegisterCurrency(Currency.Quest);
            Currency.RegisterCurrency(Currency.Obstacle);
            Currency.RegisterCurrency(Currency.Power);
            Currency.RegisterCurrency(Currency.Merge);
            
            Currency.RegisterCurrency(Currency.Chest1);
            Currency.RegisterCurrency(Currency.Chest2);
            Currency.RegisterCurrency(Currency.Chest3);
            Currency.RegisterCurrency(Currency.Chest4);
            Currency.RegisterCurrency(Currency.Chest5);
            Currency.RegisterCurrency(Currency.Chest6);
            Currency.RegisterCurrency(Currency.Chest7);
            Currency.RegisterCurrency(Currency.Chest8);
            Currency.RegisterCurrency(Currency.Chest9);
            
            Currency.RegisterCurrency(Currency.PieceA1);
            Currency.RegisterCurrency(Currency.PieceA2);
            Currency.RegisterCurrency(Currency.PieceA3);
            Currency.RegisterCurrency(Currency.PieceA4);
            Currency.RegisterCurrency(Currency.PieceA5);
            Currency.RegisterCurrency(Currency.PieceA6);
            Currency.RegisterCurrency(Currency.PieceA7);
            Currency.RegisterCurrency(Currency.PieceA8);
            Currency.RegisterCurrency(Currency.PieceA9);
            
            Currency.RegisterCurrency(Currency.PieceB1);
            Currency.RegisterCurrency(Currency.PieceB2);
            Currency.RegisterCurrency(Currency.PieceB3);
            Currency.RegisterCurrency(Currency.PieceB4);
            Currency.RegisterCurrency(Currency.PieceB5);
            
            Currency.RegisterCurrency(Currency.PieceC1);
            Currency.RegisterCurrency(Currency.PieceC2);
            Currency.RegisterCurrency(Currency.PieceC3);
            Currency.RegisterCurrency(Currency.PieceC4);
            Currency.RegisterCurrency(Currency.PieceC5);
            Currency.RegisterCurrency(Currency.PieceC6);
            Currency.RegisterCurrency(Currency.PieceC7);
            Currency.RegisterCurrency(Currency.PieceC8);
            Currency.RegisterCurrency(Currency.PieceC9);
            
            Currency.RegisterCurrency(Currency.PieceD1);
            Currency.RegisterCurrency(Currency.PieceD2);
            Currency.RegisterCurrency(Currency.PieceD3);
            Currency.RegisterCurrency(Currency.PieceD4);
            Currency.RegisterCurrency(Currency.PieceD5);
            
            Currency.RegisterCurrency(Currency.PieceE1);
            Currency.RegisterCurrency(Currency.PieceE2);
            Currency.RegisterCurrency(Currency.PieceE3);
            Currency.RegisterCurrency(Currency.PieceE4);
            Currency.RegisterCurrency(Currency.PieceE5);
            Currency.RegisterCurrency(Currency.PieceE6);
            
            Currency.RegisterCurrency(Currency.LevelRobin);
            Currency.RegisterCurrency(Currency.LevelJohn);
            
            Currency.RegisterCurrency(Currency.LevelCastle);
            Currency.RegisterCurrency(Currency.LevelTavern);
            Currency.RegisterCurrency(Currency.LevelMine);
            Currency.RegisterCurrency(Currency.LevelSawmill);
            Currency.RegisterCurrency(Currency.LevelSheepfold);
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
        dataManager.SimpleObstaclesManager.LoadData(new ResourceConfigDataMapper<List<SimpleObstaclesDef>>("configs/simpleObstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.QuestsManager.LoadData(new ResourceConfigDataMapper<List<QuestDef>>("configs/quests.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.FogsManager.LoadData(new ResourceConfigDataMapper<List<FogDef>>("configs/fogs.data", NSConfigsSettings.Instance.IsUseEncryption));
        
        // load local profile
        ProfileService.Instance.Manager.LoadCurrentProfile((profile) =>
        {
            ProfileService.Current.StorageItems.Clear();
            ProfileService.Instance.Manager.CheckMigration();

            // set start power
            profile.GetStorageItem(Currency.Power.Name).Amount += 15;
            
            // set start money
            profile.GetStorageItem(Currency.Coins.Name).Amount += 500;
            profile.GetStorageItem(Currency.Crystals.Name).Amount += 50;

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
