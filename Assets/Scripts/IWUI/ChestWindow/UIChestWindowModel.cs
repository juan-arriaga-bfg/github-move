using System;
using UnityEngine;

public class UIChestWindowModel : IWWindowModel
{
    public ChestDef Chest { get; set; }

    public string Title
    {
        get { return ""; }
    }
    
    public string Message
    {
        get { return Chest.GetState() == ChestState.InProgres ? "Unlocking takes:" : ""; }
    }
    
    public string ChestTypeText
    {
        get { return String.Format("{0} Chest", Chest.GetChestType()); }
    }

    public string ButtonText
    {
        get
        {
            switch (Chest.GetState())
            {
                case ChestState.Lock:
                    return "Start unlock";
                case ChestState.InProgres:
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
            return IconService.Current.GetSpriteById(Chest.GetSkin());
        }
    }

    private int GetUnlockPrice()
    {
        // TODO: дописать скудку за прошедшее время
        return Chest.Price.Amount;
    }
}