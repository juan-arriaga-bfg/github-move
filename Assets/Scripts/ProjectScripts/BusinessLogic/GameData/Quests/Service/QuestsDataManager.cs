using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuestsDataManager : IECSComponent, IDataManager/*, IDataLoader<List<QuestDefOld>>*/
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

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
        cache = new Dictionary<Type, Dictionary<string, JToken>>();

        LoadData<QuestStartConditionComponent>("configs/quests/conditions.data");
        LoadData<QuestStarterEntity>          ("configs/quests/starters.data");
        LoadData<QuestEntity>                 ("configs/quests/quests.data");
        LoadData<TaskEntity>                  ("configs/quests/tasks.data");

        // LoadProfile();
    }

    private void LoadProfile()
    {
  
    }

    private void LoadData<T>(string path)
    {
        var dataMapper = new ResourceConfigDataMapper<T>(path, NSConfigsSettings.Instance.IsUseEncryption);
        var json = dataMapper.GetDataAsJson();
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogErrorFormat("Config '{0}' not loaded", path);
            return;
        }
        
        var configs = Parse(json);
        cache.Add(typeof(T), configs);
    }
    
    private Dictionary<string, JToken> Parse(string json)
    {
        Dictionary<string, JToken> ret = new Dictionary<string, JToken>();

        JToken root = JToken.Parse(json);
        
        foreach (var node in root)
        {
            string id = node["Id"].Value<string>();  
            
#if DEBUG
            if (ret.ContainsKey(id))
            {
                Debug.LogError($"[QuestsDataManager] => Parse: Duplicate ID '{id}' found in\n'{json}'!");
            }
#endif
            
            ret.Add(id, node);
        }
    
        return ret;
    }

    private T InstantiateById<T>(string id) where T : IECSComponent
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError($"[QuestsDataManager] => InstantiateById: called with null or empty id!");
            return default(T);
        }
        
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