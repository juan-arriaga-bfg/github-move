using System.Collections.Generic;
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
        
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
            
        var ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);
            
        foreach (var system in ecsSystems)
        {
            ECSService.Current.SystemProcessor.UnRegisterSystem(system);
        }
    }
    
    public void OnResetProgressClick()
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = "Reset the progress";
        model.Message = "Do you want to reset the progress and start playing from the beginning?";
        model.AcceptLabel = "<size=30>Reset progress!</size>";
        model.CancelLabel = "No!";
        
        model.OnAccept = () =>
        {
            ReloadScene(true);
        };
        
        model.OnCancel = () => {};
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
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
        var board = BoardService.Current.GetBoardById(0);

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
                
                var cell = board.RendererContext.CreateBoardElementAt<DebugCellView>(R.DebugCell, new BoardPosition(i, j, 20));
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

    public void OnDebug1Click()
    {
        Debug.Log("OnDebug1Click");


        GameDataService.Current.QuestsManager.StartNewDailyQuest();
        //var quest = GameDataService.Current.QuestsManager.StartQuestById("Daily", null);
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
        //
        // ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        // {
        //     Characters = new Dictionary<CharacterPosition, string>
        //     {
        //         {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
        //         {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
        //     }
        // };
        //
        // var json = JsonConvert.SerializeObject(charsList);
        // var charsListClone = JsonConvert.DeserializeObject<ConversationScenarioCharsListComponent>(json);
        //
        // var charsListClone2 = new ConversationScenarioCharsListComponent();
        // JToken.Parse(json).PopulateObject(charsListClone2);
        //
        // return;
        //
        var scenario = GameDataService.Current.ConversationsManager.BuildScenario("123");
        int i = 0;

        //BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.CreatePiece, PieceType.A1.Id);

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
        CurrencyHellper.Purchase(reward, success =>
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
}