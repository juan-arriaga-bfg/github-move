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

        // var s = GameDataService.Current.QuestsManager.InstantiateQuestStarter(@id: "1");
        // questStarters.Add(s);
        
        for (int i = 1; i <= 30; i++)
        {
            var starter = GameDataService.Current.QuestsManager.InstantiateQuestStarter(i.ToString());
            questStarters.Add(starter);
        }
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
#if DEBUG
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif        
        var dataManager = GameDataService.Current.QuestsManager;
        
        for (var i = 0; i < questStarters.Count; i++)
        {
            var starter = questStarters[i];
            if (starter.Check())
            {
#if DEBUG
                sw.Stop();
#endif
                var quest = dataManager.StartQuestById(starter.QuestToStartId, null);
                if (ConnectedToBoard)
                {
                    quest.ConnectToBoard();
                }
#if DEBUG
                sw.Start();
#endif
            }
        }
#if DEBUG
      sw.Stop();
      Debug.Log($"[QuestManager] => CheckConditions() done in {sw.ElapsedMilliseconds}ms");
#endif
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