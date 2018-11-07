using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuestsDataManager : IECSComponent, IDataManager
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private Dictionary<Type, Dictionary<string, JToken>> cache;
    
    /// <summary>
    /// List of Quests that currently in progress
    /// </summary>
    public List<QuestEntity> ActiveQuests;
    
    /// <summary>
    /// List of ids of completed quests
    /// </summary>
    public List<string> CompletedQuests;

    /// <summary>
    /// Will be invoked when we Start new or Claim active quests. This mean that ActiveQuests list is changing.
    /// </summary>
    public Action OnActiveQuestsListChanged;
    
    private List<QuestStarterEntity> questStarters;

    /// <summary>
    /// Flag that indicates that all active quests and tasks are listening to BoardEvents
    /// </summary>
    public bool ConnectedToBoard { get; private set; }

    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

#region Save/Load    
    
    private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Objects,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
        // Converters = new List<JsonConverter> {new ECSEntityJsonConverter(), new VectorConverter()}
    };
    
    public void Reload()
    {
        OnActiveQuestsListChanged = null;
        questStarters = null;

        cache = new Dictionary<Type, Dictionary<string, JToken>>();
        
        LoadData<QuestStarterEntity>          ("configs/questStarters.data");
        LoadData<QuestEntity>                 ("configs/quests.data");
        LoadData<TaskEntity>                  ("configs/questTasks.data");

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
        
        Dictionary<string, JToken> starterConfigs;
        if (!cache.TryGetValue(typeof(QuestStarterEntity), out starterConfigs))
        {
            Debug.LogError($"[QuestsDataManager] => CreateStarters: Configs cache for type '{typeof(QuestStarterEntity)}' not found!");
            return;
        }
        
        foreach (var config in starterConfigs.Values)
        {
            QuestStarterEntity starter = InstantiateFromJson<QuestStarterEntity>(config);
            
            foreach (var condition in starter.Conditions)
            {
                starter.RegisterComponent(condition);
            }
            
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


        T ret = InstantiateFromJson<T>(config);
        return ret;
    }

    private T InstantiateFromJson<T>(JToken token) where T : IECSComponent
    {
        var bkp = JsonConvert.DefaultSettings;
        JsonConvert.DefaultSettings = () => serializerSettings;
                
        T item = token.ToObject<T>();
    
        JsonConvert.DefaultSettings = bkp;
        
        return item;
    }
    
    private TaskEntity InstantiateTask(string id)
    {
        return InstantiateById<TaskEntity>(id);
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
        StartNewQuestsIfAny();

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

    public bool StartNewQuestsIfAny()
    {
        const string ACTION_ID = "StartNewQuestsIfAny";
        
        Debug.Log($"[QuestsDataManager] => StartNewQuestsIfAny Schedule");
        
        var action = new QueueActionComponent {Id = ACTION_ID}
                    .AddCondition(new OpenedWindowsQueueConditionComponent {IgnoredWindows = new HashSet<string> {UIWindowType.MainWindow}})
                    .SetAction(() =>
                     {
                         var questsToStart = CheckConditions();
                         if (questsToStart.Count == 0)
                         {
                             Debug.Log($"[QuestsDataManager] => CheckConditions == 0, return");
                             return;
                         }

                         Debug.Log($"[QuestsDataManager] => StartNewQuestsIfAny before dlg");
                         
                         var model = UIService.Get.GetCachedModel<UIQuestStartWindowModel>(UIWindowType.QuestStartWindow);
                         model.SetQuests(questsToStart);
                         model.BuildTestConversation();
                         //model.BuildQuestCompletedConversation();
        
                         UIService.Get.ShowWindow(UIWindowType.QuestStartWindow);
                     });

        ProfileService.Current.QueueComponent.AddAction(action, true);

        return true;
    }

    public void StartQuests(List<string> questIds)
    {
        var dataManager = GameDataService.Current.QuestsManager;
        
        foreach (var id in questIds)
        {
            QuestEntity quest = GetActiveQuestById(id);
            if (quest != null)
            {
                // Quest already started
                continue;
            }
            
            quest = dataManager.StartQuestById(id, null);
            if (ConnectedToBoard)
            {
                quest.ConnectToBoard();
            }
        }
    }
    
    /// <summary>
    /// Call this to foreach through Starters and Start new quests if Conditions are met.
    /// Highly expensive! 
    /// </summary>
    private List<string> CheckConditions()
    {
        if (questStarters == null)
        {
            Debug.LogError($"[QuestsDataManager] => CheckConditions() is called when questStarters list is empty");
            return null;
        }

        List<string> ret = new List<string>();
        
#if DEBUG
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif        

        for (var i = 0; i < questStarters.Count; i++)
        {
            var starter = questStarters[i];
            if (starter.Check())
            {
#if DEBUG
                sw.Stop();
#endif
                ret.AddRange(starter.QuestToStartIds);
#if DEBUG
                sw.Start();
#endif
            }
        }
#if DEBUG
        sw.Stop();
        Debug.Log($"[QuestsDataManager] => CheckConditions() done in {sw.ElapsedMilliseconds}ms");
#endif
        return ret;
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

    private QuestEntity StartQuestById(string id, JToken saveData)
    {
        QuestEntity quest = GetActiveQuestById(id);
        
        if (quest != null)
        {
            Debug.LogError($"[QuestsDataManager] => StartQuestById({id}): quest already started");
            return null;
        }
        
        quest = InstantiateQuest(id);
        ActiveQuests.Add(quest);

        quest.Start(saveData);

        OnActiveQuestsListChanged?.Invoke();
        
        return quest;
    }

    /// <summary>
    /// Call it when a player claimed a reward to remove a quest from ActiveQuests list
    /// </summary>
    /// <param name="id"></param>
    public void CompleteQuest(string id)
    {
        Debug.Log("!!! CompleteQuest: " + id);
        
        for (var i = 0; i < ActiveQuests.Count; i++)
        {
            var quest = ActiveQuests[i];
            if (quest.Id == id)
            {
                CompletedQuests.Add(id);
                ActiveQuests.RemoveAt(i);
                quest.DisconnectFromBoard();
                
                OnActiveQuestsListChanged?.Invoke();
                
                StartNewQuestsIfAny();
                return;
            }
        }
        
        Debug.LogError($"[QuestsDataManager] => CompleteQuest: Quest with id '{id}' is not active!");
    }
}