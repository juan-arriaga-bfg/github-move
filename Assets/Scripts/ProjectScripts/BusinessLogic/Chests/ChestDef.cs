using System;
using System.Collections.Generic;

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

    public DateTime StartTime { get; set; }
    
    private ChestType chestType;

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

    public ChestState GetState()
    {
        var delay = DateTime.Now - StartTime;

        if (delay.TotalSeconds < 1) return ChestState.Lock;
        if (delay.TotalSeconds < Time) return ChestState.InProgres;
        
        return ChestState.Open;
    }

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

    public string GetOpenTime()
    {
        var time = new TimeSpan(0, 0, Time);

        if (time.TotalHours > 0) return string.Format("{0}h {1}m", time.Hours, time.Minutes);
        
        return string.Format("{0}m", time.Minutes);
    }

    public string getCurrentTime()
    {
        var time = DateTime.Now - StartTime;
        
        if (time.TotalHours > 0) return string.Format("{0}:{1}", time.Hours, time.Minutes);
        return string.Format("{0}:{1}", time.Minutes, time.Seconds);
    }
}