﻿using System;
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
            UIWindowType.MessageWindow,
            UIWindowType.QuestStartWindow,
            UIWindowType.SimpleQuestStartWindow,
            UIWindowType.ChestMessage,
            UIWindowType.HeroesWindow,
            UIWindowType.CollectionWindow,
        });

        // on cache complete
        IWUIManager.Instance.OnUIInited += () =>
        {
            // close launcher
            UIService.Get.CloseWindow(UIWindowType.LauncherWindow);
            
            InitGameField();
            
            // get model for window
            var model = UIService.Get.GetCachedModel<UIMainWindowModel>(UIWindowType.MainWindow);
            UIService.Get.ShowWindow(UIWindowType.MainWindow);
            
            if (onComplete != null)
            {
                onComplete();
            }
        };
    }

    private void InitGameField()
    {
        // create manager
        BoardService.Instance.SetManager(new BoardManager());
        
        var sandboxGameController = new GameObject().AddComponent<SandboxGameController>();
        sandboxGameController.Run();
        sandboxGameController.name = "SandboxGameController";
    }
}