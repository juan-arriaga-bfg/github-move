using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Quests;
using UnityEngine;

/// <summary>
/// Base class fot Task
/// </summary>
public abstract class TaskEntity : ECSEntity, IECSSerializeable
{
    [JsonProperty(Order = -1000)] 
    public string Id { get; protected set; }

    [JsonProperty(Order = -900)]
    public TaskState State { get; protected set; }

    [JsonProperty] public int Order;
    
    [JsonProperty] public TaskGroup Group;
       
#region Serialization

    // overrided to be able to use ShouldSerializeComponentsCache
    [JsonProperty]
    [JsonConverter(typeof(ECSEntityJsonConverter))]
    public override Dictionary<int, IECSComponent> ComponentsCache
    {
        get { return componentsCache; }
        set { componentsCache = value; }
    }
    
    public bool ShouldSerializeOrder()
    {
        return false;
    }
    
    public bool ShouldSerializeComponentsCache()
    {
        return false;
    }
    
    public bool ShouldSerializeGroup()
    {
        return false;
    }
    
#endregion

    private QuestDescriptionComponent cachedQuestDescription;
    
    /// <summary>
    /// Return true when Task actually waiting for specified changes in the game
    /// </summary>
    public bool IsInProgress()
    {
        return State == TaskState.New || State == TaskState.InProgress;
    }
    
    /// <summary>
    /// Return true when all required actions are done by the player and game can provide a reward for this task (or a reward already provided).
    /// In this state task will not react to corresponded events in the game anymore.
    /// </summary>
    public bool IsCompletedOrClaimed()
    {
        return State == TaskState.Completed || State == TaskState.Claimed;
    }
    
    /// <summary>
    /// Return true if a player claimed a reward for this task
    /// </summary>
    /// <returns></returns>
    public bool IsClaimed()
    {
        return State == TaskState.Claimed;
    }
    
    /// <summary>
    /// Return true if a player completed the task but nob claimed a reward
    /// </summary>
    /// <returns></returns>
    public bool IsCompleted()
    {
        return State == TaskState.Completed;
    }
    
    /// <summary>
    /// Will be invoked avery time when state (or some counter) is changing
    /// </summary>
    public Action<TaskEntity> OnChanged;

    protected abstract bool Check();

    /// <summary>
    /// Until we call this, Task will not react to any events in the game 
    /// </summary>
    public virtual void Start()
    {
        if (State == TaskState.Pending)
        {
            State = TaskState.New;
            OnChanged?.Invoke(this);
        }
    }
    
    /// <summary>
    /// Mark that reward is provided to player
    /// </summary>
    public void SetClaimedState()
    {
        if (State == TaskState.Claimed)
        {
            return;
        }
        
        if (State != TaskState.Completed)
        {
            throw new ArgumentException($"Can't set Claimed state when task is in '{State}' state!");
        }
        
        State = TaskState.Claimed;
        
        OnChanged?.Invoke(this);
    }

    /// <summary>
    /// Should be called if task will listen Board Events
    /// </summary>
    public virtual void ConnectToBoard()
    {
        
    }

    /// <summary>
    /// Should be called if task will listen Board Events
    /// </summary>
    public virtual void DisconnectFromBoard()
    {
        
    }

    /// <summary>
    /// Show hint for a player to describe how this task can be completed
    /// </summary>
    public virtual void Highlight()
    {
        var attributes = GetType().GetCustomAttributes(typeof(TaskHighlight), inherit: false);
        if (attributes.Length == 0)
        {
            Debug.LogError($"[TaskEntity] => TaskHighlight attribute not found for {GetType()} class.");
        }
        else
        {
            for (var i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                var attr  = attribute as TaskHighlight;
                if (attr == null)
                {
                    continue;
                }

                if (attr.ConditionTypes != null)
                {
                    bool conditionFailed = false;
                    foreach (Type conditionType in attr.ConditionTypes)
                    {
                        object condInstance = Activator.CreateInstance(conditionType);
                        ITaskHighlightCondition cond = condInstance as ITaskHighlightCondition;
                        if (cond == null || !cond.Check(this))
                        {
                            conditionFailed = true;
                            break;
                        }
                    }

                    if (conditionFailed)
                    {
                        continue;
                    }
                }

                var hlType = attr.HighlightType;
                object hlInstance = Activator.CreateInstance(hlType);
                ITaskHighlight hl = hlInstance as ITaskHighlight;

                // ReSharper disable once PossibleNullReferenceException
                if (hl.Highlight(this))
                {
                    return;
                }
            }
        }
    }

    protected override ECSEntity RegisterComponentDeserializedInternal(IECSComponent component)
    {
        QuestDescriptionComponent descriptionComponent = component as QuestDescriptionComponent;
        if (descriptionComponent != null)
        {
            cachedQuestDescription = descriptionComponent;
        }
        
        return base.RegisterComponentDeserializedInternal(component);
    }

    public virtual string GetLocalizedMessage()
    {
        string key = cachedQuestDescription?.Message;
        return LocalizationService.Get(key, key) ;
    }
    
    public virtual string GetLocalizedTitle()
    {
        string key = cachedQuestDescription?.Title;
        return LocalizationService.Get(key, key) ;
    }
    
    public virtual string GetIco()
    {
        return  cachedQuestDescription?.Ico;
    }
    
    public virtual string GetLocalizedHint()
    {
        string key = cachedQuestDescription?.Hint;
        if (string.IsNullOrEmpty(key))
        {
            return string.Empty;
        }
        
        return LocalizationService.Get(key, key);
    }
}