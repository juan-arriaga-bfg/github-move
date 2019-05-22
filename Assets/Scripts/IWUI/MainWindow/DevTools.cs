using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [IWUIBinding("#PanelToggle")] private GameObject panelToggle;
    
    [IWUIBinding("#QuestDialogsToggle")] private Toggle questDialogsToggle;
    [IWUIBinding("#TutorialToggle")] private Toggle tutorialToggle;
    [IWUIBinding("#RemoverToggle")] private Toggle removerToggle;
    [IWUIBinding("#LoggerToggle")] private Toggle loggerToggle;
    [IWUIBinding("#IapToggle")] private Toggle iapToggle;
    [IWUIBinding("#SecureTimerToggle")] private Toggle secureTimerToggle;

    [IWUIBinding("#CommonButtons")] private GameObject commonPanel;
    [IWUIBinding("#ExtendedButtons")] private GameObject extendedPanel;

#if !UNITY_EDITOR
    private static bool isQuestDialogsDisabled = true;
    private static bool isTutorialDisabled = true;
    private static bool isRemoverDebugDisabled = true;
#endif
    
    private bool isEnabled = false;
    public static bool IsIdsEnabled = false;
    
    public override void OnViewInit(IWUIWindowView context)
    {
        base.OnViewInit(context);
        
        panel.SetActive(isEnabled);
        panelToggle.SetActive(isEnabled);
        
        questDialogsToggle.isOn = IsQuestDialogsEnabled();
        tutorialToggle.isOn = IsTutorialEnabled();
        removerToggle.isOn = IsRemoverDebugEnabled();
        loggerToggle.isOn = IsLoggerEnabled();
        iapToggle.isOn = IsIapEnabled();
        
        #if DEBUG
        secureTimerToggle.isOn = IsSecureTimerEnabled();
        #endif

        IW.Logger.IsEnabled = IsLoggerEnabled();
        
        UpdateFpsMeter();
        
        commonPanel.SetActive(true);
        extendedPanel.SetActive(false);
    }
    
    public void OnToggleValueChanged(bool isChecked)
    {
        isEnabled = !isChecked;
        panel.SetActive(isEnabled);
        panelToggle.SetActive(isEnabled);
    }

    public void OnExtendValueChanged(bool isChecked)
    {
        commonPanel.SetActive(isChecked);
        extendedPanel.SetActive(!isChecked);
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
    
    public void OnAbTestCheatSheetClick()
    {
        var model = UIService.Get.GetCachedModel<UIAbTestCheatSheetWindowModel>(UIWindowType.AbTestCheatSheetWindow);
        UIService.Get.ShowWindow(UIWindowType.AbTestCheatSheetWindow);
    }
    
    public void OnQuestsCheatSheetClick()
    {
        UIWaitWindowView.Show();
        
        var model = UIService.Get.GetCachedModel<UIQuestCheatSheetWindowModel>(UIWindowType.QuestCheatSheetWindow);
        model.Refresh();
        UIService.Get.ShowWindow(UIWindowType.QuestCheatSheetWindow);
    }
    
    private List<DebugTextView> cells = new List<DebugTextView>();
    
    public void OnToggleCells(bool isChecked)
    {
        var board = BoardService.Current.FirstBoard;

        if (isChecked)
        {
            foreach (var cell in cells)
            {
                board.RendererContext.DestroyElement(cell);
            }
            
            cells = new List<DebugTextView>();
            return;
        }
        
        for (var i = 0; i < board.BoardDef.Width; i++)
        {
            for (var j = 0; j < board.BoardDef.Height; j++)
            {
                //if(board.BoardLogic.IsLockedCell(new BoardPosition(i, j, 1))) continue;
                
                var cell = board.RendererContext.CreateBoardElementAt<DebugTextView>(R.DebugCell, new BoardPosition(i, j, BoardLayer.MAX.Layer));
                cell.SetIndex(i, j);
                cells.Add(cell);
            }
        }
    }

    public void OnToggleIds(bool isChecked)
    {
        IsIdsEnabled = !isChecked;
        
        var logic = BoardService.Current.FirstBoard.BoardLogic;
        var cache = new Dictionary<int, List<BoardPosition>>(logic.PositionsCache.Cache);

        cache.Remove(PieceType.Fog.Id);
        cache.Remove(PieceType.LockedEmpty.Id);

        foreach (var positions in cache.Values)
        {
            foreach (var position in positions)
            {
                var piece = logic.GetPieceAt(position);

                if (piece?.ActorView == null) continue;
                
                piece.ActorView.ShowId(IsIdsEnabled);
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
    
    public void OnSpawnAirShipClick()
    {
        Debug.Log("OnSpawnAirShipClick");
        
        var pieces1 = new Dictionary<int, int>
        {
            { PieceType.Parse("A1"), 1 },
            { PieceType.Parse("B3"), 1 },
            { PieceType.Parse("A2"), 1 },
            { PieceType.Parse("B5"), 1 },
        };
        
        BoardService.Current.FirstBoard.BoardLogic.AirShipLogic.Add(pieces1).AnimateSpawn();        
        return;
        
        var allIds = PieceType.GetAllIds();
        
        for (int i = allIds.Count - 1; i >=0 ; i--)
        {
            var id = allIds[i];
            var def = PieceType.GetDefById(id);
            if (def.Filter.Has(PieceTypeFilter.Fake)
            || def.Filter.Has(PieceTypeFilter.Multicellular)
            || def.Filter.Has(PieceTypeFilter.Mine)
            || def.Filter.Has(PieceTypeFilter.Obstacle)
            || def.Filter.Has(PieceTypeFilter.Character)
            || def.Filter.Has(PieceTypeFilter.ProductionField)                
            || (def.Abbreviations[0].Contains("NPC") && id > 2000501)                
                )
            {
                allIds.RemoveAt(i);
            }
        }

        for (int i = 0; i < 1000; i++)
        {
            int PIECES_COUNT = UnityEngine.Random.Range(1, 5);
            var pieces = new Dictionary<int, int>();
            for (int i1 = 0; i1 < PIECES_COUNT; i1++)
            {
                int index = UnityEngine.Random.Range(0, allIds.Count);

                int id = allIds[index];
                // var def = PieceType.GetDefById(id);
                
                if (i % 5 == 0)
                {
                    if (pieces.Count > 0)
                    {
                        pieces[pieces.Keys.First()]++;
                    }
                    else
                    {
                        pieces.Add(id, 1);
                    } 
                }
                else
                {
                    if (pieces.ContainsKey(id))
                    {
                        pieces[id]++;
                    }
                    else
                    {
                        pieces.Add(id, 1);
                    } 
                }

                if (i % 7 == 0)
                {
                    break;
                }
            }

            try
            {
                var view = BoardService.Current.FirstBoard.BoardLogic.AirShipLogic.Add(pieces);
                
                view.PlaceTo(new Vector2(17 + i * 3f, view.transform.position.y + 10));
                view.AnimateSpawn();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }        
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

        void RemovePiece()
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
               .InsertCallback(1, RemovePiece)
               .InsertCallback(2, LeakWatcherToggle.TakeSnapshot)
               .InsertCallback(3, CreatePiece)
               .InsertCallback(4, RemovePiece)
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
        new HighlightTaskPointToMarketButton().Highlight(null);
        new HighlightTaskPointToEnergyPlusButton().Highlight(null);
    }

    public void OnDebug2Click()
    {
        Debug.Log("OnDebug2Click");
        
        Analytics.SendQuestCompletedEvent("SomeQuestId");
        
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
        var currentExperience = ProfileService.Current.Purchases.GetStorageItem(Currency.Experience.Name).Amount;
        var priceExperience = GameDataService.Current.LevelsManager.Price;
        var currentLevel = GameDataService.Current.LevelsManager.Level;
        var levelDefs = GameDataService.Current.LevelsManager.Levels;
        
        if (levelDefs.Count == currentLevel)
        {
            UIErrorWindowController.AddError("Reached max level");
            return;
        }
        
        if (priceExperience - currentExperience == 1)
        {
            priceExperience = levelDefs[currentLevel].Price.Amount + priceExperience;
        }

        int expDiff = priceExperience - currentExperience - 1;

        if (expDiff < 0)
        {
            return;
        }
        
        CurrencyHelper.Purchase(Currency.Experience.Name, expDiff);
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
    
    public void OnIapValueChanged(bool isChecked)
    {
        
#if UNITY_EDITOR
        EditorPrefs.SetBool("DEBUG_IAP_DISABLED", isChecked);
#else
        ObscuredPrefs.SetBool("DEBUG_IAP_DISABLED", isChecked);
#endif
        
    }
    
    public static bool IsIapEnabled()
    {
        
#if UNITY_EDITOR
        return EditorPrefs.GetBool("DEBUG_IAP_DISABLED", true);
#else
        return ObscuredPrefs.GetBool("DEBUG_IAP_DISABLED", true);
#endif
        
    }
    
    public void OnLoggerValueChanged(bool isChecked)
    {
        
#if UNITY_EDITOR
        EditorPrefs.SetBool("DEBUG_LOGGER_DISABLED", isChecked);
#else
        ObscuredPrefs.SetBool("DEBUG_LOGGER_DISABLED", isChecked);
#endif
        IW.Logger.IsEnabled = isChecked;
    }
    
    public static bool IsLoggerEnabled()
    {
        
#if UNITY_EDITOR
        return EditorPrefs.GetBool("DEBUG_LOGGER_DISABLED", true);
#else
        return ObscuredPrefs.GetBool("DEBUG_LOGGER_DISABLED", true);
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

    public void OnSecureTimerValueChanged(bool isChecked)
    {
#if UNITY_EDITOR
        EditorPrefs.SetBool("DEBUG_SECURE_TIMER", isChecked);
        SecuredTimeService.Current.ForceUseStandardDateTime = !isChecked;
#elif DEBUG
        ObscuredPrefs.SetBool("DEBUG_SECURE_TIMER", isChecked);
        SecuredTimeService.Current.ForceUseStandardDateTime = !isChecked;
#endif
    }

    public bool IsSecureTimerEnabled()
    {
#if UNITY_EDITOR
        return EditorPrefs.GetBool("DEBUG_SECURE_TIMER", true);
#elif DEBUG
        return ObscuredPrefs.GetBool("DEBUG_SECURE_TIMER", true);
#endif
        return true;
    }
    
    /*{
        get
        {
#if UNITY_EDITOR
            return !EditorPrefs.GetBool("DEBUG_SECURE_TIMER", true);
#endif
            return !ObscuredPrefs.GetBool("DEBUG_SECURE_TIMER", true);
        }
    }*/

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

    public void OpenSystemDevParams()
    {
        var model = UIService.Get.GetCachedModel<UISystemDevParamsWindowWindowModel>(UIWindowType.SystemDevParamsWindowWindow);
        UIService.Get.ShowWindow(UIWindowType.SystemDevParamsWindowWindow);
    }

    [UsedImplicitly]
    public void OnReloadSceneClick()
    {
        ProfileService.Instance.Manager.UploadCurrentProfile(false);
        ReloadScene();
    }

    // todo: move this somewhere
    public static void ReloadScene()
    {
        IW.Logger.Log($"[DevTools] => ReloadScene");

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
            
             .AddComponent(new ServerSideConfigInitComponent()
                 .SetDependency(typeof(InternetMonitorInitComponent)))  
             
            .AddComponent(new ShopServiceInitComponent()
                .SetDependency(typeof(CleanupForReloadInitComponent)))
            
            .AddComponent(new ProfileInitComponent()
                .SetDependency(typeof(CleanupForReloadInitComponent))
                .SetDependency(typeof(ServerSideConfigInitComponent)))

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