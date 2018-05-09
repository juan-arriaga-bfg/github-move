using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ChestState
{
    Close = 0,
    InProgress,
    Open,
    Finished
}

public class Chest
{
    private ChestDef def;
    private ChestState chestState;
    
    public DateTime? StartTime { get; set; }
    private DateTime completeTime;

    private CurrencyPair price;
    
    public Chest(ChestDef def)
    {
        this.def = def;
        price = new CurrencyPair {Currency = this.def.Price.Currency, Amount = this.def.Price.Amount};
    }
    
    public ChestDef Def
    {
        get { return def; }
    }
    
    public string Currency
    {
        get { return def.Uid; }
    }

    public CurrencyPair Price
    {
        get
        {
            price.Amount = Mathf.Clamp((int) (def.Price.Amount * GetTimerProgress()), 1, def.Price.Amount);
            
            return price;
        }
    }

    public int Piece
    {
        get { return def.Piece; }
    }

    public int MergePoints
    {
        get { return def.MergePoints; }
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
            switch (value)
            {
                case ChestState.Close:
                    StartTime = null;
                    break;
                case ChestState.InProgress:
                    SetStartTime(DateTime.Now);
                    break;
            }

            chestState = value;
        }
    }

    public void SetStartTime(DateTime time)
    {
        StartTime = time;
        completeTime = time.AddSeconds(def.Time);
    }

    public Dictionary<int, int> GetRewardPieces(int level)
    {
        var max = def.PieceAmounts[level];
        var result = new Dictionary<int, int>();

        for (var i = 0; i < max; i++)
        {
            var random = ItemWeight.GetRandomItem(def.PieceWeights).Piece;

            if (result.ContainsKey(random))
            {
                result[random] += 1;
                continue;
            }
            
            result.Add(random, 1);
        }
        
        return result;
    }
    
    public Dictionary<string, int> GetRewardChargers(int level)
    {
        var max = def.ChargerAmounts[level];
        var result = new Dictionary<string, int>();

        for (var i = 0; i < max; i++)
        {
            var random = ItemWeight.GetRandomItem(def.ChargerWeights).Uid;

            if (result.ContainsKey(random))
            {
                result[random] += 1;
                continue;
            }
            
            result.Add(random, 1);
        }
        
        return result;
    }

    public float GetTimerProgress()
    {
        return StartTime == null ? 1 : Mathf.Clamp01((int)(completeTime - DateTime.Now).TotalSeconds/(float)def.Time);
    }
    
    public string GetTimeText()
    {
        var time = new TimeSpan(0, 0, def.Time);
        var str = "";

        if (time.Hours > 0)
        {
            str += string.Format("{0}h", time.Hours);
        }

        if (time.Minutes > 0)
        {
            str += string.Format("{0}{1}m", string.IsNullOrEmpty(str) ? "" : " ", time.Minutes);
        }

        if (time.Seconds > 0)
        {
            str += string.Format("{0}{1}s", string.IsNullOrEmpty(str) ? "" : " ", time.Seconds);
        }
        
        return str;
    }
    
    public string GetTimeLeftText()
    {
        return TimeFormat(completeTime - DateTime.Now);
    }

    private string TimeFormat(TimeSpan time)
    {
        if ((int) time.TotalSeconds < 0) return "00:00";

        return (int) time.TotalHours > 0
            ? string.Format("<mspace=2.75em>{0:00}:{1:00}:{2:00}</mspace>", time.Hours, time.Minutes, time.Seconds)
            : string.Format("<mspace=2.75em>{0:00}:{1:00}</mspace>", time.Minutes, time.Seconds);
    }
}