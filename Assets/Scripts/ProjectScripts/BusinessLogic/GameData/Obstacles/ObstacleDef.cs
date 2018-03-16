using System.Collections.Generic;

public class ObstacleDef
{
    public int Uid { get; set; }
    public BoardPosition Position { get; set; }
    public List<IObstacleCondition> UnlockConditions { get; set; }
    public List<IObstacleCondition> OpenConditions { get; set; }
    public ChestType Reward { get; set; }
    public CurrencyPair Price { get; set; }
    
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
    
    public override string ToString()
    {
        var str = string.Format("Uid: {0}, Position: {1}, Reward: {2}, Price: {3} - {4}", Uid, Position, Reward, Price.Currency, Price.Amount);

        str += "\nUnlockCondition:\n[";

        for (var i = 0; i < UnlockConditions.Count; i++)
        {
            var unlock = UnlockConditions[i];
            str += string.Format("{0} - {1}, ", i, unlock);
        }
        
        str += "]\nOpenCondition:\n[";
        
        for (var i = 0; i < OpenConditions.Count; i++)
        {
            var open = OpenConditions[i];
            str += string.Format("{0} - {1}, ", i, open);
        }
        
        str += "]";
        
        return str;
    }
}