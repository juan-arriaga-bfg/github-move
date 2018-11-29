﻿using System;
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
    [IWUIBinding("#TaskButton")] private Button taskButton;
    [IWUIBinding("#TaskButtonLabel")] private NSText taskButtonLabel;
    [IWUIBinding("#View")] private CanvasGroup canvasGroup;

    private TaskEntity task;
    private UIDailyQuestTaskElementEntity targetEntity;

    private List<BoardPosition> listOfPiecesToHighlight;

    private UIDailyQuestWindowView dailyQuestWindow;
    
    public override void Init()
    {
        base.Init();
        
        targetEntity = entity as UIDailyQuestTaskElementEntity;

        task = targetEntity.Task;
        
        targetEntity.Task.OnChanged += OnTaskChanged;

        listOfPiecesToHighlight = null;
        
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
           var ray = GodRayView.Show(pos);
           ray.Remove();
        }
    }

    public void UpdateUi()
    {
        taskButton.interactable = !task.IsClaimed();
        taskButtonLabel.Text = GetTextForButton(task);

        taskIcon.sprite = UiQuestButton.GetIcon(task);

        lblProgress.Text = UiQuestButton.GetTaskProgress(task);
        
        // lblDescription.Text = $"<color=#D2D2D2><size=25>[{task.Group} - {task.Id.ToLower()}]</size></color> " + task.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid)?.Message;

        string key = task.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid)?.Message;
        lblDescription.Text = LocalizationService.Get(key, key) ;

        lblReward.Text = GetRewardAsText();
        
        ToggleActive(!task.IsClaimed(), false);
    }

    private string GetTextForButton(TaskEntity task)
    {
        string key;
        
        switch (task.State)
        {
            case TaskState.Completed:
                key = LocalizationService.Get("window.daily.quest.task.button.claim", "window.daily.quest.task.button.claim");
                break;
            
            case TaskState.Claimed:
                key = LocalizationService.Get("window.daily.quest.task.button.done", "window.daily.quest.task.button.done");
                break;
            
            default:
                key = LocalizationService.Get("window.daily.quest.task.button.help", "window.daily.quest.task.button.help");
                break;
        }

        return key;
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
        taskButton.interactable = false;
        
        task.SetClaimedState();

        ToggleActive(false, true);
        
        List<CurrencyPair> allReward = task.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        var board = BoardService.Current.FirstBoard;
        BoardPosition npcPos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
        
        // Reward to drop to field
        List<CurrencyPair> currencies;
        Dictionary<int, int> pieces = CurrencyHellper.FiltrationRewards(allReward, out currencies);

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
        float alpha = enabled ? 1 : 0.4f;
        
        if (!animated)
        {
            canvasGroup.alpha = alpha;
            return;
        }

        canvasGroup.DOFade(alpha, 0.3f);
    }
}