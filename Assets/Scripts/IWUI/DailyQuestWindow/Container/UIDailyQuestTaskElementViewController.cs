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
    
    [IWUIBinding("#BackNormal")] private Image backNormal;
    [IWUIBinding("#BackActive")] private Image backActive;
    [IWUIBinding("#BackCompleted")] private Image backCompleted;

    [IWUIBinding("#DoneLabel")] private GameObject doneLabel;
    [IWUIBinding("#Shine")] private GameObject shine;

    private const int LABEL_DESCRIPTION_STYLE_NORMAL = 15;
    private const int LABEL_REWARD_STYLE_NORMAL = 14;
    private const int LABEL_DESCRIPTION_STYLE_COMPLETED = 18;
    private const int LABEL_REWARD_STYLE_COMPLETED = 19;
    
    private TaskEntity task;
    private UIDailyQuestTaskElementEntity targetEntity;

    private List<BoardPosition> listOfPiecesToHighlight;

    private List<CurrencyPair> reward;

    public override void Init()
    {
        base.Init();
        
        targetEntity = entity as UIDailyQuestTaskElementEntity;

        task = targetEntity.Task;
        
        targetEntity.Task.OnChanged += OnTaskChanged;

        listOfPiecesToHighlight = null;

        reward = GetRewardFromComponent();
        
        UpdateUi();
    }

    private void OnTaskChanged(TaskEntity task)
    {
        if (task.State != TaskState.Claimed)
        {
            UpdateUi();
        }
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        if (targetEntity != null)
        {
            targetEntity.Task.OnChanged -= OnTaskChanged;
        }

        HighlightDroppedPieces();
        
        base.OnViewClose(context);
    }

    private void HighlightDroppedPieces()
    {
        if (listOfPiecesToHighlight == null)
        {
            return;
        }

        foreach (var pos in listOfPiecesToHighlight)
        {
//           var ray = GodRayView.Show(pos);
//           ray.Remove();
        }
    }

    public void UpdateUi()
    {
        taskButton.Init(task);

        taskIcon.sprite = UiQuestButton.GetIcon(task);

        lblProgress.Text = UiQuestButton.GetTaskProgress(task, 32);
        
        // lblDescription.Text = $"<color=#D2D2D2><size=25>[{task.Group} - {task.Id.ToLower()}]</size></color> " + task.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid)?.Message;

        string key = task.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid)?.Message;
        lblDescription.Text = LocalizationService.Get(key, key) ;

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
        backActive.gameObject.SetActive(false);
        backNormal.gameObject.SetActive(!task.IsCompleted());
        backCompleted.gameObject.SetActive(task.IsCompleted());
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
        
        if (task.IsCompletedOrClaimed())
        {
            if (!ProvideReward())
            {
                // No free space
                UIMessageWindowController.CreateMessage(
                    LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"),
                    LocalizationService.Get("common.title.error",      "common.title.error"));
                
                return;
            }
            
            if (task is TaskCompleteDailyTaskEntity)
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
                OnSuccess = (droppedPiecesPositions) => { listOfPiecesToHighlight = droppedPiecesPositions; } 
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
            taskIcon.transform.position);
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
            return;
        }

        canvasGroup.DOFade(alpha, 0.3f);
        taskIconCanvasGroup.DOFade(alpha, 0.3f);
    }
}