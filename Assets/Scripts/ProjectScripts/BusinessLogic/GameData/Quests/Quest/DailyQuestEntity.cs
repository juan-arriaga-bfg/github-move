using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Quests;
using UnityEngine;

public class DailyQuestEntity : QuestEntity
{   
    private readonly Dictionary<TaskGroup, int> tasksCount = new Dictionary<TaskGroup, int>
    {
        {TaskGroup.Permanent, 1},
        {TaskGroup.Easy,      0},
        {TaskGroup.Normal,    0},
        {TaskGroup.Hard,      0},
    };

    // Set this to prevent QuestManager from replacing this instance on timer completion
    [JsonIgnore] public bool Immortal;
    
    protected override void SelectActiveTasks()
    {
        ActiveTasksOrder = 0;
        
        ActiveTasks = new List<TaskEntity>();

        // Do not select random task if load saved game
        if (tasksLoadedFromSave.Count > 0)
        {
            ActiveTasks.AddRange(tasksLoadedFromSave);
            return;
        }

        TaskCompleteDailyTaskEntity allClearTask = null;
        
        // Sort tasks by groups
        Dictionary<TaskGroup,List<TaskEntity>> tasksDict = new Dictionary<TaskGroup, List<TaskEntity>>();
        foreach (var key in tasksCount.Keys)
        {
            tasksDict.Add(key, new List<TaskEntity>());
        }
        
        for (int i = 0; i < Tasks.Count - 1; i++)
        {
            var task = Tasks[i];

            List<TaskEntity> tasks;
            if (!tasksDict.TryGetValue(task.Group, out tasks))
            {
                allClearTask = task as TaskCompleteDailyTaskEntity;
                if (allClearTask != null)
                {
                    continue;
                }

                Debug.LogError($"[DailyQuestEntity] => SelectActiveTasks: Task '{task.Id}' have no group specified.");
                continue;
            }
            
            tasks.Add(task);
        }
        
        // Get random tasks
        foreach (var def in tasksCount)
        {
            TaskGroup group = def.Key;
            int count = def.Value;

            List<TaskEntity> tasksPool = tasksDict[group];
            if (count > tasksPool.Count)
            {
                Debug.LogError($"[DailyQuestEntity] => Not enough tasks in pool to fill '{def.Key}' group. At least {count} tasks expected.");
            }
            
            for (int i = 0; i < count && tasksPool.Count > 0; i++)
            {
                int index = UnityEngine.Random.Range(0, tasksPool.Count);
                ActiveTasks.Add(tasksPool[index]);
                tasksPool.RemoveAt(index);
            }
        }

        if (allClearTask != null)
        {
            allClearTask.SetTarget(ActiveTasks.Count);
            ActiveTasks.Add(allClearTask);
        }
        else
        {
            Debug.LogError($"[DailyQuestEntity] => SelectActiveTasks: 'All clear' CompleteDailyTask not added.");
        }
        
    }
    
    public override QuestSaveData GetDataForSerialization()
    {
        return new QuestSaveData
        {
            Quest = this,
            Tasks = ActiveTasks
        };
    }
    
    protected override void StartActiveTasks()
    {
        for (int i = 0; i < ActiveTasks.Count; i++)
        {
            var task = ActiveTasks[i] as TaskCompleteDailyTaskEntity;
            
            if (task != null)
            {
                task.SetTarget(ActiveTasks.Count - 1);
                break;
            }
        }
        
        base.StartActiveTasks();

    }
    
    protected override void UpdateState()
    {
        if (IsInProgress())
        {
            bool isAllTasksCompleted = true;
            
            foreach (var task in ActiveTasks)
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
        else if (IsCompleted())
        {
            bool isAllTasksClaimed = true;
            
            foreach (var task in ActiveTasks)
            {
                isAllTasksClaimed = isAllTasksClaimed && task.IsClaimed();
            }

            if (isAllTasksClaimed)
            {
                State = TaskState.Claimed;
            }
        }
    }

    public bool IsAllTasksClaimed()
    {
        foreach (var task in ActiveTasks)
        {
            if (!task.IsClaimed())
            {
                return false;
            }
        }

        return true;
    }
}