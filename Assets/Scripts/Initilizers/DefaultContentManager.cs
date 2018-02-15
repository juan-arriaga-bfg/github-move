using IW.Content.ContentModule;
using UnityEngine;

public class DefaultContentManager : ContentManager 
{
    protected override Object TryGetObjectFromBundle(string objectName)
    {
        string[] splitedObjectName = objectName.Split(new string[]{"/"}, System.StringSplitOptions.RemoveEmptyEntries);

        objectName = splitedObjectName[splitedObjectName.Length - 1];

        AssetBundle resAssetBundle = IWAssetBundleService.Instance.Manager.GetBundleFromCache("res");

        if (resAssetBundle == null)
            return null;

        UnityEngine.Object targetObejct = resAssetBundle.LoadAsset<UnityEngine.Object>(objectName);

        return targetObejct;
    }
}
