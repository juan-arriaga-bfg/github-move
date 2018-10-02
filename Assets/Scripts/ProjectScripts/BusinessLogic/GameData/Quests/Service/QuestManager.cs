using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuestManager : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private List<QuestStarterEntity> questStarters;

    public bool ConnectedToBoard { get; private set; }
    
    public void Init()
    {
        questStarters = new List<QuestStarterEntity>();

        for (int i = 1; i <= 10; i++)
        {
            var starter = GameDataService.Current.QuestsManager.InstantiateQuestStarter(i.ToString());
            questStarters.Add(starter);
        }
        
        // var questStarter1 = GameDataService.Current.QuestsManager.InstantiateQuestStarter("Quest_3");
        // var questStarter2 = GameDataService.Current.QuestsManager.InstantiateQuestStarter("Quest_2");
        //
        
        // questStarters.Add(questStarter1);
        // questStarters.Add(questStarter2);
    }
   
    public void ConnectToBoard()
    {          
        // Run new quests if conditions changed 
        CheckConditions();

        var dataManager = GameDataService.Current.QuestsManager;
        
        if (dataManager.ActiveQuests != null)
        {
            foreach (var quest in dataManager.ActiveQuests)
            {
                quest.ConnectToBoard();
            }
        }

        ConnectedToBoard = true;
    }

    public void CheckConditions()
    {
        var dataManager = GameDataService.Current.QuestsManager;
        
        for (var i = 0; i < questStarters.Count; i++)
        {
            var starter = questStarters[i];
            if (starter.Check())
            {
                var quest = dataManager.StartQuestById(starter.QuestToStartId, null);
                if (ConnectedToBoard)
                {
                    quest.ConnectToBoard();
                }
            }
        }
    }
   
    public void DisconnectFromBoard()
    {
        var dataManager = GameDataService.Current.QuestsManager;
        
        if (dataManager.ActiveQuests == null)
        {
            return;
        }

        foreach (var quest in dataManager.ActiveQuests)
        {
            quest.DisconnectFromBoard();
        }
    }
}