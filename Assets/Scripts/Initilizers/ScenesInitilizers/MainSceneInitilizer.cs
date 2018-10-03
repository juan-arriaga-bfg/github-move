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
        
        InitGameField();

        // Should be called after BoardService initialization
        // Can't be created at DefaultApplicationinitializer because should be recreated after Reset Progress
        ConnectQuestManager();
        
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
        });
        
        // on cache complete
        IWUIManager.Instance.OnUIInited += () =>
        {
            // close launcher
            UIService.Get.CloseWindow(UIWindowType.LauncherWindow);
            
            // get model for window
            UIService.Get.ShowWindow(UIWindowType.MainWindow);

            onComplete?.Invoke();
        };
    }

    // Events subscribtion
    private void ConnectQuestManager()
    {
        // Quests and tasks
        QuestManager questManager = new QuestManager();
        QuestService.Instance.SetManager(questManager);
        
        questManager.Init();
        
        QuestService.Current.ConnectToBoard();
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