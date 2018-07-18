using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TasksDataManager : ECSEntity, IDataManager, IDataLoader<TasksDataManager>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }
	
    public override void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public int Delay { get; set; }
    public List<int> TaskCounts { get; set; }
    public List<ItemRange> Ranges { get; set; }
    public List<string> Characters { get; set; }
    public List<TaskDef> Defs { get; set; }
    
    public List<Task> Tasks;

    public TimerComponent Timer;
    private int updateAmount;

    private bool isStart;

    public void Reload()
    {
        return;
        TaskCounts = null;
        Ranges = null;
        Characters = null;
        Defs = null;
        Tasks = null;
        
        if (Timer != null)
        {
            Timer.Stop();
            UnRegisterComponent(Timer);
            Timer = null;
        }
        
        LoadData(new ResourceConfigDataMapper<TasksDataManager>("configs/tasks.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadData(IDataMapper<TasksDataManager> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                return;
                Delay = data.Delay;
                TaskCounts = data.TaskCounts;
                Ranges = data.Ranges;
                Characters = data.Characters;
                Defs = data.Defs;
                
                Timer = new TimerComponent{Delay = data.Delay};
                Timer.OnComplete += Refresh;
                RegisterComponent(Timer);
                
                Tasks = new List<Task>();

                isStart = true;
                NextLevel();
                isStart = false;
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    private int GetCount()
    {
        return 0;
        var level = GameDataService.Current.LevelsManager.Level - 1;
        
        return TaskCounts[level];
    }

    public void NextLevel()
    {
        return;
        var count = GetCount();
        
        if(Tasks.Count == count) return;
        
        for (var i = Tasks.Count; i < count; i++)
        {
            Tasks.Add(CreateTask());
        }
    }
    
    public void Update()
    {
        return;
        updateAmount++;
        
        if (Timer.IsStarted) return;
        
        Timer.Start();
    }

    private void Refresh()
    {
        return;
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
    
    public void CheckLast()
    {
        return;
        if(Tasks.Count != 0) return;
        
        Tasks.Add(CreateTask());
    }

    private Task CreateTask()
    {
        return null;
        Task task = null;
        
        if (isStart) task = CreateFromSave();
        if (task != null) return task;
        
        task = new Task();
        
        var level = GameDataService.Current.LevelsManager.Level - 1;
        var pricesCount = Ranges[level].GetValue();
        var weights = GetWeights(level);
        
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
                    task.Rewards.Add(key, value);
                    continue;
                }

                value += pair.Amount * amount;
                
                task.Rewards[key] = value;
            }
        }

        task.Rewards = CurrencyHellper.MinimizeCoinPieces(task.Rewards);
        task.Result = new CurrencyPair{ Currency = PieceType.Parse(PieceType.Coin5.Id), Amount = task.Rewards.Sum(pair => pair.Value)};
        
        return task;
    }

    private Task CreateFromSave()
    {
        return null;
        var data = ProfileService.Current.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);

        if (data.Market == null || data.Market.Count == 0) return null;

        var save = data.Market[0];
        
        data.Market.RemoveAt(0);
        
        var task = new Task
        {
            Character = Characters[Random.Range(0, Characters.Count)],
            Prices = save.Prices,
            Rewards = save.Rewards,
            Result = new CurrencyPair {Currency = PieceType.Parse(PieceType.Coin5.Id), Amount = save.Result}
        };
        
        return task;
    }

    private List<ItemWeight> GetWeights(int level)
    {
        var weights = new List<ItemWeight>();
        
        return weights;

        foreach (var def in Defs)
        {
            if(def.Level > level + 1) continue;
            
            weights.Add(def.GetItemWeight());
        }
        
        return weights;
    }
}