using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class LocalAssetBundlesCacheInitComponent : AsyncInitComponentBase
{
    private const string KEY = "LocalBundlesCacheVersion";
    
    private IWBaseMonoBehaviour coroutinesExecutor;

    public override void Execute()
    {
        // Proxy GO to run coroutine from non-monobehaviour
        var go = new GameObject();
        coroutinesExecutor = go.AddComponent<IWBaseMonoBehaviour>();
        GameObject.DontDestroyOnLoad(go);

        coroutinesExecutor.StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        IWAssetBundleManager assetBundleManager = new IWAssetBundleManager();
        IDataMapper<List<IWAssetBundleData>> assetBundleManagerDataMapper = new ResourceConfigDataMapper<List<IWAssetBundleData>>("iw/assetbundles.data", false);
        assetBundleManager.LoadData(assetBundleManagerDataMapper);
        IWAssetBundleService.Instance.SetManager(assetBundleManager);
        
        while (!Caching.ready)
        {
            yield return null;
        }

        string curVersion = IWProjectVersionSettings.Instance.CurrentVersion;
        string cachedVersion = ObscuredPrefs.GetString(KEY, null);
        
#if UNITY_EDITOR
        bool isUnityEditor = true;
#else
        bool isUnityEditor = false;
#endif 

        if (string.IsNullOrEmpty(cachedVersion) || curVersion != cachedVersion || isUnityEditor)
        {
            Debug.Log($"[LocalAssetBundlesCacheInitComponent] => Execute: ClearCache. curVersion: {curVersion}, cachedVersion: {cachedVersion ?? "null"}, isUnityEditor: {isUnityEditor}");
            Caching.ClearCache();
        }

        Caching.ClearCache(); // FORCE CACHE CLEAN DUE to magic bugs
        
        IWAssetBundleService.Instance.Manager.CacheLocalBundles( (isOk) => 
        {
            var iconResourceManager = IconService.Current as IconResourceManager;
            iconResourceManager.GenerateTextSpriteAssets();

            GameObject.Destroy(coroutinesExecutor);

            ObscuredPrefs.SetString(KEY, curVersion);
            
            isCompleted = true;
            OnComplete(this);
        });
    }
}