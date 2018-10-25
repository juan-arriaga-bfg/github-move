using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ChestState
{
    Close = 0,
    InProgress,
    Open,
    Finished
}

public class Chest
{
    private readonly ChestDef def;
    private ChestState chestState;
    
    public DateTime? StartTime { get; set; }
    private DateTime completeTime;

    private int rewardCount;
    private Dictionary<int, int> reward = new Dictionary<int, int>();
    
    public Chest(ChestDef def)
    {
        this.def = def;
    }

    public Dictionary<int, int> Reward
    {
        get { return GetRewardPieces(); }
        set { reward = value; }
    }

    public int RewardCount
    {
        get { return rewardCount; }
        set { rewardCount = value; }
    }

    public ChestDef Def => def;

    public int Piece => def.Piece;

    public int MergePoints => 100;

    public ChestState State
    {
        get
        {
            if (StartTime != null && (int) (DateTime.UtcNow - StartTime.Value).TotalSeconds >= def.Time)
            {
                chestState = ChestState.Open;
            }
            
            return chestState;
        }
        set
        {
            switch (value)
            {
                case ChestState.Close:
                    StartTime = null;
                    break;
                case ChestState.InProgress:
                    SetStartTime(DateTime.UtcNow);
                    break;
            }

            chestState = value;
        }
    }

    public bool CheckStorage()
    {
        
        var currentRewardCount = Reward.Values.Sum();
        Debug.LogError($"{currentRewardCount}/{rewardCount}");
        return currentRewardCount < rewardCount;
    }
    
    public void SetStartTime(DateTime time)
    {
        StartTime = time;
        completeTime = time.AddSeconds(def.Time);
    }
    
    public Dictionary<int, int> GetRewardPieces()
    {
        if (reward.Count != 0) return reward;
        
        var hard = GameDataService.Current.LevelsManager.GetSequence(Currency.Level.Name);
        var sequence = GameDataService.Current.ChestsManager.GetSequence(def.Uid);

        var productionAmount = def.ProductionAmount.Range();
        reward = hard.GetNextDict(productionAmount);
        reward = sequence.GetNextDict(def.PieceAmount, reward);

        rewardCount = productionAmount + def.PieceAmount;
        
        return reward;
    }
}