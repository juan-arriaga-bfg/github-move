using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class DailyQuestEntity : QuestEntity
{
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
        
        // Sort tasks by groups
        Dictionary<TaskGroup,List<TaskEntity>> tasksDict = new Dictionary<TaskGroup, List<TaskEntity>>();
        for (int i = 0; i < Tasks.Count - 1; i++)
        {
            var task = Tasks[i];

            List<TaskEntity> tasks;
            if (!tasksDict.TryGetValue(task.Group, out tasks))
            {
                tasks = new List<TaskEntity>();
                tasksDict.Add(task.Group, tasks);
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
    }
    
    public override QuestSaveData GetDataForSerialization()
    {
        return new QuestSaveData
        {
            Quest = this,
            Tasks = ActiveTasks
        };
    }
}