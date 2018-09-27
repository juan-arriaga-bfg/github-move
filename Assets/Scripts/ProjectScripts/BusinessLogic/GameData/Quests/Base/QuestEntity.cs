using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quests;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class TaskDef
{
    [JsonProperty] public string TaskId { get; protected set; }
    [JsonProperty] public int Order { get; protected set; }
}

[JsonObject(MemberSerialization.OptIn)]
public class QuestEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public string Id { get; protected set; }

    [JsonProperty] public TaskState State { get; protected set; }

    [JsonProperty] public int ActiveTasksOrder { get; protected set; } = -1;
    
    [JsonProperty] public List<TaskDef> TaskDefs { get; protected set; }
    
    [JsonProperty] public List<TaskEntity> Tasks { get; protected set; } = new List<TaskEntity>();
    
    public List<TaskEntity> ActiveTasks { get; protected set; }
    
    public Action<QuestEntity, TaskEntity> OnChanged;

#region Serialization

    public bool ShouldSerializeTaskDefs()
    {
        return false;
    }
    
    public bool ShouldSerializeTasks()
    {
        return false;
    }
    
#endregion
    
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
}