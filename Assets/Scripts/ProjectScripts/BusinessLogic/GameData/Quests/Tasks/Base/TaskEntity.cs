using System;
using Newtonsoft.Json;
using Quests;

public abstract class TaskEntity : ECSEntity, IECSSerializeable
{
    [JsonProperty(Order = -1000)] 
    public string Id { get; protected set; }

    [JsonProperty(Order = -900)]
    public TaskState State { get; protected set; }

    [JsonProperty] public int Order;
    
#region Serialization

    public bool ShouldSerializeOrder()
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
    
    public Action<TaskEntity> OnChanged;

    protected abstract bool Check();

    public virtual void Start()
    {
        if (State == TaskState.Pending)
        {
            State = TaskState.New;
        }
    }

    public virtual void ConnectToBoard()
    {
        
    }

    public virtual void DisconnectFromBoard()
    {
        
    }
}