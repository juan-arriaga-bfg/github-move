using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quests;
using UnityEngine;

public class TaskDef
{
    [JsonProperty] public string TaskId { get; protected set; }
    [JsonProperty] public int Order { get; protected set; }
}

/// <summary>
/// Represents Quest that contains one or more Tasks
/// </summary>
public class QuestEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public string Id { get; protected set; }

    [JsonProperty] public TaskState State { get; protected set; }

    /// <summary>
    /// Index (order) of active tasks group
    /// </summary>
    [JsonProperty] public int ActiveTasksOrder { get; protected set; } = -1;
    
    [JsonProperty] public List<TaskDef> TaskDefs { get; protected set; }
    
    /// <summary>
    /// All the tasks included to quests
    /// </summary>
    [JsonIgnore] public List<TaskEntity> Tasks { get; protected set; } = new List<TaskEntity>();
    
    /// <summary>
    /// List of tasks that active currently
    /// </summary>
    [JsonIgnore] public List<TaskEntity> ActiveTasks { get; protected set; } = new List<TaskEntity>();
    
    /// <summary>
    /// Will be called every time when Quest state or state of any active Task is changing.
    /// </summary>
    [JsonIgnore] public Action<QuestEntity, TaskEntity> OnChanged;

    /// <summary>
    /// List of tasks that loaded their state from player profile
    /// </summary>
    protected List<TaskEntity> tasksLoadedFromSave = new List<TaskEntity>();

#region Serialization

    // overrided to be able to use ShouldSerializeComponentsCache
    [JsonConverter(typeof(ECSEntityJsonConverter))]
    public override Dictionary<int, IECSComponent> ComponentsCache
    {
        get { return componentsCache; }
        set { componentsCache = value; }
    }
    
    public bool ShouldSerializeTaskDefs()
    {
        return false;
    }
    
    public bool ShouldSerializeTasks()
    {
        return false;
    }
    
    public bool ShouldSerializeComponentsCache()
    {
        return false;
    }

    public virtual QuestSaveData GetDataForSerialization()
    {
        return new QuestSaveData
        {
            Quest = this,
            Tasks = Tasks
        };
    }

    // Load quest/tasks progress from json
    private void Load(JToken json)
    {
        JToken quest = json["Quest"];
        JToken tasks = json["Tasks"];

        quest.PopulateObject(this);
        
        foreach (var node in tasks)
        {
            string id = node.SelectToken("Id").Value<string>();
            TaskEntity task = GetTaskById(id);

            Debug.LogError($"[QuestEntity] => Load: Quest '{Id}': No task with id '{id}' defined");
            
            node.PopulateObject(task);
            
            tasksLoadedFromSave.Add(task);
        }
    }
    
#endregion
    
    public bool IsInProgress()
    {
        return State == TaskState.New || State == TaskState.InProgress;
    }
    
    public bool IsCompletedOrClaimed()
    {
        return State == TaskState.Completed || State == TaskState.Claimed;
    }
    
    public bool IsClaimed()
    {
        return State == TaskState.Claimed;
    }
    
    public bool IsCompleted()
    {
        return State == TaskState.Completed;
    }

    protected virtual void UpdateState()
    {
        if (IsInProgress())
        {
            bool isAllTasksCompleted = true;
            
            foreach (var task in Tasks)
            {
                isAllTasksCompleted = isAllTasksCompleted && task.IsCompletedOrClaimed();

                if (task.State == TaskState.InProgress && State == TaskState.New)
                {
                    State = TaskState.InProgress;
                }
            }

            if (isAllTasksCompleted)
            {
                State = TaskState.Completed;
            }
        }
    }
    
    /// <summary>
    /// Start quest (and load previous state if any)
    /// </summary>
    public virtual void Start(JToken saveData)
    {
        if (saveData != null)
        {
            Load(saveData);
        }
        
        if (State == TaskState.Pending)
        {
            State = TaskState.New;
        }

        SelectActiveTasks();
        StartActiveTasks();
        
        Debug.Log($"{(saveData != null ? "LOADED" : "STARTED")} {this}");
    }
    
    public void DisconnectFromBoard()
    {
        if (ActiveTasks == null)
        {
            return;
        }
        
        for (int i = 0; i < ActiveTasks.Count; i++)
        {
            var task = ActiveTasks[i];
            task.DisconnectFromBoard();
        }
    }
    
    public void ConnectToBoard()
    {
        if (ActiveTasks == null)
        {
            return;
        }
        
        for (int i = 0; i < ActiveTasks.Count; i++)
        {
            var task = ActiveTasks[i];
            task.ConnectToBoard();
        }
    }

    protected virtual void StartActiveTasks()
    {
        for (int i = 0; i < ActiveTasks.Count; i++)
        {
            var task = ActiveTasks[i];
            task.Start();
        }
    }

    protected virtual void SelectActiveTasks()
    {
        ActiveTasks = new List<TaskEntity>();
        SortTasks();

        ActiveTasks = new List<TaskEntity>();

        if (ActiveTasksOrder < 0)
        {
            ActiveTasksOrder = Tasks[0].Order;
        }
        
        for (int i = 0; i < Tasks.Count; i++)
        {
            var task = Tasks[i];
            if (task.Order == ActiveTasksOrder)
            {
                ActiveTasks.Add(task); 
            }
        }
    }

    public override ECSEntity RegisterComponent(IECSComponent component, bool isCollection = false)
    {
        TaskEntity task = component as TaskEntity;
        if (task != null)
        {
            Tasks.Add(task);
            task.OnChanged += TaskChanged;
        }
        
        return base.RegisterComponent(component, isCollection);
    }

    protected virtual void TaskChanged(TaskEntity task)
    {
        UpdateState();
        OnChanged?.Invoke(this, task);
        Debug.Log("TaskChanged: " + this);
    }
    
    private void SortTasks()
    {
        Tasks.Sort((task1, task2) => { return task1.Order - task2.Order;});
    }

    public override string ToString()
    {
        string ret = $"QUEST: {Id}, State: {State}, ActiveTasksOrder: {ActiveTasksOrder}, Tasks: {Tasks?.Count}, ActiveTasks: {ActiveTasks?.Count}";

        if (ActiveTasks != null)
        {
            foreach (var task in ActiveTasks)
            {
                ret += $"\n    [{task.Order}] " + task.ToString();
            }
        }

        return ret;
    }

    public TaskEntity GetTaskById(string id)
    {
        for (var i = 0; i < Tasks.Count; i++)
        {
            var task = Tasks[i];
            if (task.Id == id)
            {
                return task;
            }
        }

        return null;
    }
    
    public T GetTask<T>() where T : TaskEntity
    {
        for (var i = 0; i < Tasks.Count; i++)
        {
            var task = Tasks[i] as T;
            if (task != null)
            {
                return task;
            }
        }

        return null;
    }

    /// <summary>
    /// Call it when player take a reward
    /// </summary>
    public void SetClaimedState()
    {
        if (State == TaskState.Claimed)
        {
            return;
        }
        
        if (State != TaskState.Completed)
        {
            throw new ArgumentException($"Can't set Claimed state when quest is in '{State}' state!");
        }
        
        State = TaskState.Claimed;
        
        OnChanged?.Invoke(this, null);
    }
    
    public void SetInProgressState()
    {
        if (State == TaskState.InProgress)
        {
            return;
        }
        
        if (State != TaskState.New && State != TaskState.Pending)
        {
            throw new ArgumentException($"Can't set InProgress state when quest is in '{State}' state!");
        }
        
        State = TaskState.InProgress;
        
        OnChanged?.Invoke(this, null);
    }

    /// <summary>
    /// Used for debug purposes
    /// </summary>
    public void ForceComplete()
    {
        if (State != TaskState.Claimed)
        {
            State = TaskState.Completed;
            OnChanged?.Invoke(this, null);
        }
    }

    public int GetActiveTasksCount()
    {
        return ActiveTasks.Count;
    }
    
    public int GetCompletedTasksCount()
    {
        int count = 0;
        for (var i = 0; i < ActiveTasks.Count; i++)
        {
            if (ActiveTasks[i].IsCompletedOrClaimed())
            {
                count += 1;
            }
        }
        return count;
    }
    
    public int GetClaimedTasksCount()
    {
        int count = 0;
        for (var i = 0; i < ActiveTasks.Count; i++)
        {
            if (ActiveTasks[i].IsClaimed())
            {
                count += 1;
            }
        }
        return count;
    }
    
    public int GetCompletedButNotClaimedTasksCount()
    {
        int count = 0;
        for (var i = 0; i < ActiveTasks.Count; i++)
        {
            if (ActiveTasks[i].IsCompleted())
            {
                count += 1;
            }
        }
        return count;
    }
    
    public void Cleanup()
    {
        DisconnectFromBoard();
        OnChanged = null;
        
        if (ActiveTasks == null)
        {
            return;
        }
        
        for (int i = 0; i < ActiveTasks.Count; i++)
        {
            var task = ActiveTasks[i];
            task.Cleanup(); 
        }
    }
}