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

    private readonly CurrencyPair price;
    private Dictionary<int, int> reward = new Dictionary<int, int>();
    
    public Chest(ChestDef def)
    {
        this.def = def;
        price = new CurrencyPair {Currency = this.def.Price.Currency, Amount = this.def.Price.Amount};
    }

    public Dictionary<int, int> Reward
    {
        get { return GetRewardPieces(); }
        set { reward = value; }
    }

    public ChestDef Def => def;
    public string Currency => def.Uid;

    public CurrencyPair Price
    {
        get
        {
            price.Amount = Mathf.Clamp((int) (def.Price.Amount * GetTimerProgress()), 1, def.Price.Amount);
            
            return price;
        }
    }

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
        return rewardCount < (def.PieceAmount + def.HardPieceAmount);
    }

    public void SetStartTime(DateTime time)
    {
        StartTime = time;
        completeTime = time.AddSeconds(def.Time);
    }

    public Dictionary<int, int> GetRewardPieces()
    {
        if (reward.Count != 0) return reward;
        
        var max = def.PieceAmount;
        var hard = def.GetHardPieces();
        
        reward = new Dictionary<int, int>(hard);
        
        for (var i = 0; i < max; i++)
        {
            var random = ItemWeight.GetRandomItem(def.PieceWeights).Piece;

            if (random == PieceType.Empty.Id)
            {
                random = def.GetRandomPiece();
            }
            
            if(random == PieceType.None.Id) continue;

            if (reward.ContainsKey(random))
            {
                reward[random] += 1;
                continue;
            }
            
            reward.Add(random, 1);
        }
        
        return reward;
    }

    public float GetTimerProgress()
    {
        return StartTime == null ? 1 : Mathf.Clamp01((int)(completeTime - DateTime.UtcNow).TotalSeconds/(float)def.Time);
    }
    
    public string GetTimeText()
    {
        var time = new TimeSpan(0, 0, def.Time);
        var str = "";

        if (time.Hours > 0)
        {
            str += $"{time.Hours}h";
        }

        if (time.Minutes > 0)
        {
            str += $"{(string.IsNullOrEmpty(str) ? "" : " ")}{time.Minutes}m";
        }

        if (time.Seconds > 0)
        {
            str += $"{(string.IsNullOrEmpty(str) ? "" : " ")}{time.Seconds}s";
        }
        
        return str;
    }
    
    public string GetTimeLeftText()
    {
        return TimeFormat(completeTime - DateTime.UtcNow);
    }

    private string TimeFormat(TimeSpan time)
    {
        if ((int) time.TotalSeconds < 0) return "00:00";

        return (int) time.TotalHours > 0
            ? $"<mspace=2.75em>{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}</mspace>"
            : $"<mspace=2.75em>{time.Minutes:00}:{time.Seconds:00}</mspace>";
    }
}