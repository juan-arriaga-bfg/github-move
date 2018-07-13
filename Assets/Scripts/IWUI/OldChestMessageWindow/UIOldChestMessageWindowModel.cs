using System;
using System.Collections.Generic;

public class UIOldChestMessageWindowModel : IWWindowModel 
{
    public Chest Chest { get; set; }

    public Action OnStart;
    public Action OnBoost;
    public Action OnStop;
    
    public string Title
    {
        get { return IsCurrentActive ? "Chest unlocking!" : "Unlock chest!"; }
    }
    
    public string Message
    {
        get
        {
            switch (Chest.State)
            {
                case ChestState.Close:
                    return IsShowSlowButton 
                        ? "Do you want to unlock this chest?"
                        : "Another chest is unlocking. Do you want to unlock this chest immediately?";
                case ChestState.InProgress:
                    return IsCurrentActive
                        ? "Chest is unlocking. Do you want to unlock chest immediately?"
                        : "This chest is unlocked. Do you want to unlock this chest immediately?";
            }
            
            return "Error!";
        }
    }
    
    public string FastButtonText
    {
        get { return string.Format("Unlock Now\n{0}<sprite name={1}>", Chest.Price.Amount, Chest.Price.Currency); }
    }
    
    public string SlowButtonText
    {
        get { return string.Format("Unlock in {0}", Chest.GetTimeText()); }
    }
    
    public string StopButtonText
    {
        get { return "Cancel"; }
    }
    
    public string ChanceText
    {
        get { return "May contain:"; }
    }
    
    public bool IsShowSlowButton
    {
        get { return GameDataService.Current.ChestsManager.ActiveChest == null; }
    }

    public bool IsCurrentActive
    {
        get { return GameDataService.Current.ChestsManager.ActiveChest == Chest; }
    }

    public List<string> Icons()
    {
        var icons = new List<string>();

        foreach (var piece in Chest.Def.PieceWeights)
        {
            if(piece.Weight == 0) continue;
            
            icons.Add(piece.Uid);
        }
        
        return icons;
    }
}