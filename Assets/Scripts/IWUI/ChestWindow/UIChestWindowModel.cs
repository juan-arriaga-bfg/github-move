using System;
using UnityEngine;

public enum ChestType
{
    None,
    Common,
    Rare,
    Epic
}

public enum ChestState
{
    None,
    Lock,
    Progres,
    Open
}

public class UIChestWindowModel : IWWindowModel
{
    public ChestDef Chest { get; set; }

    private ChestType chestType;
    
    private ChestType CurrentChestType
    {
        get
        {
            if (chestType != ChestType.None) return chestType;
            return (ChestType) Enum.Parse(typeof(ChestType), Chest.Uid);
        }
    }

    public ChestState CurrentChestState { get; set; }
    
    public string Title
    {
        get { return ""; }
    }
    
    public string Message
    {
        get { return CurrentChestState == ChestState.Progres ? "Unlocking takes:" : ""; }
    }
    
    public string ChestTypeText
    {
        get { return String.Format("{0} Chest", CurrentChestType); }
    }

    public string ButtonText
    {
        get
        {
            switch (CurrentChestState)
            {
                case ChestState.Lock:
                    return "Start unlock";
                case ChestState.Progres:
                    return string.Format("<size=60>{0}</size> OPEN", GetUnlockPrice());
                default:
                    return "";
            }
        }
    }

    public string TimerLength
    {
        get
        {
            var min = new TimeSpan(0, 0, Chest.Time).TotalMinutes;
            return string.Format("<color=#{0}><size=70>{1}</size>\nMinute{2}</color>", "EA7A48", min, min > 1 ? "s" : "");
        }
    }

    public Sprite ChestImage
    {
        get
        {
            var id = "chest_3";
            
            switch (CurrentChestType)
            {
                case ChestType.Common:
                    id = "chest_1";
                    break;
                case ChestType.Rare:
                    id = "chest_2";
                    break;
                case ChestType.Epic:
                    id = "chest_4";
                    break;
            }
            
            return IconService.Current.GetSpriteById(id);
        }
    }

    private int GetUnlockPrice()
    {
        // TODO: дописать скудку за прошедшее время
        return Chest.Price.Amount;
    }
}