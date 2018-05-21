using System.Collections.Generic;
using UnityEngine;

public class TasksDataManager : IDataLoader<TasksDataManager>
{
    public int Delay { get; set; }
    public List<int> Counts { get; set; }
    public List<TaskDef> Defs { get; set; }
    
    public List<Task> Tasks;
    
    public void LoadData(IDataMapper<TasksDataManager> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Delay = data.Delay;
                Counts = data.Counts;
                Defs = data.Defs;
                
                Tasks = new List<Task>();
                
                for (var i = 0; i < Counts[0]; i++)
                {
                    var index = Random.Range(0, Defs.Count);
                    Tasks.Add(new Task(Defs[index]));
                }
            }
            else
            {
                Debug.LogWarning("[TasksDataManager]: tasks config not loaded");
            }
        });
    }
}