using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class DevTools : UIContainerElementViewController
{
    [IWUIBinding("#ButtonsPanel")] private GameObject panel;
    [IWUIBinding("#QuestDialogsToggle")] private Toggle questDialogsToggle;
    [IWUIBinding("#TutorialToggle")] private Toggle tutorialToggle;

#if !UNITY_EDITOR
    private static bool isQuestDialogsDisabled;
    private static bool isTutorialDisabled;
#endif
    
    public override void OnViewInit(IWUIWindowView context)
    {
        base.OnViewInit(context);
        
        panel.SetActive(false);
        
        questDialogsToggle.isOn = IsQuestDialogsEnabled();
        tutorialToggle.isOn = IsTutorialEnabled();
    }
    
    public void OnToggleValueChanged(bool isChecked)
    {
        panel.SetActive(!isChecked);
    }
    
    public static void ReloadScene(bool resetProgress)
    {
        var manager = GameDataService.Current.QuestsManager;
        manager.DisconnectFromBoard();
            
        BoardService.Instance.SetManager(null);

        if (resetProgress)
        {
            var profileBuilder = new DefaultProfileBuilder();
            ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());
        }

        GameDataService.Current.Reload();
                   
        var ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);
            
        foreach (var system in ecsSystems)
        {
            ECSService.Current.SystemProcessor.UnRegisterSystem(system);
        }
        
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    
    public void OnResetProgressClick()
    {
        UIMessageWindowController.CreateMessageWithTwoButtons(
            "Reset the progress",
            "Do you want to reset the progress and start playing from the beginning?",
            "<size=30>Reset progress!</size>",
            "No!",
            () => { ReloadScene(true); },
            () => {});
    }

    public void OnCurrencyCheatSheetClick()
    {
        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.CurrencyCheatSheetWindow);
        UIService.Get.ShowWindow(UIWindowType.CurrencyCheatSheetWindow);
    }
    
    public void OnPiecesCheatSheetClick()
    {
        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.PiecesCheatSheetWindow);
        UIService.Get.ShowWindow(UIWindowType.PiecesCheatSheetWindow);
    }
    
    private List<DebugCellView> cells = new List<DebugCellView>();
    
    public void OnToggleCells(bool isChecked)
    {
        var board = BoardService.Current.FirstBoard;

        if (isChecked)
        {
            foreach (var cell in cells)
            {
                board.RendererContext.DestroyElement(cell);
            }
            
            cells = new List<DebugCellView>();
            return;
        }
        
        for (var i = 0; i < board.BoardDef.Width; i++)
        {
            for (var j = 0; j < board.BoardDef.Height; j++)
            {
                //if(board.BoardLogic.IsLockedCell(new BoardPosition(i, j, 1))) continue;
                
                var cell = board.RendererContext.CreateBoardElementAt<DebugCellView>(R.DebugCell, new BoardPosition(i, j, BoardLayer.MAX.Layer));
                cell.SetIndex(i, j);
                cells.Add(cell);
            }
        }
    }

    public void OnCompleteFirstQuestClick()
    {
        var manager = GameDataService.Current.QuestsManager;
        if (manager.ActiveStoryQuests.Count == 0)
        {
            return;
        }

        var quest = manager.ActiveStoryQuests[0];
        quest.ForceComplete();
    }
    
    public void OnSpawnFireFlyClick()
    {
        Debug.Log("OnSpawnFireFlyClick");
        BoardService.Current.FirstBoard.BoardLogic.FireflyLogic.Execute();
    }

    private void ShowQuestWindow(List<QuestEntity> quests, int index)
    {
        if (quests[index].Id.ToLower().Contains("daily"))
        {
            return;
        }
        
        var model = UIService.Get.GetCachedModel<UIQuestWindowModel>(UIWindowType.QuestWindow);
        model.Quest = quests[index];
        model.Quest.Start(null);
        
        var nextIndex = index + 1;
        
        if (index < quests.Count)
        {
            model.OnClosed = () =>
            {
                ShowQuestWindow(quests, nextIndex);
            };
        }
    
        UIService.Get.ShowWindow(UIWindowType.QuestWindow);
    }

    private void ShowAllStoryQuestsWindows()
    {
        var manager = GameDataService.Current.QuestsManager;
        
        Dictionary<string, JToken> configs = manager.Cache[typeof(QuestEntity)];

        var quests = new List<QuestEntity>();
            
        foreach (var config in configs)
        {
            QuestEntity obj = manager.InstantiateFromJson<QuestEntity>(config.Value);
            var id = obj.Id;
            if (id == "13_CreatePiece_A6")
            {
                continue;
            }
            
            quests.Add(manager.InstantiateQuest(id));
        }

        ShowQuestWindow(quests, 0);
    }

    public void ShowNotifications()
    {
#if DEBUG
        var notificationManager = LocalNotificationsService.Current as BfgLocalNotificationsManagerBase;
        notificationManager?.DebugSchedule();        
#endif
    }
    
    public void OnDebug1Click()
    {
        Debug.Log("OnDebug1Click");

        
        LocalNotificationsService.Current.PushNotify(new Notification(100, "Test title", "Test Message", DateTime.UtcNow.AddSeconds(30)));
        // var codexManager = GameDataService.Current.CodexManager;
        // codexManager.ClearCodexContentCache();
        return;
        
        ShowAllStoryQuestsWindows();
        return;
        
        // CurrencyHellper.Purchase(Currency.Experience.Name, 2000);
        // return;
        // GameDataService.Current.QuestsManager.StartNewDailyQuest();
        //var quest = GameDataService.Current.QuestsManager.StartQuestById("Daily", null);

        new HighlightTaskPointToOrdersButton().Highlight(null);
        new HighlightTaskPointToShopButton().Highlight(null);
        new HighlightTaskPointToEnergyPlusButton().Highlight(null);
    }

    public static void UpdateFogSectorsDebug()    
    {
        var view = new FogSectorsView();
        view.Init(BoardService.Current.FirstBoard.RendererContext);
        view.UpdateFogSectorsMesh();
    }

    public void OnDebug2Click()
    {
        Debug.Log("OnDebug2Click");

        GameDataService.Current.QuestsManager.StartNewDailyQuest();
        return;
        
        const string QUEST_ID = "1";
        
        var quest = GameDataService.Current.QuestsManager.GetActiveQuestById(QUEST_ID);
        quest.ActiveTasks[0].Highlight();
        return;
        //
        // var board = BoardService.Current.FirstBoard;
        // var positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceTypeFilter.Simple, 1);
        //
        // foreach (var pos in positions)
        // {
        //     var ray = GodRayView.Show(pos);
        //     ray.Remove();
        // }

#if LEAKWATCHER
        GC.Collect();
        GC.WaitForPendingFinalizers();
    
        Debug.Log(LeakWatcher.Instance.DataAsString(false));
#endif

        // QuestService.Current.Load();
        // BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.Match, null);

        // string text = File.ReadAllText(@"D:/save.json");
        // QuestSaveComponent q = JsonConvert.DeserializeObject<QuestSaveComponent>(text);
        //
        // string i = "";
    }

    public void OnAddExpClick()
    {
        int exp = Input.GetKey(KeyCode.LeftShift) ? 100 : 1000;
        CurrencyHelper.Purchase(Currency.Experience.Name, exp);
    }

    /// <summary>
    /// Start quest without conversations. DEBUG use only!
    /// </summary>
    public static void FastStartQuest(List<string> questsToStart)
    {
        var questManager = GameDataService.Current.QuestsManager;
        questManager.StartQuests(questsToStart);
    }
    
    /// <summary>
    /// Complete quest without conversations. DEBUG use only!
    /// </summary>
    public static void FastCompleteQuest(QuestEntity questToFinish)
    {
        var questManager = GameDataService.Current.QuestsManager;
        
        questToFinish.SetClaimedState();
        questManager.FinishQuest(questToFinish.Id);

        List<CurrencyPair> reward = questToFinish.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;
        CurrencyHelper.Purchase(reward, success =>
            {
                string starterId;
                List<string> questsToStart = questManager.CheckConditions(out starterId);
                questManager.StartQuests(questsToStart);
            },
            new Vector2(Screen.width / 2, Screen.height / 2)
        );
    }
    
#if DEBUG
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnDebug1Click();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnDebug2Click();
            }
        }
    }
#endif

    public void OnQuestDialogsValueChanged(bool isChecked)
    {
        
#if UNITY_EDITOR
        EditorPrefs.SetBool("DEBUG_QUEST_DIALOGS_DISABLED", !isChecked);
#else
        isQuestDialogsDisabled = !isChecked;
#endif
        
    }
    
    public static bool IsQuestDialogsEnabled()
    {
        
#if UNITY_EDITOR
        return !EditorPrefs.GetBool("DEBUG_QUEST_DIALOGS_DISABLED", false);
#else
        return !isQuestDialogsDisabled;
#endif
        
    }
    
    public void OnTutorialValueChanged(bool isChecked)
    {
        
#if UNITY_EDITOR
        EditorPrefs.SetBool("DEBUG_TUTORIAL_DISABLED", !isChecked);
#else
        isTutorialDisabled = !isChecked;
#endif
        
    }
    
    public static bool IsTutorialEnabled()
    {
        
#if UNITY_EDITOR
        return !EditorPrefs.GetBool("DEBUG_TUTORIAL_DISABLED", false);
#else
        return !isTutorialDisabled;
#endif
        
    }

    [UsedImplicitly]
    public void OnReloadSceneClick()
    {
        // var mainWindowGo = UIService.Get.Get(UIWindowType.LauncherWindow).gameObject;
        
        UIService.Get.CloseWindow(UIWindowType.MainWindow);
        UIService.Get.CloseWindow(UIWindowType.ResourcePanelWindow);

        // var findGo = GameObject.Find("UIContainer");
        // var refGo = IWUIManager.Instance.gameObject;
        // DontDestroyOnLoad(IWUIManager.Instance.transform.parent);
        //
        // if (!ReferenceEquals(findGo, refGo))
        // {
        //     Debug.LogError("SUKA!!!!!!!");
        // }
        // else
        // {
        //     Debug.LogError("OK!!!!!!!");
        // }

        AsyncInitService.Current
            .AddComponent(new ShowLoadingWindowInitComponent())

            .AddComponent(new ReloadSceneLoaderComponent()
                .SetDependency(typeof(ShowLoadingWindowInitComponent)))

            .AddComponent(new CleanupForReloadInitComponent()
                .SetDependency(typeof(ReloadSceneLoaderComponent)))            
                        
            .AddComponent(new ProfileInitComponent()
                .SetDependency(typeof(CleanupForReloadInitComponent)))

            .AddComponent(new GameDataInitComponent()
                .SetDependency(typeof(ProfileInitComponent)))

            .AddComponent(new MainSceneLoaderComponent()
                .SetDependency(typeof(GameDataInitComponent)))

            .Run(null);

        // Undo dont destroy on load
        // SceneManager.MoveGameObjectToScene(mainWindowGo, SceneManager.GetActiveScene());

        // SceneManager.LoadScene("Reload", LoadSceneMode.Single);
        // return;

        // IEnumerator LoadSceneCoroutine(Action onComplete)
        // {
        //     var loadingOperation = SceneManager.LoadSceneAsync("Reload", LoadSceneMode.Single);
        //
        //     // Wait until the asynchronous scene fully loads
        //     while (!loadingOperation.isDone)
        //     {
        //         yield return null;
        //     }
        //
        //     onComplete?.Invoke();
        // }
        //
        // StartCoroutine(LoadSceneCoroutine(null));
    }
}