using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using Quests;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyQuestTaskElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#TaskDescription")] private NSText lblDescription;
    [IWUIBinding("#TaskProgress")] private NSText lblProgress;
    [IWUIBinding("#TaskReward")] private NSText lblReward;
    [IWUIBinding("#TaskIcon")] private Image taskIcon;
    [IWUIBinding("#TaskIcon")] private CanvasGroup taskIconCanvasGroup;
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

    private List<CurrencyPair> reward;

    public override void Init()
    {
        base.Init();
        
        targetEntity = entity as UIDailyQuestTaskElementEntity;

        task = targetEntity.Task;
        
        targetEntity.Quest.OnChanged += OnQuestChanged;

        reward = GetRewardFromComponent();
        
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

        taskIcon.sprite = UiQuestButton.GetIcon(task);

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
        if (reward == null || reward.Count == 0)
        {
            return "";
        }
        
        List<CurrencyPair> currencies;
        var pieces = CurrencyHellper.FiltrationRewards(reward, out currencies);
        
        if (task is TaskCompleteDailyTaskEntity)
        {
            var text = LocalizationService.Get("window.daily.quest.todays.reward.text", "window.daily.quest.todays.reward.text");
            text += " <sprite name=pointLightProgressLine>";
            return text;
        }
        
        var str = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), "");
        
        //var stringBuilder = new StringBuilder($"<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>{str}</color></font> <size=50>");
        var stringBuilder = new StringBuilder($"{str} ");    
        stringBuilder.Append(CurrencyHellper.RewardsToString("  ", pieces, currencies, task is TaskCompleteDailyTaskEntity));
        stringBuilder.Append("</size>");
            
        return stringBuilder.ToString();
    }

    private List<CurrencyPair> GetRewardFromComponent()
    {
        var reward = task.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value ?? new List<CurrencyPair>();
        int count = reward.Count;
        
        if (count == 0)
        {
            Debug.LogError("[UIDailyQuestTaskElementViewController] => GetReward: No reward specified for 'Clear all' task!");
            return reward;
        }
        
        if (task is TaskCompleteDailyTaskEntity)
        {
            int globalIndex = GameDataService.Current.QuestsManager.DailyQuestRewardIndex;
            int index = globalIndex % count;
            reward = new List<CurrencyPair> {reward[index]};
        }
        
        return reward;
    }

    public void OnClick()
    {
        if (task.IsClaimed())
        {
            return;
        }

        bool isCurrentTaskClearAll = (task is TaskCompleteDailyTaskEntity);
        bool isClaimClearAllAllowed = GameDataService.Current.QuestsManager.DailyQuest.IsAllTasksClaimed(true);

        if (isCurrentTaskClearAll && !isClaimClearAllAllowed)
        {
            ((UIDailyQuestWindowView) targetEntity.WindowController.WindowView).ScrollToFirstNotCompletedOrNotClaimedTask();
            return;
        }
        
        if (task.IsCompleted())
        {
            if (!ProvideReward())
            {
                // No free space
                UIMessageWindowController.CreateMessage(
                    LocalizationService.Get("window.daily.error.title", "window.daily.error.title"),
                    LocalizationService.Get("window.daily.error.free.space", "window.daily.error.free.space"));
                
                return;
            }
            
            if (isCurrentTaskClearAll || GameDataService.Current.QuestsManager.DailyQuest.GetCompletedButNotClaimedTasksCount() == 0)
            {
                targetEntity.WindowController.CloseCurrentWindow();
            }
 
            return;
        }
        
        targetEntity.WindowController.CloseCurrentWindow();
        task.Highlight();
    }

    private bool ProvideReward(Action onComplete = null)
    {
        taskButton.Disable();
        
        task.SetClaimedState();

        ToggleActive(false, true);
        
        var board = BoardService.Current.FirstBoard;
        BoardPosition npcPos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
        
        // Reward to drop to field
        List<CurrencyPair> currencies;
        Dictionary<int, int> pieces = CurrencyHellper.FiltrationRewards(reward, out currencies);

        var amount = pieces.Sum(pair => pair.Value);
        
        if (!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(npcPos, amount))
        {
            UIErrorWindowController.AddError(LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"));
            return false;
        }
        
        Action dropReward = () =>
        {
            DOTween.Sequence()// Delay to hide window
                   .InsertCallback(0.5f, () =>
                    {
                        board.ActionExecutor.AddAction(new EjectionPieceAction
                        {
                            From = npcPos,
                            Pieces = pieces,
                            OnComplete = () =>
                            {
                                var view = board.RendererContext.GetElementAt(npcPos) as CharacterPieceView;

                                if (view != null) view.StartRewardAnimation();

                                AddResourceView.Show(npcPos, currencies);

                                onComplete?.Invoke();
                            },
                            OnSuccess = (droppedPiecesPositions) => { HighlightDroppedPieces(droppedPiecesPositions); }
                        });
                    });
        };

        // Provide all reward
        if (currencies.Count > 0)
        {
            CurrencyHellper.Purchase(currencies, success =>
            {
                if (pieces.Count > 0)
                {
                    dropReward();
                }
                else
                {
                    onComplete?.Invoke();
                }
            },
            targetEntity.WindowController.Window.Layers[0].ViewCamera.WorldToScreenPoint(taskIcon.transform.position));
        }
        else
        {
            dropReward(); 
        }

        return true;
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

        float TIME = 0.3f;
        back.DOColor(activeBackColor, TIME)
                  .SetId(back)
                  .SetLoops(4, LoopType.Yoyo)
                  .SetEase(Ease.OutSine)
                  .OnComplete(() =>
                   {
                       back.color = currentColor;
                   });
    }
}