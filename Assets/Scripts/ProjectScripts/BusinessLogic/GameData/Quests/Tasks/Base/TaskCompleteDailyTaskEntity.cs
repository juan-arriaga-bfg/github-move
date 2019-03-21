using Debug = IW.Logger;
using UnityEngine;

/// <summary>
/// Base task for daily tasks 
/// </summary>
[TaskHighlight(typeof(HighlightTaskNotImplemented))]
public class TaskCompleteDailyTaskEntity : TaskCounterEntity
{   
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public override void ConnectToBoard()
    {
        GameDataService.Current.QuestsManager.OnQuestStateChanged += OnQuestStateChanged;
    }

    public override void DisconnectFromBoard()
    {
        GameDataService.Current.QuestsManager.OnQuestStateChanged -= OnQuestStateChanged;
    }

    private void OnQuestStateChanged(QuestEntity quest, TaskEntity task)
    {
        if (task == this)
        {
            return;
        }
        
        var questManager = GameDataService.Current.QuestsManager;
        var dailyQuest = questManager.DailyQuest;
        if (dailyQuest == null || quest != dailyQuest)
        {
            return;
        }

        CalculateCurrentValue();
    }

    public void CalculateCurrentValue()
    {
        var questManager = GameDataService.Current.QuestsManager;
        var activeTasks = questManager.DailyQuest.ActiveTasks;

        int current = CurrentValue;
        int completedCount = 0;
        foreach (var taskEntity in activeTasks)
        {
            if (taskEntity.IsCompletedOrClaimed())
            {
                completedCount++;
            }
        }

        if (activeTasks.Count - 1 != TargetValue)
        {
            Debug.LogError($"[TaskCompleteDailyTaskEntity] => OnQuestStateChanged: TargetValue '{TargetValue}' != DailyQuest.Tasks.Count '{activeTasks.Count - 1}'");
        }
        
        if (current >= completedCount)
        {
            return;
        }

        CurrentValue = completedCount;
    }

    public override string ToString()
    {
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: {CurrentValue}/{TargetValue}";
        return ret;
    }

    public void SetTarget(int value)
    {
        TargetValue = value;
    }
}