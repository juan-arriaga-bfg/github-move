using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quests;
using UnityEngine;

public class TaskDef
{
    [JsonProperty] public string TaskId { get; protected set; }
    [JsonProperty] public int Order { get; protected set; }
}

public class QuestEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public string Id { get; protected set; }

    [JsonProperty] public TaskState State { get; protected set; }

    [JsonProperty] public int ActiveTasksOrder { get; protected set; } = -1;
    
    [JsonProperty] public List<TaskDef> TaskDefs { get; protected set; }
    
    [JsonIgnore] public List<TaskEntity> Tasks { get; protected set; } = new List<TaskEntity>();
    
    [JsonIgnore] public List<TaskEntity> ActiveTasks { get; protected set; }
    
    [JsonIgnore] public Action<QuestEntity, TaskEntity> OnChanged;
    
    // overrided to be able to use ShouldSerializeComponentsCache
    [JsonConverter(typeof(ECSEntityJsonConverter))]
    public override Dictionary<int, IECSComponent> ComponentsCache
    {
        get { return componentsCache; }
        set { componentsCache = value; }
    }

#region Serialization

    public bool ShouldSerializeTaskDefs()
    {
        return false;
    }
    
    public bool ShouldSerializeTasks()
    {
        return false;
    }
    
    // public bool ShouldSerializeState()
    // {
    //     return false;
    // }
    
    public bool ShouldSerializeComponentsCache()
    {
        return false;
    }
    
#endregion
    
    public bool IsInProgress()
    {
        return State == TaskState.New || State == TaskState.InProgress;
    }
    
    public bool IsCompleted()
    {
        return State == TaskState.Completed || State == TaskState.Claimed;
    }
    
    public bool IsClaimed()
    {
        return State == TaskState.Claimed;
    }

    private void UpdateState()
    {
        if (IsInProgress())
        {
            foreach (var task in Tasks)
            {
                if (task.State != TaskState.Completed && task.State != TaskState.Claimed)
                {
                    return;
                }
            }
            
            State = TaskState.Completed;
        }
    }
    
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

    private void StartActiveTasks()
    {
        for (int i = 0; i < ActiveTasks.Count; i++)
        {
            var task = ActiveTasks[i];
            task.Start();
        }
    }

    private void SelectActiveTasks()
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

    private void TaskChanged(TaskEntity task)
    {
        Debug.Log("TaskChanged: " + this);
        UpdateState();
        OnChanged?.Invoke(this, task);
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
    
    public QuestSaveData GetDataForSerialization()
    {
        return new QuestSaveData
        {
            Quest = this,
            Tasks = Tasks
        };
    }

    public void Load(JToken json)
    {
        JToken quest = json["Quest"];
        JToken tasks = json["Tasks"];

        quest.PopulateObject(this);
        
        foreach (var node in tasks)
        {
            string id = node.SelectToken("Id").Value<string>();
            TaskEntity task = GetTaskById(id);
            node.PopulateObject(task);
        }
    }

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
}