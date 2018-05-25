using System.Collections.Generic;
using UnityEngine;

public class TasksDataManager : IDataLoader<TasksDataManager>
{
    public int Delay { get; set; }
    public List<int> TaskCounts { get; set; }
    public List<ItemRange> Ranges { get; set; }
    public List<string> Characters { get; set; }
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
                TaskCounts = data.TaskCounts;
                Ranges = data.Ranges;
                Characters = data.Characters;
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
        var level = GameDataService.Current.LevelsManager.Level - 1;
        
        return TaskCounts[level];
    }

    public void NextLevel()
    {
        var count = GetCount();
        
        if(Tasks.Count == count) return;
        
        for (var i = Tasks.Count; i < count; i++)
        {
            Tasks.Add(CreateTask());
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
        
        Tasks.Add(CreateTask());
        
        if(updateAmount != 0) Timer.Start();
    }

    private Task CreateTask()
    {
        var level = GameDataService.Current.LevelsManager.Level;
        var task = new Task();
        var pricesCount = Ranges[level].GetValue();
        var weights = GetWeights(level);
        var count = 0;
        
        task.Character = Characters[Random.Range(0, Characters.Count)];
        task.Prices = new List<CurrencyPair>();
        task.Rewards = new Dictionary<int, int>();

        for (var i = 0; i < pricesCount; i++)
        {
            var item = ItemWeight.GetRandomItem(weights, true);
            var def = Defs.Find(d => d.Resource == item.Uid);
            var amount = def.Range.GetValue();
            
            task.Prices.Add(new CurrencyPair{Currency = item.Uid, Amount = amount});

            foreach (var pair in def.Rewards)
            {
                var key = PieceType.Parse(pair.Currency);
                int value;
                
                if (task.Rewards.TryGetValue(key, out value) == false)
                {
                    value = pair.Amount * amount;
                    count += value;
                    task.Rewards.Add(key, value);
                    continue;
                }

                value += pair.Amount * amount;
                count += value;
                
                task.Rewards[key] = value;
            }
        }
        
        task.Result = new CurrencyPair{ Currency = PieceType.Parse(PieceType.Coin5.Id), Amount = count};
        
        return task;
    }

    private List<ItemWeight> GetWeights(int level)
    {
        var weights = new List<ItemWeight>();

        foreach (var def in Defs)
        {
            if(def.Level > level) continue;
            
            weights.Add(def.GetItemWeight());
        }
        
        return weights;
    }
}