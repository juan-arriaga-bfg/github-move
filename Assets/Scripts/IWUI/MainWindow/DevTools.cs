using System;
using System.Collections.Generic;
using BfgAnalytics;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Tayx.Graphy;
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
    [IWUIBinding("#RemoverToggle")] private Toggle removerToggle;

#if !UNITY_EDITOR
    private static bool isQuestDialogsDisabled = true;
    private static bool isTutorialDisabled = true;
    private static bool isRemoverDebugDisabled = true;
#endif
    
    public override void OnViewInit(IWUIWindowView context)
    {
        base.OnViewInit(context);
        
        panel.SetActive(false);
        
        questDialogsToggle.isOn = IsQuestDialogsEnabled();
        tutorialToggle.isOn = IsTutorialEnabled();
        removerToggle.isOn = IsRemoverDebugEnabled();
        
        UpdateFpsMeter();
    }
    
    public void OnToggleValueChanged(bool isChecked)
    {
        panel.SetActive(!isChecked);
    }
    
    public void OnProfilesClick()
    {
        UIService.Get.ShowWindow(UIWindowType.ProfileCheatSheetWindow);
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
    
    public void OnQuestsCheatSheetClick()
    {
        UIWaitWindowView.Show();
        
        var model = UIService.Get.GetCachedModel<UIQuestCheatSheetWindowModel>(UIWindowType.QuestCheatSheetWindow);
        model.Refresh();
        UIService.Get.ShowWindow(UIWindowType.QuestCheatSheetWindow);
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

    public void OnScheduleNotificationsClick()
    {
#if DEBUG
        LocalNotificationsService.Current.DebugSchedule();        
#endif
    }
    
    public void OnScheduleAllNotificationsClick()
    {
#if DEBUG
        LocalNotificationsService.Current.DebugScheduleAll();        
#endif
    }
    
    public void OnDebug1Click()
    {

        Debug.Log("OnDebug1Click");
        var model = UIService.Get.GetCachedModel<UICharacterUnlockedWindowModel>(UIWindowType.CharacterUnlockedWindow);
        model.CharacterId = UiCharacterData.CharSleepingBeauty;
        model.Rewards = new List<CurrencyPair>
        {
            new CurrencyPair {Currency = Currency.Coins.Name, Amount = 123},
            new CurrencyPair {Currency = Currency.Crystals.Name, Amount = 23}
        };
        UIService.Get.ShowWindow(UIWindowType.CharacterUnlockedWindow);
        return;

#if UNITY_EDITOR
        
        void CreatePiece()
        {
            BoardService.Current.FirstBoard.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = new BoardPosition(19, 13),
                PieceTypeId = PieceType.A1.Id
            });
        }

        void RemovePiee()
        {
            var pos = new BoardPosition(19, 13, 1);
        
            BoardService.Current.FirstBoard.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                Positions = new List<BoardPosition> {pos},
                To = pos,
                OnCompleteAction = null
            });
        }
        

        DOTween.Sequence()
               .InsertCallback(0, CreatePiece)
               .InsertCallback(1, RemovePiee)
               .InsertCallback(2, LeakWatcherToggle.TakeSnapshot)
               .InsertCallback(3, CreatePiece)
               .InsertCallback(4, RemovePiee)
               .InsertCallback(5, LeakWatcherToggle.CompareSnapshot);
                
        
        return;
#endif        
        
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
    public static void FastStartQuest(HashSet<string> questsToStart)
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
                HashSet<string> questsToStart = questManager.CheckConditions(out _);
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
        EditorPrefs.SetBool("DEBUG_QUEST_DIALOGS_DISABLED", isChecked);
#else
        isQuestDialogsDisabled = isChecked;
#endif
        
    }
    
    public static bool IsQuestDialogsEnabled()
    {
        
#if UNITY_EDITOR
        return EditorPrefs.GetBool("DEBUG_QUEST_DIALOGS_DISABLED", true);
#else
        return isQuestDialogsDisabled;
#endif
        
    }
    
    public void OnTutorialValueChanged(bool isChecked)
    {
        
#if UNITY_EDITOR
        EditorPrefs.SetBool("DEBUG_TUTORIAL_DISABLED", isChecked);
#else
        isTutorialDisabled = isChecked;
#endif
        
    }
    
    public static bool IsTutorialEnabled()
    {
        
#if UNITY_EDITOR
        return EditorPrefs.GetBool("DEBUG_TUTORIAL_DISABLED", true);
#else
        return isTutorialDisabled;
#endif
        
    }
    
    public void OnRemoverDebugValueChanged(bool isChecked)
    {
#if UNITY_EDITOR
        EditorPrefs.SetBool("DEBUG_REMOVER_DISABLED", isChecked);
#else
        isRemoverDebugDisabled = isChecked;
#endif
    }
    
    public static bool IsRemoverDebugEnabled()
    {
#if UNITY_EDITOR
        return EditorPrefs.GetBool("DEBUG_REMOVER_DISABLED", true);
#else
        return isRemoverDebugDisabled;
#endif
        
    }

    public static bool IsSequenceReset
    {
        get
        {
#if UNITY_EDITOR
            return EditorPrefs.GetBool("DEBUG_SEQUENCE_RESET", false);
#else
        return false;
#endif
        }
        set
        {
#if UNITY_EDITOR
            EditorPrefs.SetBool("DEBUG_SEQUENCE_RESET", value);
#endif
        }
    }

    [UsedImplicitly]
    public void OnReloadSceneClick()
    {
        ProfileService.Instance.Manager.UploadCurrentProfile();
        ReloadScene();
    }

    // todo: move this somewhere
    public static void ReloadScene()
    {
        AsyncInitService.Current
            .AddComponent(new ShowLoadingWindowInitComponent())
            .AddComponent(new ClosePermanentWindowsInitComponent())

            .AddComponent(new ReloadSceneLoaderComponent()
                .SetDependency(typeof(ShowLoadingWindowInitComponent))
                .SetDependency(typeof(ClosePermanentWindowsInitComponent)))

            .AddComponent(new CleanupForReloadInitComponent()
                .SetDependency(typeof(ReloadSceneLoaderComponent)))     
                         
            .AddComponent(new InternetMonitorInitComponent()
                .SetDependency(typeof(CleanupForReloadInitComponent)))
             
            .AddComponent(new ShopServiceInitComponent()
                .SetDependency(typeof(CleanupForReloadInitComponent)))
            
            .AddComponent(new ProfileInitComponent()
                .SetDependency(typeof(CleanupForReloadInitComponent)))
            
            .AddComponent(new GameDataInitComponent()
                .SetDependency(typeof(ProfileInitComponent)))
         
            .AddComponent(new NotificationServiceInitComponent()
                .SetDependency(typeof(ProfileInitComponent)))
            
            .AddComponent(new MainSceneLoaderComponent()
                .SetDependency(typeof(GameDataInitComponent)))

            .Run(null);
    }
    
    private void UpdateFpsMeter()
    {
        int index = ObscuredPrefs.GetInt("FPS_METER_MODE", 1);
        
        var fps = FindObjectOfType<GraphyManager>();
        if (fps == null)
        {
            return;
        }
        
        switch (index)
        {
            case 0:
                fps.FpsModuleState = GraphyManager.ModuleState.OFF;    
                fps.RamModuleState = GraphyManager.ModuleState.OFF;
                break;
            
            case 1:
                fps.FpsModuleState = GraphyManager.ModuleState.BASIC;    
                fps.RamModuleState = GraphyManager.ModuleState.OFF;
                break;
            
            case 2:
                fps.FpsModuleState = GraphyManager.ModuleState.FULL;    
                fps.RamModuleState = GraphyManager.ModuleState.OFF;
                break;
            
            case 3:
                fps.FpsModuleState = GraphyManager.ModuleState.FULL;    
                fps.RamModuleState = GraphyManager.ModuleState.BASIC;
                break;
        } 
    }
    
    public void OnFpsClick()
    {   
        int index = ObscuredPrefs.GetInt("FPS_METER_MODE", 1);
        
        index++;
        
        if (index > 3)
        {
            index = 0;
        }
        
        ObscuredPrefs.SetInt("FPS_METER_MODE", index);
        
        UpdateFpsMeter();
    }
}