using Debug = IW.Logger;
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

public sealed class QuestsDataManager : ECSEntity, IDataManager
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private const int DAILY_TIMER_DELAY = 24 * 60 * 60;
    private const int DAILY_TIMER_START_OFFSET = 60 * 60;
    // private const int SECONDS_IN_MIN = 60;
    // private const int SECONDS_IN_HOUR = SECONDS_IN_MIN * 60;
    // private const int DAILY_TIMER_START_OFFSET = SECONDS_IN_HOUR * 17 + SECONDS_IN_MIN * 1;
    
    public const string DAILY_QUEST_ID = "Daily"; 
    
    public TimerComponent DailyTimer { get; private set; }

    private Dictionary<Type, Dictionary<string, JToken>> cache;
    public Dictionary<Type, Dictionary<string, JToken>> Cache => cache;

    /// <summary>
    /// List of Story-related Quests that currently in progress
    /// </summary>
    public List<QuestEntity> ActiveStoryQuests;
    
    /// <summary>
    /// List of All active quests that handled by the Manager
    /// </summary>
    public List<QuestEntity> ActiveQuests;
    
    public DailyQuestEntity DailyQuest;

    public int DailyQuestRewardIndex { get; private set; }
    public int DailyQuestCompletedCount;
    
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

    private ECSEntity context;

    private long? pendingDailyTimer;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;
        
        Reload();
    }
	
    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
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
        StopDailyTimer();
        
        OnActiveQuestsListChanged = null;
        QuestStarters = null;
        DailyQuest = null;

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
        var questSave = ((GameDataManager)context).UserProfile.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);

        FinishedQuests = questSave.FinishedQuests ?? new List<string>();

        DailyQuestRewardIndex = questSave.DailyQuestRewardIndex;
        DailyQuestCompletedCount = questSave.DailyQuestCompletedCount;
        
        ActiveStoryQuests = new List<QuestEntity>();
        ActiveQuests = new List<QuestEntity>();

        if (questSave.ActiveQuests == null)
        {
            return;
        }


        List<QuestEntity> startedQuests = new List<QuestEntity>();
        foreach (var activeQuest in questSave.ActiveQuests)
        {
            JToken saveData  = activeQuest.Data; 
            JToken questNode = saveData["Quest"];
            string id        = questNode["Id"].Value<string>();
        
            var quest = StartQuestById(id, saveData);
            if (quest != null)
            {
                startedQuests.Add(quest);
            }
        }
        
        if (DailyQuest != null)
        {
            pendingDailyTimer = questSave.DailyTimerStart;
        }

        // Handle migration - case when target is changed
        foreach (var quest in startedQuests)
        {
            quest.ForceCheckActiveTasks(this);
        }
    }

    private void LoadData<T>(string path)
    {
        var dataMapper = new ResourceConfigDataMapper<T>(path, NSConfigsSettings.Instance.IsUseEncryption);
        var json = dataMapper.GetJsonDataAsString();
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

    public T InstantiateFromJson<T>(JToken token) where T : IECSComponent
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
        
        if (DailyQuest != null)
        {
            LocalNotificationsService.Current.UnRegisterNotifier(DailyTimer);
        }
    }
    
#endregion

    public bool StartNewQuestsIfAny()
    {
        const string ACTION_ID = "StartNewQuestsIfAny";
        
        Debug.Log($"[QuestsDataManager] => StartNewQuestsIfAny Schedule");

        DefaultSafeQueueBuilder.BuildAndRun(ACTION_ID, true, () =>
        {
            var questsToStart = CheckConditions(out string starterId);
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

        return true;
    }

    public void StartQuests(HashSet<string> questIds)
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
    public HashSet<string> CheckConditions(out string starterId)
    {
        starterId = null;
        
        if (QuestStarters == null)
        {
            Debug.LogError($"[QuestsDataManager] => CheckConditions() is called when questStarters list is empty");
            return null;
        }

        HashSet<string> ret = new HashSet<string>();
        
#if DEBUG
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif        

        for (var i = 0; i < QuestStarters.Count; i++)
        {
            var starter = QuestStarters[i];
            if (starter.Check())
            {
                starterId = starterId ?? starter.Id;
                foreach (var id in starter.QuestToStartIds)
                {
                    ret.Add(id);
                }
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

        if (quest == null)
        {
            Debug.LogWarning($"[QuestDataManager] => StartQuestById: No quest with id '{id}' defined");
            return null;
        }
        
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

        // Commented out: looks unsafe when loading foreign profile in sync window
        
        // Recreate daily quest if we lost all the tasks due to migration
        // if (DailyQuest != null && DailyQuest.ActiveTasks.Count == 1)
        // {
        //     StartNewDailyQuest();
        // }
        
        // end

        return quest;
    }

    /// <summary>
    /// Call it when a player claimed a reward to remove a quest from ActiveQuests list
    /// </summary>
    /// <param name="id"></param>
    public void FinishQuest(string id)
    {
        for (var i = 0; i < ActiveQuests.Count; i++)
        {
            var quest = ActiveQuests[i];
            if (quest.Id == id)
            {
                bool isClaimedByPlayer = quest.IsClaimed();

                if (!isClaimedByPlayer)
                {
                    quest.SetClaimedState();
                }

                ActiveQuests.RemoveAt(i);
                
                if (DailyQuest != null && DailyQuest.Id == id)
                {
                    if (isClaimedByPlayer)
                    {
                        DailyQuestRewardIndex++;
                    }
                    
                    StopDailyTimer();
                    DailyQuest = null;
                }
                else
                {
                    ActiveStoryQuests.Remove(quest);
                    FinishedQuests.Add(id);
                }

                if (ConnectedToBoard)
                {
                    quest.DisconnectFromBoard();
                }
                
                quest.OnChanged -= OnQuestsStateChangedEvent;
                
                OnActiveQuestsListChanged?.Invoke();

                // Do not trigger conditions check if we have complete DailyQuest (cause issues on DailyQuest restart due to migration)
                if (!(quest is DailyQuestEntity))
                {
                    StartNewQuestsIfAny();
                }

                return;
            }
        }
        
        Debug.LogError($"[QuestsDataManager] => CompleteQuest: Quest with id '{id}' is not active!");
    }

    private void OnQuestsStateChangedEvent(QuestEntity quest, TaskEntity task)
    {
        OnQuestStateChanged?.Invoke(quest, task);
        
        if (task is TaskCompleteDailyTaskEntity && task.IsClaimed())
        {
            DailyQuestCompletedCount++;
        }
    }

    public bool IsQuestDefined(string id)
    {
        return cache[typeof(QuestEntity)].ContainsKey(id);
    }
    
    public bool IsQuestCompleted(string id)
    {
        return !string.IsNullOrEmpty(id) && FinishedQuests.Contains(id);
    }

    public void StartNewDailyQuest()
    {
        Debug.Log($"[QuestsDataManager] => StartDailyQuest");

        QuestEntity quest = GetActiveQuestById(DAILY_QUEST_ID);
        
        if (quest != null)
        {
            quest.ForceComplete();
            FinishQuest(DAILY_QUEST_ID);
        }
        
        quest = StartQuestById(DAILY_QUEST_ID, null);

        if (ConnectedToBoard)
        {
            quest.ConnectToBoard();
            StartDailyTimer();
        }
    }

    public void StartDailyTimer()
    {
        if (!ConnectedToBoard)
        {
            Debug.LogError($"[QuestsDataManager] => StartDailyTimer: Do not call StartDailyTimer before ConnectedToBoard!");
        }
        
        StopDailyTimer();

        DateTime startTime = pendingDailyTimer.HasValue ? UnixTimeHelper.UnixTimestampToDateTime(pendingDailyTimer.Value) : CalculateDailyTimerStartTime();
        pendingDailyTimer = null;
        
        Debug.Log($"[QuestsDataManager] => StartDailyTimer: startTime: {startTime}, delay: {DAILY_TIMER_DELAY}");

        DailyTimer = new TimerComponent
        {
            UseUTC = false,
            Delay = DAILY_TIMER_DELAY,
            Tag = "daily"
        };
        
        DailyTimer.OnComplete += OnCompleteDailyTimer;
                                                                                                                
        RegisterComponent(DailyTimer);
        DailyTimer.Start(startTime);

        LocalNotificationsService.Current.RegisterNotifier(new Notifier(DailyTimer, NotifyType.DailyTimeout));
    }

    public void StopDailyTimer()
    {
        if (DailyTimer == null)
        {
            return;    
        }
        
        Debug.Log($"[QuestsDataManager] => StopDailyTimer");

        UnRegisterComponent(DailyTimer);
        
        DailyTimer.OnComplete -= OnCompleteDailyTimer;
        DailyTimer.Stop();
        
        LocalNotificationsService.Current.UnRegisterNotifier(DailyTimer);
        
        DailyTimer = null;        
    }

    private void OnCompleteDailyTimer()
    {
        Debug.Log($"[QuestsDataManager] => OnCompleteDailyTimer");

        if (DailyQuest != null && DailyQuest.Immortal == false)
        {
            StartNewDailyQuest();
        }
    }

    private DateTime CalculateDailyTimerStartTime()
    {
        // var now = DateTime.Now;
        var secureTime = SecuredTimeService.Current;
        var now = secureTime.Now;
        var todayDayStart = now.TruncDateTimeToDays();
        var todayQuestStart = todayDayStart.AddSeconds(DAILY_TIMER_START_OFFSET);

        if (todayQuestStart > now)
        {
            var fixedQuestStart = todayQuestStart.AddDays(-1);
            return fixedQuestStart;
        }
        
        return todayQuestStart;
        //
        // var nextDay = now.AddSeconds(DAILY_TIMER_DELAY);
        // var nextDayStart = nextDay
        // var nextDailyStart = nextDayStart.AddSeconds(DAILY_TIMER_START_OFFSET);
        // var fromNowToStart = (nextDailyStart - now).TotalSeconds;
        // var elapsed = DAILY_TIMER_DELAY - fromNowToStart;
        // var resultLocal = now.AddSeconds(-elapsed);
        // // var resultUtc = resultLocal.ToUniversalTime();
        //
        // return resultLocal;
    }
    
    public void Cleanup()
    {
        DisconnectFromBoard();
        OnQuestStateChanged = null;
        OnActiveQuestsListChanged = null;

        foreach (var questEntity in ActiveQuests)
        {
            questEntity.Cleanup();
        }
    }
}