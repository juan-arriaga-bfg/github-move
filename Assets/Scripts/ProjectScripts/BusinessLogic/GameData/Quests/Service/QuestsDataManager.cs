using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuestsDataManager : IECSComponent, IDataManager/*, IDataLoader<List<QuestDefOld>>*/
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private Dictionary<Type, Dictionary<string, JToken>> cache;
    
    public List<QuestEntity> ActiveQuests;
    public List<string> CompletedQuests;

    public Action OnActiveQuestsListChanged;
    
    private List<QuestStarterEntity> questStarters;

    public bool ConnectedToBoard { get; private set; }
    
    private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Objects,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
        // Converters = new List<JsonConverter> {new ECSEntityJsonConverter(), new VectorConverter()}
    };
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

#region Save/Load    
    
    public void Reload()
    {
        OnActiveQuestsListChanged = null;
        questStarters = null;
        
        cache = new Dictionary<Type, Dictionary<string, JToken>>();

        LoadData<QuestStartConditionComponent>("configs/quests/conditions.data");
        LoadData<QuestStarterEntity>          ("configs/quests/starters.data");
        LoadData<QuestEntity>                 ("configs/quests/quests.data");
        LoadData<TaskEntity>                  ("configs/quests/tasks.data");

        LoadProfile();
    }

    private void LoadProfile()
    {
        var questSave = ProfileService.Current.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);

        CompletedQuests = questSave.CompletedQuests ?? new List<string>();
        
        ActiveQuests = new List<QuestEntity>();

        if (questSave.ActiveQuests == null)
        {
            return;
        }

        foreach (var activeQuest in questSave.ActiveQuests)
        {
            JToken saveData  = activeQuest.Data; 
            JToken questNode = saveData["Quest"];
            string id        = questNode["Id"].Value<string>();
        
           StartQuestById(id, saveData);
        }
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
    
    public List<QuestSaveData> GetQuestsSaveData()
    {
        List<QuestSaveData> questDatas = new List<QuestSaveData>();
        foreach (var quest in ActiveQuests)
        {
            questDatas.Add(quest.GetDataForSerialization());
        }

        return questDatas;
    }
       
    public void CreateStarters()
    {
        questStarters = new List<QuestStarterEntity>();

        // var s = InstantiateQuestStarter(@id: "1");
        // questStarters.Add(s);
        
        for (int i = 1; i <= 30; i++)
        {
            var starter = InstantiateQuestStarter(i.ToString());
            questStarters.Add(starter);
        }
    }
    
#endregion

#region Builder
    
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
    
#endregion
    
#region Board Connection
    
    public void ConnectToBoard()
    {          
        // Run new quests if conditions changed 
        CheckConditions();

        if (ActiveQuests != null)
        {
            foreach (var quest in ActiveQuests)
            {
                quest.ConnectToBoard();
            }
        }

        ConnectedToBoard = true;
    }

    public void DisconnectFromBoard()
    {
        ConnectedToBoard = false;
        
        if (ActiveQuests == null)
        {
            return;
        }

        foreach (var quest in ActiveQuests)
        {
            quest.DisconnectFromBoard();
        }
    }
    
#endregion
    
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
    
    public QuestEntity StartQuestById(string id, JToken saveData)
    {
        QuestEntity quest = InstantiateQuest(id);
        ActiveQuests.Add(quest);

        quest.Start(saveData);

        OnActiveQuestsListChanged?.Invoke();
        
        return quest;
    }

    public QuestEntity GetActiveQuestById(string id)
    {
        for (var i = 0; i < ActiveQuests.Count; i++)
        {
            var quest = ActiveQuests[i];
            if (quest.Id == id)
            {
                return quest;
            }
        }

        return null;
    }

    public void CompleteQuest(string id)
    {
        for (var i = 0; i < ActiveQuests.Count; i++)
        {
            var quest = ActiveQuests[i];
            if (quest.Id == id)
            {
                CompletedQuests.Add(id);
                ActiveQuests.RemoveAt(i);
                quest.DisconnectFromBoard();
                
                OnActiveQuestsListChanged?.Invoke();
                
                CheckConditions();
                return;
            }
        }
        
        Debug.LogError($"[QuestsDataManager] => CompleteQuest: Quest with id '{id}' is not active!");
    }
}