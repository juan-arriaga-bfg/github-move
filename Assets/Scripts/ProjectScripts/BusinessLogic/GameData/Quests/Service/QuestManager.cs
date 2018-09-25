using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using UnityEngine;

public class QuestManager : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private List<QuestEntity> activeQuests = new List<QuestEntity>();
    private List<QuestStarterEntity> questStarters = new List<QuestStarterEntity>();

    public void Init()
    {
        var questStarter1 = GameDataService.Current.QuestsManager.InstantiateQuestStarter("Quest1");
        var questStarter2 = GameDataService.Current.QuestsManager.InstantiateQuestStarter("Quest2");

        questStarters.Add(questStarter1);
        questStarters.Add(questStarter2);

        // var quest = new QuestEntity();
        // quest.RegisterComponent(new QuestDescriptionComponent());
        // quest.RegisterComponent(new QuestRewardComponent());
        //         
        // TaskMatchCounterEntity taskMatch = new TaskMatchCounterEntity {Id = "MatchTaskId"};
        // taskMatch.RegisterComponent(new QuestDescriptionComponent());
        //         
        // TaskCreatePieceCounterEntity taskBuild = new TaskCreatePieceCounterEntity {Id = "CreateTaskId"};
        // taskBuild.RegisterComponent(new QuestDescriptionComponent());
        //
        // quest.RegisterComponent(taskMatch);
        // quest.RegisterComponent(taskBuild);
        
        // activeQuests.Add(quest);
    }
   
    public void ConnectToBoard()
    {
        CheckConditions();
        
        foreach (var quest in activeQuests)
        {
            quest.Start();
            Debug.Log(quest.ToString());
        }

        JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
        };
        
        var text1 = JsonConvert.SerializeObject(activeQuests[0], serializerSettings);
        var text2 = JsonConvert.SerializeObject(questStarters[0], serializerSettings);

        var item1 = JsonConvert.DeserializeObject<ECSEntity>(text1, serializerSettings);
        var item2 = JsonConvert.DeserializeObject<ECSEntity>(text2, serializerSettings);
        int i    = 0;
    }

    private void CheckConditions()
    {
        for (var i = 0; i < questStarters.Count; i++)
        {
            var starter = questStarters[i];
            if (starter.Check())
            {
                var quest = GameDataService.Current.QuestsManager.InstantiateQuest(starter.QuestToStartId);
                activeQuests.Add(quest);
            }
        }
    }
}