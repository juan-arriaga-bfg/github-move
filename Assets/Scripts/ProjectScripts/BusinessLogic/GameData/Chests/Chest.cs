using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private ChestState chestState;
    
    public DateTime? StartTime { get; set; }
    private DateTime completeTime;
    
    public Chest(ChestDef def)
    {
        this.def = def;
    }
    
    public ChestDef Def
    {
        get { return def; }
    }
    
    public string Currency
    {
        get { return def.Uid; }
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
            if (value == ChestState.InProgress)
            {
                StartTime = DateTime.Now;
                completeTime = DateTime.Now.AddSeconds(def.Time);
            }
            
            chestState = value;
        }
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
        
        return str;
    }
    
    public string GetTimeLeftText()
    {
        return TimeFormat(completeTime - DateTime.Now);
    }

    private string TimeFormat(TimeSpan time)
    {
        return (int) time.TotalHours > 0 ? string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds) : string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
    }
}