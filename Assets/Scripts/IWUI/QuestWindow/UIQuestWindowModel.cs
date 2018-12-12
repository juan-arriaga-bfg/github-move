using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Quests;

public class UIQuestWindowModel : IWWindowModel
{
    private QuestEntity quest;
    public QuestEntity Quest
    {
        get { return quest; }
        set
        {
            quest = value;
            tasks = quest?.Tasks;
        }
    }

    private List<TaskEntity> tasks;

    private TaskEntity FirstTask => tasks[0];
    
    public string Title => quest.GetLocalizedTitle();

    public string Message => quest.GetLocalizedMessage();

    public string Description => FirstTask.GetLocalizedMessage();

    public Dictionary<int, int> PiecesReward;
    public List<CurrencyPair> CurrencysReward;
    
    public string ButtonText
    {
        get
        {
            switch (Quest.State)
            {
                case TaskState.Pending:
                case TaskState.New:
                case TaskState.InProgress:                                                                                              
                    return LocalizationService.Get("common.button.find", "common.button.find");

                case TaskState.Completed:
                    return LocalizationService.Get("common.button.claim", "common.button.claim");

                case TaskState.Claimed:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return string.Empty;
        }
    }

    public string AmountText
    {
        get
        {
            var task = FirstTask;
            return UiQuestButton.GetTaskProgress(task);
        }
    }
    
    public string RewardText
    {
        get
        {
            var str = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), "");
            var strBuilder = new StringBuilder($"<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>{str}</color></font> <size=50>");
            
            strBuilder.Append(CurrencyHellper.RewardsToString("  ", PiecesReward, CurrencysReward));
            strBuilder.Append("</size>");
            
            return strBuilder.ToString();
        }
    }

    public Sprite Icon => UiQuestButton.GetIcon(FirstTask);

    public void InitReward()
    {
        var reward = quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;
        
        if(reward == null) return;

        PiecesReward = CurrencyHellper.FiltrationRewards(reward, out CurrencysReward);
    }
}
