using UnityEngine;

public class LocalBundlesInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        Caching.ClearCache();
        
        IWAssetBundleService.Instance.Manager.CacheLocalBundles( (_) => 
        {
            var iconResourceManager = IconService.Current as IconResourceManager;
            iconResourceManager.GenerateTextSpriteAssets();

            isCompleted = true;
            OnComplete(this);
        });
    }
}