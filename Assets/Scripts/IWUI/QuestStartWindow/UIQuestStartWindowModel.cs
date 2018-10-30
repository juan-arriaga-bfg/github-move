using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIQuestStartWindowModel : IWWindowModel
{
    public List<QuestEntity> Quests { get; private set; }

    public void SetQuests(List<string> questsToStart)
    {
        var questManager = GameDataService.Current.QuestsManager;
        
        Quests = new List<QuestEntity>();
        foreach (var id in questsToStart)
        {
            var quest = questManager.InstantiateQuest(id);
            Quests.Add(quest);
        }
    }
}
