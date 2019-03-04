using UnityEngine;

public class EcsSystemProcessorInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        // create and register ECS manager
        ECSSystemProcessor ecsSystemProcessor = new GameObject("_ECSProcessor").AddComponent<ECSSystemProcessor>();
        GameObject.DontDestroyOnLoad(ecsSystemProcessor);
        ECSManager ecsManager = new ECSManager();
        ECSService.Instance.SetManager(ecsManager);
        ecsManager.AddSystemProcessor(ecsSystemProcessor);

        isCompleted = true;
        OnComplete(this);
    }
}