using System;

public class UIChestMessageWindowModel : IWWindowModel 
{
    public Chest Chest { get; set; }

    public Action OnStart;
    public Action OnBoost;
    
    public string Title
    {
        get { return "Unlock chest!"; }
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
                    return "This chest is unlocked. Do you want to unlock this chest immediately?";
            }

            return "Error!";
        }
    }
    
    public string FastButtonText
    {
        get { return string.Format("Unlock Now\n{0}<sprite name={1}>", Chest.Def.Price.Amount, Chest.Def.Price.Currency); }
    }
    
    public string SlowButtonText
    {
        get { return string.Format("Unlock in {0}", Chest.GetTimeText()); }
    }
    
    public bool IsShowSlowButton
    {
        get { return GameDataService.Current.ChestsManager.ActiveChest == null; }
    }
}
