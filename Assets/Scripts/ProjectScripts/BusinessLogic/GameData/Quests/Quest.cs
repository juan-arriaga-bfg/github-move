using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    private QuestDef def;

    private int currentAmount;
    
    public Quest(QuestDef def)
    {
        this.def = def;
        WantedPiece = PieceType.Parse(def.Price.Currency);
    }
    
    public QuestDef Def
    {
        get { return def; }
    }
    
    public int WantedPiece { get; private set; }
    
    public string WantedIcon
    {
        get { return PieceType.Parse(WantedPiece); }
    }
    
    public int TargetAmount
    {
        get { return def.Price.Amount; }
    }

    public int CurrentAmount
    {
        get { return currentAmount; }
        set { currentAmount = Mathf.Min(value, def.Price.Amount); }
    }

    public Dictionary<int, int> Rewards
    {
        get
        {
            var reward = def.Rewards.Find(pair => pair.Currency == Currency.Coins.Name);

            return reward == null ? new Dictionary<int, int>() : CurrencyHellper.CurrencyToCoinPieces(reward.Amount);
        }
    }

    public bool Check()
    {
        return CurrentAmount >= TargetAmount;
    }

    public bool IsActive()
    {
        return def.Conditions.Find(condition => condition.Check() == false) == null;
    }
}