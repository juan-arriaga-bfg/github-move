using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneInitilizer : SceneInitializer<DefaultApplicationInitilizer>
{
    public static string[] WindowNames =>
        new[]
        {
            UIWindowType.MainWindow,
            UIWindowType.MessageWindow,
            UIWindowType.ChestMessage,
            UIWindowType.QuestWindow,
            UIWindowType.ErrorWindow,
            UIWindowType.CodexWindow,
            UIWindowType.ExchangeWindow,
            UIWindowType.OrdersWindow,
            UIWindowType.QuestStartWindow,
            UIWindowType.NextLevelWindow,
            UIWindowType.DailyQuestWindow,
            UIWindowType.ResourcePanelWindow,
            UIWindowType.SettingsWindow,
            UIWindowType.SoftShopWindow,
            UIWindowType.HardShopWindow,
            UIWindowType.MarketWindow,
            UIWindowType.ConfirmationWindow,
            UIWindowType.WaitWindow,
            UIWindowType.CreditsWindow,
            UIWindowType.EnergyShopWindow,
#if DEBUG
            UIWindowType.CurrencyCheatSheetWindow,
            UIWindowType.PiecesCheatSheetWindow,
            UIWindowType.QuestCheatSheetWindow,
#endif
        };
    
    protected override void InitScene(ApplicationInitializer applicationInitializer, Action onComplete)
    {
        base.InitScene(applicationInitializer, onComplete);

        // set resource deliverer for UI
        IWUISettings.Instance.SetResourceManager(new DefaultUIResourceManager());
        
        InitGameField();

        // Hot reload?
        if (IWUIManager.Instance.IsComplete)
        {
            StartCoroutine(ShowGameScene(onComplete, true));
            return;
        }
               
        // cache windows
        IWUIManager.Instance.Init(WindowNames);
        
        // on cache complete
        IWUIManager.Instance.OnUIInited += () =>
        {
            StartCoroutine(ShowGameScene(onComplete, false));
        };
    }

    private IEnumerator ShowGameScene(Action onComplete, bool hotReload)
    {
        var asyncInit = AsyncInitService.Current;
        while (!asyncInit.IsAllComponentsInited())
        {
             yield return new WaitForSeconds(0.1f);
        }
        
        // Just to refresh progressbar
        yield return new WaitForEndOfFrame();
        
        // close launcher
        UIService.Get.CloseWindow(UIWindowType.LauncherWindow);

        //show resource panel 
        UIService.Get.ShowWindow(UIWindowType.ResourcePanelWindow);

        // get model for window
        UIService.Get.ShowWindow(UIWindowType.MainWindow);
        
        IWUIManager.Instance.OnCloseQuery = UIMessageWindowController.CreateQuitMessage;

        onComplete?.Invoke();

        ProfileService.Current.QueueComponent.Run();
        BoardService.Current.FirstBoard.TutorialLogic.Run();

        if (!hotReload)
        {
            NSAudioService.Current.Play(SoundId.Ambient1Music, true, 1)
                          .SetVolume(0f)
                          .SetVolume(1f, 2f);
        }
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