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
        var rewardCount = Reward.Values.Sum();
        return rewardCount < def.PieceAmount;
    }

    public void SetStartTime(DateTime time)
    {
        StartTime = time;
        completeTime = time.AddSeconds(def.Time);
    }

    public Dictionary<int, int> GetRewardPieces()
    {
        if (reward.Count != 0) return reward;
        
        var sequence = GameDataService.Current.ChestsManager.GetSequence(def.Uid);
        
        reward = sequence.GetNextDict(def.PieceAmount);
        
        return reward;
    }

    public float GetTimerProgress()
    {
        return StartTime == null ? 1 : Mathf.Clamp01((int)(completeTime - DateTime.UtcNow).TotalSeconds/(float)def.Time);
    }
}