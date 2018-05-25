using System.Collections.Generic;
using UnityEngine;

public class TasksDataManager : IDataLoader<TasksDataManager>
{
    public int Delay { get; set; }
    public List<int> Counts { get; set; }
    public List<TaskDef> Defs { get; set; }
    
    public List<Task> Tasks;

    public TimerComponent Timer;
    private int updateAmount;
    
    public void LoadData(IDataMapper<TasksDataManager> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Delay = data.Delay;
                Counts = data.Counts;
                Defs = data.Defs;
                
                Timer = new TimerComponent{Delay = data.Delay};
                Timer.OnComplete += Refresh;
                
                ProfileService.Current.RegisterComponent(Timer);
                
                Tasks = new List<Task>();
                
                NextLevel();
            }
            else
            {
                Debug.LogWarning("[TasksDataManager]: tasks config not loaded");
            }
        });
    }

    private int GetCount()
    {
        var level = ProfileService.Current.GetStorageItem(Currency.LevelMarket.Name).Amount;

        return Counts[level];
    }

    public void NextLevel()
    {
        var count = GetCount();
        
        if(Tasks.Count == count) return;
        
        for (var i = 0; i < count; i++)
        {
            var index = Random.Range(0, Defs.Count);
            Tasks.Add(new Task(Defs[index]));
        }
    }
    
    public void Update()
    {
        updateAmount++;
        
        if (Timer.IsStarted) return;
        
        Timer.Start();
    }

    private void Refresh()
    {
        updateAmount--;
        
        var count = GetCount();

        if (Tasks.Count == count)
        {
            updateAmount = 0;
            return;
        }
        
        var index = Random.Range(0, Defs.Count);
        Tasks.Add(new Task(Defs[index]));
        
        if(updateAmount != 0) Timer.Start();
    }
}