using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAudioManager : NSAudioManager 
{
    protected override AudioClip TryGetAudioClipFromBundle(NSAudioData audioData)
    {
        string objectName = audioData.ClipName;
        
        string[] splitedObjectName = objectName.Split(new string[]{"/"}, System.StringSplitOptions.RemoveEmptyEntries);

        objectName = splitedObjectName[splitedObjectName.Length - 1];

        AssetBundle resAssetBundle = IWAssetBundleService.Instance.Manager.GetBundleFromCache("res");

        if (resAssetBundle == null)
            return null;

        UnityEngine.Object targetObejct = resAssetBundle.LoadAsset<UnityEngine.Object>(objectName);

        return targetObejct as AudioClip;
    }
}
