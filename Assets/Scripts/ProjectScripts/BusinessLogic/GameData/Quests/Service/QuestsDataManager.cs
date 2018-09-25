using System;
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
    private Dictionary<string, JToken> questStartConditions;
    private Dictionary<string, JToken> questStarters;
    private Dictionary<string, JToken> tasks;
    private Dictionary<string, JToken> quests;

    private Dictionary<Type, Dictionary<string, JToken>> cache;
    
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
                cache = new Dictionary<Type, Dictionary<string, JToken>>();
                
                string conditionsFile = @"C:\Users\keht\.Rider2018.2\config\scratches\QuestStartConditions.json";
                questStartConditions = Parse(conditionsFile);
                
                string startersFile = @"C:\Users\keht\.Rider2018.2\config\scratches\QuestStarters.json";
                questStarters = Parse(startersFile);
                
                string questsFile = @"C:\Users\keht\.Rider2018.2\config\scratches\Quests.json";
                quests = Parse(questsFile);
                
                string tasksFile = @"C:\Users\keht\.Rider2018.2\config\scratches\Tasks.json";
                tasks = Parse(tasksFile);
                
                cache.Add(typeof(QuestStartConditionComponent), questStartConditions);
                cache.Add(typeof(QuestStarterEntity), questStarters);
                cache.Add(typeof(QuestEntity), quests);
                cache.Add(typeof(TaskEntity), tasks);
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
            
#if DEBUG
            if (ret.ContainsKey(id))
            {
                Debug.LogError($"[QuestsDataManager] => Parse: Duplicate ID '{id} found in {file}!");
            }
#endif
            
            ret.Add(id, node);
        }
    
        return ret;
    }

    private T InstantiateById<T>(string id) where T : IECSComponent
    {
        Dictionary<string, JToken> dictWithConfigs;
        if (!cache.TryGetValue(typeof(T), out dictWithConfigs))
        {
            Debug.LogError($"[QuestsDataManager] => InstantiateById: Configs cache for type '{typeof(T)}' not found!");
            return default(T);
        }
        
        JToken config;
        if (!dictWithConfigs.TryGetValue(id, out config))
        {
            Debug.LogError($"[QuestsDataManager] => InstantiateById: Config not found for id '{id}' with type '{typeof(T)}'!");
            return default(T); 
        }
    
        var bkp = JsonConvert.DefaultSettings;
        JsonConvert.DefaultSettings = () => serializerSettings;
                
        T item = config.ToObject<T>();
    
        JsonConvert.DefaultSettings = bkp;
    
        return item;
    }

    private QuestStartConditionComponent InstantiateCondition(string id)
    {
        return InstantiateById<QuestStartConditionComponent>(id);
    }    
    
    private TaskEntity InstantiateTask(string id)
    {
        return InstantiateById<TaskEntity>(id);
    }
    
    public QuestStarterEntity InstantiateQuestStarter(string id)
    {
        var starter = InstantiateById<QuestStarterEntity>(id);
        foreach (var conditionId in starter.ConditionIds)
        {
            var condition = InstantiateCondition(conditionId);
            starter.RegisterComponent(condition);
        }

        return starter;
    }

    public QuestEntity InstantiateQuest(string id)
    {
        var quest = InstantiateById<QuestEntity>(id);
        foreach (var taskDef in quest.TaskDefs)
        {
            var task = InstantiateTask(taskDef.TaskId);
            task.Order = taskDef.Order;
            
            quest.RegisterComponent(task);
        }

        return quest;
    }
}