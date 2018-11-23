using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class UIDailyQuestTaskElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#TaskDescription")] private NSText lblDescription;
    [IWUIBinding("#TaskProgress")] private NSText lblProgress;
    [IWUIBinding("#TaskReward")] private NSText lblReward;
    [IWUIBinding("#TaskIcon")] private Image taskIcon;
    [IWUIBinding("#TaskButton")] private Button taskButton;
    [IWUIBinding("#TaskButtonLabel")] private NSText taskButtonLabel;

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
        
        lblDescription.Text = task.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid)?.Message;

        lblReward.Text = GetReward();
    }

    private string GetTextForButton(TaskEntity task)
    {
        if (task.IsCompletedOrClaimed())
        {
            return LocalizationService.Get("window.daily.quest.task.button.claim", "window.daily.quest.task.button.claim");
        }
        
        return LocalizationService.Get("window.daily.quest.task.button.help", "window.daily.quest.task.button.help");
    }

    private string GetReward()
    {
        var reward = task.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        if (reward == null)
        {
            return "";
        }

        List<CurrencyPair> currencysReward;
        var piecesReward = CurrencyHellper.FiltrationRewards(reward, out currencysReward);
        
        var str = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), "");
        
        var stringBuilder = new StringBuilder($"<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>{str}</color></font> <size=50>");
            
        stringBuilder.Append(CurrencyHellper.RewardsToString("  ", piecesReward, currencysReward));
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

        List<CurrencyPair> reward = task.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        CurrencyHellper.Purchase(reward, success =>
        {
            onComplete?.Invoke();
        },
        taskIcon.transform.position);
    }
}