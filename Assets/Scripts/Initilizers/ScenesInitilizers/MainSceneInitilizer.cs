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
        
        InitGameField();

        // cache windows
        IWUIManager.Instance.Init(new[]
        {
            UIWindowType.MainWindow,
            UIWindowType.SampleWindow,
            UIWindowType.MessageWindow,
            UIWindowType.ChestMessage,
            UIWindowType.QuestWindow,
            UIWindowType.ErrorWindow,
            UIWindowType.ChestsShopWindow,
            UIWindowType.EnergyShopWindow,
            UIWindowType.CodexWindow,
            UIWindowType.CurrencyCheatSheetWindow,
            UIWindowType.PiecesCheatSheetWindow,
            UIWindowType.ExchangeWindow,
            UIWindowType.OrdersWindow,
            UIWindowType.QuestStartWindow,
            UIWindowType.NextLevelWindow,
            UIWindowType.ConfirmRemoverMessageWindow,
            UIWindowType.DailyQuestWindow,
            UIWindowType.SuperMatchHintWindow,
            UIWindowType.ResourcePanelWindow,
        });
        
        // on cache complete
        IWUIManager.Instance.OnUIInited += () =>
        {
            // close launcher
            UIService.Get.CloseWindow(UIWindowType.LauncherWindow);
            
            //show resource panel 
            UIService.Get.ShowWindow(UIWindowType.ResourcePanelWindow);
            
            // get model for window
            UIService.Get.ShowWindow(UIWindowType.MainWindow);

            onComplete?.Invoke();
            
            ProfileService.Current.QueueComponent.Run();
            BoardService.Current.FirstBoard.TutorialLogic.Run();
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