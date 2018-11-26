using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Quests;
using UnityEngine;

public sealed class QuestsDataManager : IECSComponent, IDataManager
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private Dictionary<Type, Dictionary<string, JToken>> cache;
    
    /// <summary>
    /// List of Story-related Quests that currently in progress
    /// </summary>
    public List<QuestEntity> ActiveStoryQuests;
    
    /// <summary>
    /// List of All active quests that handled by the Manager
    /// </summary>
    public List<QuestEntity> ActiveQuests;
    
    public DailyQuestEntity DailyQuest;
    
    /// <summary>
    /// List of ids of completed quests
    /// </summary>
    public List<string> FinishedQuests;

    /// <summary>
    /// Will be invoked when we Start new or Claim active quests. This mean that ActiveQuests list is changing.
    /// </summary>
    public Action OnActiveQuestsListChanged;
    
    /// <summary>
    /// Will be invoked when quest state is changing.
    /// </summary>
    public Action<QuestEntity, TaskEntity> OnQuestStateChanged;
    
    public List<QuestStarterEntity> QuestStarters;

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
        QuestStarters = null;

        cache = new Dictionary<Type, Dictionary<string, JToken>>();
        
        LoadData<QuestStarterEntity>          ("configs/questStarters.data");
        LoadData<QuestEntity>                 ("configs/quests.data");
        LoadData<TaskEntity>                  ("configs/questTasks.data");

        if (Application.isPlaying)// Check for UT
        {
            LoadProfile();
        }
    }

    private void LoadProfile()
    {
        var questSave = ProfileService.Current.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);

        FinishedQuests = questSave.FinishedQuests ?? new List<string>();
        
        ActiveStoryQuests = new List<QuestEntity>();
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
       
    public bool CreateStarters()
    {
        QuestStarters = new List<QuestStarterEntity>();
        
        Dictionary<string, JToken> starterConfigs;
        if (!cache.TryGetValue(typeof(QuestStarterEntity), out starterConfigs))
        {
            Debug.LogError($"[QuestsDataManager] => CreateStarters: Configs cache for type '{typeof(QuestStarterEntity)}' not found!");
            return false;
        }
        
        foreach (var config in starterConfigs.Values)
        {
            QuestStarterEntity starter = InstantiateFromJson<QuestStarterEntity>(config);
            
            foreach (var condition in starter.Conditions)
            {
                starter.RegisterComponent(condition);
            }
            
            QuestStarters.Add(starter);
        }

        return true;
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

    public TaskEntity InstantiateTask(string id)
    {
        return InstantiateById<TaskEntity>(id);
    }

    public QuestEntity InstantiateQuest(string id)
    {
        var quest = InstantiateById<QuestEntity>(id);
        if (quest == null)
        {
            return null;
        }
        
        foreach (var taskDef in quest.TaskDefs)
        {
            var task = InstantiateTask(taskDef.TaskId);
            if (task == null)
            {
                continue;
            }
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
                    .AddCondition(new OpenedWindowsQueueConditionComponent {IgnoredWindows = UIWindowType.IgnoredWindows})
                    .SetAction(() =>
                     {
                         string starterId = null;
                         var questsToStart = CheckConditions(out starterId);
                         if (questsToStart.Count == 0)
                         {
                             Debug.Log($"[QuestsDataManager] => CheckConditions == 0, return");
                             return;
                         }

                         Debug.Log($"[QuestsDataManager] => StartNewQuestsIfAny before dlg");

                         if (!DevTools.IsQuestDialogsEnabled())
                         {
                             DevTools.FastStartQuest(questsToStart);
                             return;
                         }

                         var model = UIService.Get.GetCachedModel<UIQuestStartWindowModel>(UIWindowType.QuestStartWindow);
                         model.Init(null, questsToStart, starterId);

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
    public List<string> CheckConditions(out string starterId)
    {
        starterId = null;
        
        if (QuestStarters == null)
        {
            Debug.LogError($"[QuestsDataManager] => CheckConditions() is called when questStarters list is empty");
            return null;
        }

        List<string> ret = new List<string>();
        
#if DEBUG
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif        

        for (var i = 0; i < QuestStarters.Count; i++)
        {
            var starter = QuestStarters[i];
            if (starter.Check())
            {
                starterId = starter.Id;
                ret.AddRange(starter.QuestToStartIds);
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
    
    public QuestStarterEntity GetQuestStarterById(string id)
    {
        for (var i = 0; i < QuestStarters.Count; i++)
        {
            var starter = QuestStarters[i];
            if (starter.Id == id)
            {
                return starter;
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

        // Cache in appropriate store
        if (quest is StoryQuestEntity)
        {
             ActiveStoryQuests.Add(quest);
        } 
        else if (quest is DailyQuestEntity)
        {
            DailyQuest = (DailyQuestEntity) quest;
        }

        // To handle Pending -> New transition for listeners
        if (saveData == null)
        {
            quest.OnChanged += OnQuestsStateChangedEvent;
            OnActiveQuestsListChanged?.Invoke();
            quest.Start(null);
        }
        else
        {
            quest.Start(saveData);
            quest.OnChanged += OnQuestsStateChangedEvent;
            OnActiveQuestsListChanged?.Invoke();
        }

        return quest;
    }

    /// <summary>
    /// Call it when a player claimed a reward to remove a quest from ActiveQuests list
    /// </summary>
    /// <param name="id"></param>
    public void FinishQuest(string id)
    {
        //Debug.Log("!!! CompleteQuest: " + id);
        
        for (var i = 0; i < ActiveQuests.Count; i++)
        {
            var quest = ActiveQuests[i];
            if (quest.Id == id)
            {
                quest.SetClaimedState();
                
                ActiveQuests.RemoveAt(i);
                
                if (DailyQuest != null && DailyQuest.Id == id)
                {
                    DailyQuest = null;
                }
                else
                {
                    ActiveStoryQuests.Remove(quest);
                    FinishedQuests.Add(id);
                }
                
                quest.DisconnectFromBoard();
                
                quest.OnChanged -= OnQuestsStateChangedEvent;
                
                OnActiveQuestsListChanged?.Invoke();
                
                StartNewQuestsIfAny();
                return;
            }
        }
        
        Debug.LogError($"[QuestsDataManager] => CompleteQuest: Quest with id '{id}' is not active!");
    }

    private void OnQuestsStateChangedEvent(QuestEntity quest, TaskEntity task)
    {
        OnQuestStateChanged?.Invoke(quest, task);
    }

    public bool IsQuestDefined(string id)
    {
        return cache[typeof(QuestEntity)].ContainsKey(id);
    }

    public void StartDailyQuest()
    {
        const string ID = "Daily";
        
        QuestEntity quest = GetActiveQuestById(ID);
        
        if (quest != null)
        {
            quest.ForceComplete();
            FinishQuest(ID);
        }
        
        quest = StartQuestById(ID, null);

        if (ConnectedToBoard)
        {
            quest.ConnectToBoard();
        }
    }
}