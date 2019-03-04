using System.Collections.Generic;
using UnityEngine;

public class AudioServiceInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
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