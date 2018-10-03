using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Quests;

public abstract class TaskEntity : ECSEntity, IECSSerializeable
{
    [JsonProperty(Order = -1000)] 
    public string Id { get; protected set; }

    [JsonProperty(Order = -900)]
    public TaskState State { get; protected set; }

    [JsonProperty] public int Order;
    
    // overrided to be able to use ShouldSerializeComponentsCache
    [JsonProperty]
    [JsonConverter(typeof(ECSEntityJsonConverter))]
    public override Dictionary<int, IECSComponent> ComponentsCache
    {
        get { return componentsCache; }
        set { componentsCache = value; }
    }
    
#region Serialization

    public bool ShouldSerializeOrder()
    {
        return false;
    }
    
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