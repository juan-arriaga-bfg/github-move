using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestCheatSheetElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#QuestData")] private NSText lblData;
    [IWUIBinding("#TaskTexts")] private NSText lblTexts;
    [IWUIBinding("#TaskProgress")] private NSText lblProgress;
    [IWUIBinding("#TaskIcon")] private Image taskIcon;
    [IWUIBinding("#TaskIconAnchor")] private Transform taskIconAnchor;
    [IWUIBinding("#TaskIconBox")] private CanvasGroup taskIconCanvasGroup;
    // [IWUIBinding("#View")] private CanvasGroup canvasGroup;
    [IWUIBinding("#Mark")] private GameObject mark;
    
    [IWUIBinding("#Back")] private Image back;
    
    [IWUIBinding("#StateController")] private UIQuestCheatSheetStateController stateController;

    [IWUIBinding("#BtnDlgStart")] private UIButtonViewController btnDlgStart;
    [IWUIBinding("#BtnDlgEnd")]   private UIButtonViewController btnDlgEnd;
    [IWUIBinding("#BtnDescr")]    private UIButtonViewController btnDescr;
    [IWUIBinding("#BtnHint")]     private UIButtonViewController btnHint;
    
    private UIQuestCheatSheetWindowModel model;
    
    private TaskEntity targetTask;
    private QuestEntity targetQuest;

    private UIQuestCheatSheetElementEntity targetEntity;
    
    private int piecesAmount;
    private Dictionary<int, int> piecesReward;
    private List<CurrencyPair> currenciesReward;
    
    private Transform content;

    public override void Init()
    {
        base.Init();
        
        targetEntity = entity as UIQuestCheatSheetElementEntity;

        SetQuest();

        GameDataService.Current.QuestsManager.OnQuestStateChanged += OnQuestChanged;

        GetRewardFromComponent(out piecesReward, out currenciesReward);
        piecesAmount = piecesReward.Sum(pair => pair.Value);
        
        stateController.Init(targetQuest.Id);
        stateController.OnApplyChanges += OnApplyChanges;

        InitButtons();
        
        UpdateUi();
    }

    private void InitButtons()
    {
        btnDescr.OnClick(() =>
        {
            var model = UIService.Get.GetCachedModel<UIQuestWindowModel>(UIWindowType.QuestWindow);
            model.Quest = targetQuest;
            UIService.Get.ShowWindow(UIWindowType.QuestWindow);
        });
        
        btnHint.OnClick(() =>
        {
            targetQuest.Tasks[0].Highlight();
            targetEntity.WindowController.CloseCurrentWindow();
        });
        
        btnDlgStart.OnClick(() =>
        {
            stateController.SetInactive();
            
            var starter = GetStarterForQuest(targetQuest.Id);
            var model = UIService.Get.GetCachedModel<UIQuestStartWindowModel>(UIWindowType.QuestStartWindow);
            model.Init(null, starter.QuestToStartIds, starter.Id);

            UIService.Get.ShowWindow(UIWindowType.QuestStartWindow);
        });

        btnDlgEnd.OnClick(() =>
        {
            stateController.SetStarted();
            targetQuest.ForceComplete();

            QueueActionComponent queueItem;
            
            if (UiQuestButton.ShowCharUnlockedWindow(targetQuest))
            {
                queueItem = new QueueActionComponent()
                           .AddCondition(new WindowClosedQueueConditionComponent
                            {
                                Windows = new HashSet<string> {UIWindowType.CharacterUnlockedWindow}
                            })
                           .SetAction(() => { stateController.SetDone(); });
            }
            else
            {
                var model = UIService.Get.GetCachedModel<UIQuestStartWindowModel>(UIWindowType.QuestStartWindow);
                model.Init(targetQuest, null, null);
                model.TestMode = true;

                UIService.Get.ShowWindow(UIWindowType.QuestStartWindow);

                queueItem = new QueueActionComponent()
                           .AddCondition(new WindowClosedQueueConditionComponent
                            {
                                Windows = new HashSet<string> {UIWindowType.QuestStartWindow}
                            })
                           .SetAction(() => { stateController.SetDone(); });
            }
            

            ProfileService.Current.QueueComponent.AddAction(queueItem, false);
        });
    }

    private QuestStarterEntity GetStarterForQuest(string questId)
    {
        return GameDataService.Current.QuestsManager.QuestStarters.FirstOrDefault(e => e.QuestToStartIds.Contains(questId));
    }
    
    private void SetQuest()
    {
        targetQuest = targetEntity.Quest;
        targetTask = targetQuest.Tasks[0];
    }

    private void OnQuestChanged(QuestEntity quest, TaskEntity changedTask)
    {
        if (quest.Id == targetQuest.Id)
        {
            Debug.Log($"[UIQuestCheatSheetElementViewController] => Handle quest change {quest.Id}, new state: {quest.State}");

            ReInit();
        }
    }

    private void ReInit()
    {
        SetQuest();
        UpdateUi();
        stateController.UpdateUI();
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        if (targetEntity != null)
        {
            GameDataService.Current.QuestsManager.OnQuestStateChanged -= OnQuestChanged;
        }

        stateController.OnApplyChanges -= OnApplyChanges;
        
        base.OnViewClose(context);
    }

    private void OnApplyChanges()
    {
        ReInit();
    }

    public void UpdateUi()
    {
        if (content != null)
        {
            UIService.Get.PoolContainer.Return(content.gameObject);
            content = null;
        }
        
        content = UiQuestButton.GetIcon(targetTask, taskIconAnchor, null, taskIcon);
        taskIcon.gameObject.SetActive(content == null);

        var data = GetData();

        const string COLOR_ID     = "#FFFA1F";
        const string COLOR_TYPE   = "#FFFA1F";
        const string COLOR_TARGET = "#FFFA1F";
        const string COLOR_REWARD = "#FFFA1F";
        const string COLOR_TEXT   = "#FFFA1F";
        
        //ID <color=#fffA1f>1_PR_CREATE_Piece2</color> type <color=#fffA1f>CreatePiece</color> target <color=#fffA1f>A2</color> x1200
        
        // id
        StringBuilder dataStr = new StringBuilder($"ID {Colorize(targetQuest.Id, COLOR_ID)}");
        if (targetTask.Id != targetQuest.Id)
        {
            dataStr.Append($"/{Colorize(targetTask.Id, COLOR_ID)}");
        }

        // type
        string typeStr = targetTask.GetType().ToString().Replace("Task", "").Replace("Entity", "");
        dataStr.Append($"  type {Colorize(typeStr, COLOR_TYPE)}");
        
        // target
        dataStr.Append("  target ");
        if (!string.IsNullOrEmpty(data.PieceId))
        {
            dataStr.Append($"{Colorize(data.PieceId, COLOR_TARGET)}");
        }
        
        // Count
        if (targetTask is TaskCounterEntity)
        {
            dataStr.Append($" x {Colorize(((TaskCounterEntity)targetTask).TargetValue.ToString(), COLOR_TARGET)}");
        }
        // Reward
        if (!string.IsNullOrEmpty(data.QuestReward))
        {
            dataStr.Append($"  Reward {Colorize(data.QuestReward, COLOR_REWARD)}");
        }

        lblData.Text = dataStr.ToString();
        lblTexts.Text = $"T: {Colorize(data.TaskTitle, COLOR_TEXT)} M: {Colorize(data.TaskMessage, COLOR_TEXT)}";
        lblProgress.Text = data.TaskProgress;
        
        mark.SetActive(targetTask.IsCompleted());
    }

    private string Colorize(string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }
    
    private UIQuestCheatSheetElementData GetData()
    {
        string pieceId;
        
        switch (targetTask)
        {
            case TaskCurrencyEntity taskAboutCurrency:
                pieceId = taskAboutCurrency.CurrencyName;
                break;

            case IHavePieceId taskAboutPiece:
                pieceId = PieceType.Parse(taskAboutPiece.PieceId);
                break;
            
            default:
                pieceId = null;
                break;
        }

        var data = new UIQuestCheatSheetElementData
        {
            QuestId = targetQuest.Id,
            QuestReward = GetRewardAsText(),
            TaskId = targetTask.Id,
            TaskTitle = targetTask.GetLocalizedTitle(),
            TaskMessage = targetTask.GetLocalizedMessage(),
            TaskProgress = UiQuestButton.GetTaskProgress(targetTask, 32),
            QuestState = targetQuest.State,
            PieceId = pieceId
        };

        return data;
    }
    
    private string GetRewardAsText()
    {
        if ((piecesReward == null || piecesReward.Count == 0) && (currenciesReward == null || currenciesReward.Count == 0))
        {
            return "";
        }
        
        if (targetTask is TaskCompleteDailyTaskEntity)
        {
            return $"{LocalizationService.Get("window.daily.quest.todays.reward.text", "window.daily.quest.todays.reward.text")} <sprite name=pointLightProgressLine>";
        }
        
        // var str = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), "");
        //
        //var stringBuilder = new StringBuilder($"<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>{str}</color></font> <size=50>");
        // var stringBuilder = new StringBuilder($"{str} ");
        var stringBuilder = new StringBuilder();
        
        stringBuilder.Append(CurrencyHelper.RewardsToString("  ", piecesReward, currenciesReward, targetTask is TaskCompleteDailyTaskEntity));
        stringBuilder.Append("</size>");
            
        return stringBuilder.ToString();
    }

    private void GetRewardFromComponent(out Dictionary<int, int> piecesReward, out List<CurrencyPair> currenciesReward)
    {
        var reward = targetQuest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value ?? new List<CurrencyPair>();
        var count = reward.Count;
        
        piecesReward = new Dictionary<int, int>();
        currenciesReward = new List<CurrencyPair>();
        
        if (count == 0)
        {
            Debug.LogError("[UIDailyQuestTaskElementViewController] => GetReward: No reward specified for 'Clear all' task!");
            return;
        }
        
        if (targetTask is TaskCompleteDailyTaskEntity)
        {
            var globalIndex = GameDataService.Current.QuestsManager.DailyQuestRewardIndex;
            var index = globalIndex % count;
            reward = new List<CurrencyPair> {reward[index]};
        }
        
        piecesReward = CurrencyHelper.FiltrationRewards(reward, out currenciesReward);
    }
}