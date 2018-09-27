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

    private List<QuestEntity> activeQuests;
    private List<QuestStarterEntity> questStarters;

    public void Init()
    {
        activeQuests = new List<QuestEntity>();
        questStarters = new List<QuestStarterEntity>();
        
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
        var questSave = ProfileService.Current.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);
        // var json      = JsonConvert.SerializeObject(questSave);
        
        Load(questSave);
        
        // Run new quests if conditions changed 
        CheckConditions();

        JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
        };
        
        // var text1 = JsonConvert.SerializeObject(activeQuests[0], serializerSettings);
        // var text2 = JsonConvert.SerializeObject(questStarters[0], serializerSettings);
        //
        // var item1 = JsonConvert.DeserializeObject<ECSEntity>(text1, serializerSettings);
        // var item2 = JsonConvert.DeserializeObject<ECSEntity>(text2, serializerSettings);
        // int i    = 0;
    }

    private void CheckConditions()
    {
        for (var i = 0; i < questStarters.Count; i++)
        {
            var starter = questStarters[i];
            if (starter.Check())
            {
                 StartQuestById(starter.QuestToStartId, null);
            }
        }
    }

    public QuestEntity StartQuestById(string id, JToken saveData)
    {
        QuestEntity quest = GameDataService.Current.QuestsManager.InstantiateQuest(id);
        activeQuests.Add(quest);

        quest.Start(saveData);

        return quest;
    }
    
    public void Cleanup()
    {
        if (activeQuests == null)
        {
            return;
        }

        foreach (var quest in activeQuests)
        {
            quest.Cleanup();
        }
    }

        
    private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Objects,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
    };
    
    public List<QuestSaveData> GetQuestsSaveData()
    {
        List<QuestSaveData> questDatas = new List<QuestSaveData>();
        foreach (var quest in activeQuests)
        {
            questDatas.Add(quest.GetDataForSerialization());
        }

        return questDatas;
    }
    
    public void Serialize()
    {
        List<QuestSaveData> questDatas = new List<QuestSaveData>();
        foreach (var quest in activeQuests)
        {
            questDatas.Add(quest.GetDataForSerialization());
        }

        string text = JsonConvert.SerializeObject(questDatas);
        File.WriteAllText(@"D:/save.json", text);
        
        //QuestEntity quest1 = activeQuests.First(e => e.Id == "Quest1");
        // string questData = JsonConvert.SerializeObject(activeQuests[0]);
        // string tasksData = JsonConvert.SerializeObject(activeQuests[0].Tasks);
        //
        // File.WriteAllText(@"D:/qsave.json", questData);
        // File.WriteAllText(@"D:/tsave.json", tasksData);
    }

    public void Load(QuestSaveComponent save)
    {
        Cleanup();
        activeQuests.Clear();

        if (save.ActiveQuests == null)
        {
            return;
        }
        
        // string text = File.ReadAllText(@"D:/save.json");
        JToken root = JToken.Parse(save.ActiveQuests[0].DataAsJson);
        JToken active = root["ActiveQuests"];
        
        foreach (JToken saveData in active)
        {
            JToken questNode = saveData["Quest"];
            string id = questNode["Id"].Value<string>();

            StartQuestById(id, saveData);
            
            //Debug.Log("LOADED:\n" + quest);
        }
        
        // string textQ = File.ReadAllText(@"D:/qsave.json");
        // string textT = File.ReadAllText(@"D:/tsave.json");
        // QuestEntity quest = activeQuests.First(e => e.Id == "Quest1");
        // JsonConvert.PopulateObject(textQ, quest);
        //
        // JToken root = JToken.Parse(textT)/*.First*/;
        //
        // foreach (var node in root)
        // {
        //     string id = node.SelectToken("Id").Value<string>();
        //     TaskEntity task = quest.Tasks.First(e => e.Id == id);
        //     JsonConvert.PopulateObject(node.ToString(), task);
        // }
    }
}