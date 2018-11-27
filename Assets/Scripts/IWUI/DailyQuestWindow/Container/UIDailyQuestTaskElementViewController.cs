using System;
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
    [IWUIBinding("#TaskButton")] private Button taskButton;
    [IWUIBinding("#TaskButtonLabel")] private NSText taskButtonLabel;
    [IWUIBinding("#View")] private CanvasGroup canvasGroup;

    private TaskEntity task;
    private UIDailyQuestTaskElementEntity targetEntity;

    public override void Init()
    {
        base.Init();
        
        targetEntity = entity as UIDailyQuestTaskElementEntity;

        Init(targetEntity.Task);
    }
    
    public void Init(TaskEntity task)
    {
        this.task = task;

        taskButton.interactable = !task.IsClaimed();
        taskButtonLabel.Text = GetTextForButton(task);

        taskIcon.sprite = UiQuestButton.GetIcon(task);

        lblProgress.Text = UiQuestButton.GetTaskProgress(task);
        
        lblDescription.Text = $"<color=#D2D2D2><size=25>[{task.Group} - {task.Id.ToLower()}]</size></color> " + task.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid)?.Message;

        lblReward.Text = GetRewardAsText();
        
        ToggleActive(!task.IsClaimed(), false);
    }

    private string GetTextForButton(TaskEntity task)
    {
        if (task.IsCompletedOrClaimed())
        {
            return LocalizationService.Get("window.daily.quest.task.button.claim", "window.daily.quest.task.button.claim");
        }
        
        return LocalizationService.Get("window.daily.quest.task.button.help", "window.daily.quest.task.button.help");
    }

    private string GetRewardAsText()
    {
        var reward = task.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        if (reward == null)
        {
            return "";
        }

        List<CurrencyPair> currencies;
        var pieces = CurrencyHellper.FiltrationRewards(reward, out currencies);
        
        var str = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), "");
        
        var stringBuilder = new StringBuilder($"<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>{str}</color></font> <size=50>");
            
        stringBuilder.Append(CurrencyHellper.RewardsToString("  ", pieces, currencies));
        stringBuilder.Append("</size>");
            
        return stringBuilder.ToString();
    }

    public void OnClick()
    {
        if (task.IsClaimed())
        {
            return;
        }
        
        if (task.IsCompletedOrClaimed())
        {
            ProvideReward();
            return;
        }
        
        targetEntity.WindowController.CloseCurrentWindow();
        task.Highlight();
    }

    private void ProvideReward(Action onComplete = null)
    {
        taskButton.interactable = false;
        
        task.SetClaimedState();

        ToggleActive(false, true);
        
        List<CurrencyPair> allReward = task.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        // Reward to drop to field
        List<CurrencyPair> currencies;
        Dictionary<int, int> pieces = CurrencyHellper.FiltrationRewards(allReward, out currencies);

        Action dropReward = () =>
        {
            var board = BoardService.Current.GetBoardById(0);
            var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];

            board.ActionExecutor.AddAction(new EjectionPieceAction
            {
                From = position,
                Pieces = pieces,
                OnComplete = () =>
                {
                    var view = board.RendererContext.GetElementAt(position) as CharacterPieceView;

                    if (view != null) view.StartRewardAnimation();

                    AddResourceView.Show(position, currencies);

                    onComplete?.Invoke();
                }
            });
        };
        
        // // Reward to fly to counters
        // List<CurrencyPair> nonPiecesReward = new List<CurrencyPair>();
        //
        // foreach (CurrencyPair reward in allReward)
        // {
        //     bool needToAdd = true;
        //     foreach (CurrencyPair drop in currencies)
        //     {
        //         if (drop.Currency == reward.Currency)
        //         {
        //             needToAdd = false;
        //             break;
        //         }
        //     }
        //
        //     if (needToAdd)
        //     {
        //         nonPiecesReward.Add(reward);
        //     }
        // }

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
    }

    private void ToggleActive(bool enabled, bool animated)
    {
        float alpha = enabled ? 1 : 0.4f;
        
        if (!animated)
        {
            canvasGroup.alpha = alpha;
            return;
        }

        canvasGroup.DOFade(alpha, 0.3f);
    }
}