using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class DailyQuestEntity : QuestEntity
{
    // Set this to prevent QuestManager from replacing this instance on timer completion
    public bool Immortal;
    
    private readonly Dictionary<TaskGroup, int> tasksCount = new Dictionary<TaskGroup, int>
    {
        {TaskGroup.Permanent, 2},
        {TaskGroup.Easy,      2},
        {TaskGroup.Normal,    2},
        {TaskGroup.Hard,      1},
    };
    
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
            for (int i = 0; i < count; i++)
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
}