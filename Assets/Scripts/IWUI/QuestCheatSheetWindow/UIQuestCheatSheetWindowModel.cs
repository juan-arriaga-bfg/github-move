using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIQuestCheatSheetWindowModel : IWWindowModel
{
    public string Title => "Quest Cheats";

    public List<QuestEntity> Quests
    {
        get
        {
            if (quests == null)
            {
                RefreshQuestsList();
            }

            return quests;
        }
    }

    private List<QuestEntity> quests;
    
    private void RefreshQuestsList()
    {
        quests = new List<QuestEntity>();

        QuestsDataManager questsDataManager = GameDataService.Current.QuestsManager;
        var starters = questsDataManager.QuestStarters;

            foreach (var starter in starters)
        {
            var ids = starter.QuestToStartIds;
            foreach (var id in ids)
            {
                QuestEntity activeQuest = questsDataManager.GetActiveQuestById(id);
                if (activeQuest != null)
                {
                    quests.Add(activeQuest);
                }
                else
                {
                    QuestEntity quest = questsDataManager.InstantiateQuest(id);
                    quests.Add(quest);
                    
                }
            }
        }
    }

    public List<TaskEntity> Tasks
    {
        get
        {
            if (quests == null)
            {
                RefreshQuestsList();
            }

            List<TaskEntity> tasks = new List<TaskEntity>();
            foreach (var quest in quests)
            {
                tasks.Add(quest.Tasks[0]);
            }

            return tasks;
        }
    }
}