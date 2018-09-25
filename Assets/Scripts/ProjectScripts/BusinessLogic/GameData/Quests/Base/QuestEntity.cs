using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Quests;

public class TaskDef
{
    public string TaskId { get; protected set; }
    public int Order { get; protected set; }
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
    
    private readonly List<TaskEntity> tasks = new List<TaskEntity>();
    private List<TaskEntity> activeTasks;
    
    public Action<QuestEntity, TaskEntity> OnChanged;

    public virtual void Start()
    {
        if (State == TaskState.Pending)
        {
            State = TaskState.New;
        }

        SelectActiveTasks();
        StartActiveTasks();
    }

    private void StartActiveTasks()
    {
        for (int i = 0; i < activeTasks.Count; i++)
        {
            var task = activeTasks[i];
            task.Start();
        }
    }

    private void SelectActiveTasks()
    {
        activeTasks = new List<TaskEntity>();
        SortTasks();

        activeTasks = new List<TaskEntity>();

        if (ActiveTasksOrder < 0)
        {
            ActiveTasksOrder = tasks[0].Order;
        }
        
        for (int i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];
            if (task.Order == ActiveTasksOrder)
            {
                activeTasks.Add(task); 
            }
        }
    }

    public override ECSEntity RegisterComponent(IECSComponent component, bool isCollection = false)
    {
        TaskEntity task = component as TaskEntity;
        if (task != null)
        {
            tasks.Add(task);
            task.OnChanged += TaskChanged;
        }
        
        return base.RegisterComponent(component, isCollection);
    }

    private void TaskChanged(TaskEntity task)
    {
        OnChanged(this, task);
    }

    private void SortTasks()
    {
        tasks.Sort((task1, task2) => { return task1.Order - task2.Order;});
    }

    public override string ToString()
    {
        string ret = $"QUEST: {Id}, State: {State}, ActiveTasksOrder: {ActiveTasksOrder}, Tasks: {tasks?.Count}, ActiveTasks: {activeTasks?.Count}";

        if (activeTasks != null)
        {
            foreach (var task in activeTasks)
            {
                ret += $"\n    [{task.Order}] " + task.ToString();
            }
        }

        return ret;
    }
}