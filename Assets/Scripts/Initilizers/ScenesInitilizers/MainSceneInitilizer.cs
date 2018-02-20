using System;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneInitilizer : SceneInitializer<DefaultApplicationInitilizer>
{
    protected override void InitScene(ApplicationInitializer applicationInitializer, Action onComplete)
    {
        base.InitScene(applicationInitializer, onComplete);

        Application.targetFrameRate = 60;

        // set resource deliverer for UI
        IWUISettings.Instance.SetResourceManager(new DefaultUIResourceManager());

        // cache windows
        IWUIManager.Instance.Init(new string[]
        {
            UIWindowType.MainWindow,
            UIWindowType.SampleWindow,
        });

        // on cache complete
        IWUIManager.Instance.OnUIInited += () =>
        {
            // close launcher
            UIService.Get.CloseWindow(UIWindowType.LauncherWindow);
            
            // get model for window
            var model = UIService.Get.GetCachedModel<UIMainWindowModel>(UIWindowType.MainWindow);
            UIService.Get.ShowWindow(UIWindowType.MainWindow);
            
            if (onComplete != null)
            {
                onComplete();
            }

            InitGameField();

        };
    }

    private void InitGameField()
    {
        var sandboxGameController = new GameObject().AddComponent<SandboxGameController>();
        sandboxGameController.Run();
    }
}