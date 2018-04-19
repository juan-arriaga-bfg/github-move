using System.Collections.Generic;
using System.Linq;

public enum ObstacleState
{
    None,
    Lock,
    Unlock,
    InProgres,
    Open,
    Finish
}

public class Obstacle
{
    private readonly ObstacleDef def;
    
    public ObstacleDef Def
    {
        get { return def; }
    }
    
    private List<IObstacleCondition> UnlockConditions { get; set; }
    private List<IObstacleCondition> OpenConditions { get; set; }
    
    public ObstacleState State { get; set; }

    private ObstaclesLogicComponent context;
    
    public Obstacle(ObstacleDef def)
    {
        this.def = def;

        UnlockConditions = InitConditions(def.UnlockConditions);
        OpenConditions = InitConditions(def.OpenConditions);
    }
    
    public bool Check(ObstaclesLogicComponent context)
    {
        this.context = context;
        
        if (State == ObstacleState.Finish)
        {
            return false;
        }
        
        if (State == ObstacleState.Open)
        {
            return true;
        }
        
        bool isDone;

        if (State == ObstacleState.InProgres)
        {
            if (OpenConditions.Count == 0)
            {
                State = ObstacleState.Open;
                return true;
            }
            
            isDone = GetCurrentAndInit(OpenConditions);
            
            State = isDone && OpenConditions.Count == 0 ? ObstacleState.Open : ObstacleState.InProgres;
            return true;
        }
        
        isDone = GetCurrentAndInit(UnlockConditions);
        
        State = isDone && UnlockConditions.Count == 0 ? ObstacleState.Unlock : ObstacleState.Lock;
        return State != ObstacleState.Lock;
    }

    public Chest GetReward()
    {
        return GameDataService.Current.ChestsManager.GetChest(PieceType.Parse(def.Reward));
    }
    
    public int GetUid()
    {
        return def.Uid;
    }
    
    public List<T> GetOpenConditions<T>() where T : class, IObstacleCondition
    {
        var condition = new List<T>();
        var results = OpenConditions.FindAll(c => c is T);

        foreach (var result in results)
        {
            condition.Add(result as T);
        }
        
        return condition;
    }

    private List<IObstacleCondition> InitConditions(List<IObstacleCondition> conditions)
    {
        if(conditions == null) return new List<IObstacleCondition>();
        
        var arr = new IObstacleCondition[conditions.Count];
        
        conditions.CopyTo(arr);
        return arr.ToList();
    }

    private bool GetCurrentAndInit(List<IObstacleCondition> conditions)
    {
        if (conditions.Count == 0)
        {
            return true;
        }
        
        var current = conditions[0];

        if (current.IsInitialized == false)
        {
            current.Init();
        }
        
        var isDone = current.Check(context);

        if (isDone)
        {
            conditions.RemoveAt(0);
        }
        
        return isDone;
    }
}