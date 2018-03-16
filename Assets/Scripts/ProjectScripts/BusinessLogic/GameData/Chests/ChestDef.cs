using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ChestState
{
    None,
    Lock,
    InProgres,
    Open
}

public enum ChestType
{
    None,
    Common,
    Rare,
    Epic
}

public class ChestDef
{
    public string Uid { get; set; }
    public int Time { get; set; }
    
    public CurrencyPair Price { get; set; }
    public List<CurrencyPair> Rewards { get; set; }
    public List<CurrencyPair> RewardsAlternative { get; set; }

    public DateTime? StartTime { get; set; }
    
    private ChestType chestType;
    
    

    public ChestState State { get; set; }
    
    public ChestType GetChestType()
    {
        if (chestType != ChestType.None) return chestType;
        return (ChestType) Enum.Parse(typeof(ChestType), Uid);
    }

    public string GetSkin()
    {
        switch (GetChestType())
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

    public List<CurrencyPair> GetRewards()
    {
        var rewards = new List<CurrencyPair>();

        for (var i = 0; i < Rewards.Count; i++)
        {
            var random = Random.Range(0, 2);
            
            rewards.Add(random == 0 ? Rewards[i] : RewardsAlternative[i]);
        }
        
        return rewards;
    }
    
    public string GetOpenTime()
    {
        var time = new TimeSpan(0, 0, Time);

        if ((int)time.TotalHours > 0) return string.Format("{0}h {1}m", time.Hours, time.Minutes);
        
        return string.Format("{0}m", time.Minutes);
    }

    public TimeSpan GetCurrentTimeInTimeSpan()
    {
        var finish = new TimeSpan(0, 0, Time);
        return finish - (DateTime.Now - StartTime.Value);
    }

    public string GetCurrentTime()
    {
        var time = GetCurrentTimeInTimeSpan();
        var str = "";

        if ((int) time.TotalHours > 0)
        {
            str = string.Format("{0}:{1}", time.Hours, (time.Minutes > 9 ? "" : "0") + time.Minutes);
        }
        else
        {
            str = string.Format("{0}:{1}", (time.Minutes > 9 ? "" : "0") + time.Minutes,
                (time.Seconds > 9 ? "" : "0") + time.Seconds);
        }

        if (str == "00:00") State = ChestState.Open;
        
        return str;
    }
    
    public override string ToString()
    {
        var str = string.Format("Uid {0}, Time {1}, Price: {2} - {3}", Uid, Time, Price.Currency, Price.Amount);

        for (var index = 0; index < Rewards.Count; index++)
        {
            var reward = Rewards[index];
            str += string.Format(" Reward {0}: {1} - {2}", index + 1, reward.Currency, reward.Amount);
        }

        return str;
    }
}