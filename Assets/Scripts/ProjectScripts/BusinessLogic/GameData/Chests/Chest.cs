using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public enum ChestType
{
    None,
    Common,
    Rare,
    Epic
}

public enum ChestState
{
    Close,
    InProgress,
    Open,
    Finished
}

public class Chest
{
    private ChestDef def;
    private ChestType chestType;
    private ChestState chestState;
    
    private List<List<CurrencyPair>> Rewards;
    
    public DateTime? StartTime { get; set; }
    
    public Chest(ChestDef def)
    {
        this.def = def;
        
        chestType = (ChestType) Enum.Parse(typeof(ChestType), def.Uid);

        Rewards = new List<List<CurrencyPair>>();

        for (int i = 0; i < def.Rewards.Count; i++)
        {
            Rewards.Add(InitReward(i));
        }
    }
    
    public ChestDef Def
    {
        get { return def; }
    }
    
    public ChestType ChestType
    {
        get { return chestType; }
    }

    public int Piece
    {
        get { return PieceType.Parse(def.Uid); }
    }
    
    public ChestState State
    {
        get
        {
            if (StartTime != null && (int) (DateTime.Now - StartTime.Value).TotalSeconds >= def.Time)
            {
                chestState = ChestState.Open;
            }
            
            return chestState;
        }
        set
        {
            if (value == ChestState.InProgress)
            {
                StartTime = DateTime.Now;
            }
            
            chestState = value;
        }
    }

    public CurrencyPair GetReward(int level)
    {
        if (Rewards.Count < level) return null;

        var rewards = Rewards[level];
        
        if (rewards.Count == 0)
        {
            rewards = InitReward(level);
            Rewards[level] = rewards;
        }
        
        var index = Random.Range(0, rewards.Count);
        var reward = rewards[index];
        
        rewards.RemoveAt(index);
        
        return reward;
    }
    
    public string GetSkin()
    {
        switch (chestType)
        {
            case ChestType.Common:
                return "chest_1";
            case ChestType.Rare:
                return "chest_2";
            case ChestType.Epic:
                return "chest_4";
            default:
                return "chest_3";
        }
    }

    private List<CurrencyPair> InitReward(int level)
    {
        var rewards = def.Rewards[level];
            
        var arr = new CurrencyPair[rewards.Count];
            
        rewards.CopyTo(arr);
        rewards = arr.ToList();

        return rewards;
    }
}