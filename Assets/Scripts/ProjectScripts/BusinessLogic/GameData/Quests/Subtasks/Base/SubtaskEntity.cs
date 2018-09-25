using System;
using Newtonsoft.Json;
using Quests;

public abstract class SubtaskEntity : ECSEntity, IECSSerializeable
{
    [JsonProperty] public string Id;

    [JsonProperty] public SubtaskState State { get; protected set; }
    
    public Action<ECSEntity> OnChanged;

    protected abstract bool Check();

    public virtual void Start()
    {
        State = SubtaskState.InProgress;
    }
}