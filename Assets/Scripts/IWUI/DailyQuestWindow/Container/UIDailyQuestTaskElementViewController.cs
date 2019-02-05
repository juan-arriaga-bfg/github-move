using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyQuestTaskElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#TaskDescription")] private NSText lblDescription;
    [IWUIBinding("#TaskProgress")] private NSText lblProgress;
    [IWUIBinding("#TaskReward")] private NSText lblReward;
    [IWUIBinding("#TaskIcon")] private Image taskIcon;
    [IWUIBinding("#TaskIconAnchor")] private Transform taskIconAnchor;
    [IWUIBinding("#TaskIconBox")] private CanvasGroup taskIconCanvasGroup;
    [IWUIBinding("#TaskButton")] private DailyQuestWindowTaskButton taskButton;
    [IWUIBinding("#View")] private CanvasGroup canvasGroup;
    [IWUIBinding("#Mark")] private GameObject mark;
    
    [IWUIBinding("#Back")] private Image back;

    [IWUIBinding("#DoneLabel")] private GameObject doneLabel;
    [IWUIBinding("#Shine")] private GameObject shine;

    private const int LABEL_DESCRIPTION_STYLE_NORMAL = 15;
    private const int LABEL_REWARD_STYLE_NORMAL = 14;
    private const int LABEL_DESCRIPTION_STYLE_COMPLETED = 18;
    private const int LABEL_REWARD_STYLE_COMPLETED = 19;

    private readonly Color normalBackColor    = new Color(252 / 255f, 191 / 255f, 105 / 255f);
    private readonly Color activeBackColor    = new Color(255 / 255f, 230 / 255f, 185 / 255f);
    private readonly Color completedBackColor = new Color(180 / 255f, 201 / 255f, 17  / 255f);
    
    private TaskEntity task;
    public TaskEntity Task => task;

    private UIDailyQuestTaskElementEntity targetEntity;
    
    private int piecesAmount;
    private Dictionary<int, int> piecesReward;
    private List<CurrencyPair> currenciesReward;
    
    private Transform content;
    
    public override void Init()
    {
        base.Init();
        
        targetEntity = entity as UIDailyQuestTaskElementEntity;

        task = targetEntity.Task;
        
        targetEntity.Quest.OnChanged += OnQuestChanged;

        GetRewardFromComponent(out piecesReward, out currenciesReward);
        piecesAmount = piecesReward.Sum(pair => pair.Value);
        UpdateUi();
    }

    private void OnQuestChanged(QuestEntity quest, TaskEntity changedTask)
    {
        if (task == changedTask || (task is TaskCompleteDailyTaskEntity && changedTask != null))
        {
            UpdateUi();
        }
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        if (targetEntity != null)
        {
            targetEntity.Quest.OnChanged -= OnQuestChanged;
        }

        base.OnViewClose(context);
    }
    
    private void HighlightDroppedPieces(List<BoardPosition> listOfPiecesToHighlight)
    {
        if (listOfPiecesToHighlight == null)
        {
            return;
        }

        var board = BoardService.Current.FirstBoard;

        for (var i = 0; i < listOfPiecesToHighlight.Count; i++)
        {
            var pos = listOfPiecesToHighlight[i];
            var target = board.BoardLogic.GetPieceAt(pos);
            target.ActorView.ShowDropEffect(i == 0);
        }
    }

    public void UpdateUi()
    {
        taskButton.Init(task);
        
        if (content != null)
        {
            UIService.Get.PoolContainer.Return(content.gameObject);
            content = null;
        }
        
        content = UiQuestButton.GetIcon(task, taskIconAnchor, taskIcon);
        taskIcon.gameObject.SetActive(content == null);

        lblProgress.Text = UiQuestButton.GetTaskProgress(task, 32);

        lblDescription.Text = task.GetLocalizedMessage();

        lblReward.Text = GetRewardAsText();
        
        mark.SetActive(task.IsCompleted());
        
        shine.SetActive(task.IsCompleted());

        ToggleTextStyle();
        
        ToggleBack();

        ToggleActive(!task.IsClaimed(), false);
    }

    private void ToggleTextStyle()
    {
        if (task.IsCompleted())
        {
            lblDescription.StyleId = LABEL_DESCRIPTION_STYLE_COMPLETED;
            lblReward.StyleId      = LABEL_REWARD_STYLE_COMPLETED;
        }
        else
        {
            lblDescription.StyleId = LABEL_DESCRIPTION_STYLE_NORMAL;
            lblReward.StyleId      = LABEL_REWARD_STYLE_NORMAL;
        }

        lblDescription.ApplyStyle();
        lblReward.ApplyStyle();
    }

    private void ToggleBack()
    {
        back.color = task.IsCompleted() ? completedBackColor : normalBackColor;
    }

    private string GetRewardAsText()
    {
        if ((piecesReward == null || piecesReward.Count == 0) && (currenciesReward == null || currenciesReward.Count == 0))
        {
            return "";
        }
        
        if (task is TaskCompleteDailyTaskEntity)
        {
            return $"{LocalizationService.Get("window.daily.quest.todays.reward.text", "window.daily.quest.todays.reward.text")} <sprite name=pointLightProgressLine>";
        }
        
        var str = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), "");
        
        //var stringBuilder = new StringBuilder($"<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>{str}</color></font> <size=50>");
        var stringBuilder = new StringBuilder($"{str} ");
        
        stringBuilder.Append(CurrencyHelper.RewardsToString("  ", piecesReward, currenciesReward, task is TaskCompleteDailyTaskEntity));
        stringBuilder.Append("</size>");
            
        return stringBuilder.ToString();
    }

    private void GetRewardFromComponent(out Dictionary<int, int> piecesReward, out List<CurrencyPair> currenciesReward)
    {
        var reward = task.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value ?? new List<CurrencyPair>();
        var count = reward.Count;
        
        piecesReward = new Dictionary<int, int>();
        currenciesReward = new List<CurrencyPair>();
        
        if (count == 0)
        {
            Debug.LogError("[UIDailyQuestTaskElementViewController] => GetReward: No reward specified for 'Clear all' task!");
            return;
        }
        
        if (task is TaskCompleteDailyTaskEntity)
        {
            var globalIndex = GameDataService.Current.QuestsManager.DailyQuestRewardIndex;
            var index = globalIndex % count;
            reward = new List<CurrencyPair> {reward[index]};
        }
        
        piecesReward = CurrencyHelper.FiltrationRewards(reward, out currenciesReward);
    }

    public void OnClick()
    {
        if (task.IsClaimed())
        {
            return;
        }

        var isCurrentTaskClearAll = (task is TaskCompleteDailyTaskEntity);
        var isClaimClearAllAllowed = GameDataService.Current.QuestsManager.DailyQuest.IsAllTasksClaimed(true);

        if (isCurrentTaskClearAll && !isClaimClearAllAllowed)
        {
            ((UIDailyQuestWindowView) targetEntity.WindowController.WindowView).ScrollToFirstNotCompletedOrNotClaimedTask();
            return;
        }

        if (task.IsCompleted() == false)
        {
            targetEntity.WindowController.CloseCurrentWindow();
            task.Highlight();
            return;
        }
        
        var board = BoardService.Current.FirstBoard;
        
        if (board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceReward(piecesAmount, true, out var position) == false)
        {
            return;
        }
        
        taskButton.Disable();
        task.SetClaimedState();
        ToggleActive(false, true);
        
        CurrencyHelper.PurchaseAndProvideSpawn(piecesReward, currenciesReward, null, position, null, true, true);

        if (isCurrentTaskClearAll) targetEntity.WindowController.CloseCurrentWindow();
    }
    
    private void ToggleActive(bool enabled, bool animated)
    {
        ToggleBack();

        doneLabel.SetActive(!enabled);
        mark.gameObject.SetActive(enabled && task.IsCompleted());
        shine.gameObject.SetActive(enabled && task.IsCompleted());
        
        float alpha = enabled ? 1 : 0.4f;
        
        if (!animated)
        {
            canvasGroup.alpha = alpha;
            taskIconCanvasGroup.alpha = alpha;
            if (!enabled)
            {
                back.color = normalBackColor;
            }
            return;
        }

        DOTween.Kill(back, true);
        
        float TIME = 0.3f;
        
        canvasGroup.DOFade(alpha, TIME);
        taskIconCanvasGroup.DOFade(alpha, TIME);

        if (!enabled)
        {
            back.DOColor(normalBackColor, TIME);
        }
    }

    public void HighlightForHint()
    {       
        if (task.IsClaimed())
        {
            return;
        }

        if (task is TaskCompleteDailyTaskEntity)
        {
            return;
        }

        DOTween.Kill(back, true);

        var currentColor = task.IsCompleted() ? completedBackColor : normalBackColor;

        back.color = currentColor;

        var TIME = 0.3f;
        back.DOColor(activeBackColor, TIME)
            .SetId(back)
            .SetLoops(4, LoopType.Yoyo)
            .SetEase(Ease.OutSine)
            .OnComplete(() => { back.color = currentColor; });
    }
}