using System;

public class TestFontStylesSceneInitializer : SceneInitializer<TestApplicationInitializer>
{
    protected override void InitScene(ApplicationInitializer applicationInitializer, Action onComplete)
    {
        base.InitScene(applicationInitializer, onComplete);

        // set resource deliverer for UI
        IWUISettings.Instance.SetResourceManager(new DefaultUIResourceManager());
        
        FontStylesTester.Instance?.Init();
    }
}
