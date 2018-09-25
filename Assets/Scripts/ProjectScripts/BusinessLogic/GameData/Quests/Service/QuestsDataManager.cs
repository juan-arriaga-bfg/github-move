using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quests;
using UnityEngine;

public class QuestsDataManager : IECSComponent, IDataManager, IDataLoader<List<QuestDefOld>>
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    // private Dictionary<string, QuestBase>   quests;
    private Dictionary<string, JToken> tasks;
    private Dictionary<string, string> subtasks;

    private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Objects,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
    };
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Reload()
    {		
        LoadData(new ResourceConfigDataMapper<List<QuestDefOld>>("configs/quests.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
	
    public void LoadData(IDataMapper<List<QuestDefOld>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                // quests = data;

                // quests   = Parse<QuestBase>  ("D:/quests.json"  );
                // tasks    = Parse("D:/tasks.json"   );

                return;
                
                TaskEntity task = new TaskEntity {Id = "SomeTaskId"};
                task.RegisterComponent(new QuestDescriptionComponent());
                task.RegisterComponent(new QuestRewardComponent());
                
                SubtaskMatchCounterEntity subtaskMatch = new SubtaskMatchCounterEntity {Id = "MatchSubtaskId"};
                subtaskMatch.RegisterComponent(new QuestDescriptionComponent());
                
                SubtaskCreatePieceCounterEntity subtaskBuild = new SubtaskCreatePieceCounterEntity {Id = "CreateSubtaskId"};
                subtaskBuild.RegisterComponent(new QuestDescriptionComponent());

                task.RegisterComponent(subtaskMatch);
                task.RegisterComponent(subtaskBuild);


                // SubtaskEntity ent = new SubtaskEntity {Id = "SomeId"};
                // ent.RegisterComponent(new QuestDescriptionComponent());
                // ent.RegisterComponent(new QuestRewardComponent());
                //
                var text = JsonConvert.SerializeObject(task, serializerSettings);
                // Debug.LogWarning(text);
                //
                var item = JsonConvert.DeserializeObject<ECSEntity>(text, serializerSettings);
                int i = 0;

                // subtasks = Parse("D:/subtasks.json");

                // var quest = quests.First();
                // var text  = JsonConvert.SerializeObject(quest);
                // File.WriteAllText("D:/serialized.json", text);
                //
                // var questSave = ProfileService.Current.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);
                //
                // if (questSave != null)
                // {
                //
                // }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
    
    private Dictionary<string, JToken> Parse(string file)
    {
        Dictionary<string, JToken> ret = new Dictionary<string, JToken>();

        var json = File.ReadAllText(file);

        JToken root = JObject.Parse(json).First;
        
        foreach (var node in root.First)
        {
            string id = node.SelectToken("Id").Value<string>();        
            ret.Add(id, node);
        }
    
        return ret;
    }

    // private TaskBase InstantiateTaskById(string id)
    // {
    //     var taskConfig = tasks["TaskMatch"];
    //
    //     var bkp = JsonConvert.DefaultSettings;
    //     JsonConvert.DefaultSettings = () => serializerSettings;
    //             
    //     var item = taskConfig.ToObject<TaskBase>();
    //
    //     JsonConvert.DefaultSettings = bkp;
    //
    //     return item;
    // }

    // private Dictionary<string, T> Parse<T>(string file) where T : IHaveId
    // {
    //     Dictionary<string, T> ret = new Dictionary<string, T>();
    //
    //     CurrencyPair p = new CurrencyPair {Currency = PieceType.Chest1.Abbreviations[0], Amount = 1};
    //     var obj = JsonConvert.SerializeObject(p);
    //     
    //     var json = File.ReadAllText(file);
    //     List<T> list = JsonConvert.DeserializeObject<List<T>>(json, serializerSettings);
    //     foreach (var item in list)
    //     {        
    //         ret.Add(item.Id, item);
    //     }
    //     // JObject root = JObject.Parse(json);
    //     //
    //     // foreach (var node in root)
    //     // {
    //     //     string val = node.ToString();
    //     //     var item = JsonConvert.DeserializeObject(val, serializerSettings);
    //     //     
    //     //     T itemTyped = (T) item;         
    //     //     ret.Add(itemTyped.Id, itemTyped);
    //     // }
    //
    //     return ret;
    // }
    
    // public QuestBase GetQuestById(string id)
    // {
    //     QuestBase ret;
    //     quests.TryGetValue(id, out ret);
    //     return ret;
    // }
    //
    // public TaskBase GetTaskById(string id)
    // {
    //     TaskBase ret;
    //     tasks.TryGetValue(id, out ret);
    //     return ret;
    // }
    //
    // public SubtaskBase GetSubtaskById(string id)
    // {
    //     SubtaskBase ret;
    //     subtasks.TryGetValue(id, out ret);
    //     return ret;
    // }
}