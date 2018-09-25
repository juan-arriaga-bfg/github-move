using System;
using Newtonsoft.Json;
using Quests;

public abstract class TaskEntity : ECSEntity, IECSSerializeable
{
    [JsonProperty] public string Id { get; protected set; }

    [JsonProperty] public int Order;

    [JsonProperty] public TaskState State { get; protected set; }

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

        var connectedToBoard = this as IConnectedToBoardEvent;
        connectedToBoard?.ConnectToBoard();
    }

    public void Cleanup()
    {
        var connectedToBoard = this as IConnectedToBoardEvent;
        connectedToBoard?.DisconnectFromBoard();
    }
}