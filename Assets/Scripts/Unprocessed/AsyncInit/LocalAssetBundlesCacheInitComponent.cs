using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class LocalAssetBundlesCacheInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        // todo: why is it used here?
        Caching.ClearCache();
        
        IWAssetBundleManager assetBundleManager = new IWAssetBundleManager();
        IDataMapper<List<IWAssetBundleData>> assetBundleManagerDataMapper = new ResourceConfigDataMapper<List<IWAssetBundleData>>("iw/assetbundles.data", false);
        assetBundleManager.LoadData(assetBundleManagerDataMapper);
        IWAssetBundleService.Instance.SetManager(assetBundleManager);
        
        IWAssetBundleService.Instance.Manager.CacheLocalBundles( (isOk) => 
        {
            var iconResourceManager = IconService.Current as IconResourceManager;
            iconResourceManager.GenerateTextSpriteAssets();

            isCompleted = true;
        });
    }
}