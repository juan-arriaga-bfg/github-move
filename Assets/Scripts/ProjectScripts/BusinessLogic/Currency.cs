public partial class CurrencyDef
{
    private string icon;
    public string Icon
    {
        get { return string.IsNullOrEmpty(icon) ? $"icon_{Name}" : icon; }
        set { icon = value; }
    }
}

public static partial class Currency
{
    [IncludeToCheatSheet] public static readonly CurrencyDef Crystals    = new CurrencyDef { Id = 20,  Name = "Crystals",    IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef Mana        = new CurrencyDef { Id = 30,  Name = "Mana",        IsConsumable = true };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Energy      = new CurrencyDef { Id = 40,  Name = "Energy",      IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef EnergyLimit = new CurrencyDef { Id = 41,  Name = "EnergyLimit", IsConsumable = true, Icon = "icon_Energy" };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Worker      = new CurrencyDef { Id = 50,  Name = "Worker",      IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef WorkerLimit = new CurrencyDef { Id = 51,  Name = "WorkerLimit", IsConsumable = true, Icon = "icon_Worker" };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Level       = new CurrencyDef { Id = 60,  Name = "Level",       IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef Experience  = new CurrencyDef { Id = 61,  Name = "Experience",  IsConsumable = true };
                                                                                 
                          public static readonly CurrencyDef Codex       = new CurrencyDef { Id = 70,  Name = "Codex",       IsConsumable = false };
                          public static readonly CurrencyDef Order       = new CurrencyDef { Id = 71,  Name = "Order",       IsConsumable = false };
    
                          public static readonly CurrencyDef Chest       = new CurrencyDef { Id = 80,  Name = "Chest",       IsConsumable = true };
                          public static readonly CurrencyDef Mine        = new CurrencyDef { Id = 90,  Name = "Mine",        IsConsumable = true };
                          public static readonly CurrencyDef Fog         = new CurrencyDef { Id = 100, Name = "Fog",         IsConsumable = true };
                                                                                                                            
                          public static readonly CurrencyDef Quest       = new CurrencyDef { Id = 110, Name = "Quest",       IsConsumable = true };
                          public static readonly CurrencyDef Damage      = new CurrencyDef { Id = 120, Name = "Damage",      IsConsumable = true };
                          public static readonly CurrencyDef Life        = new CurrencyDef { Id = 130, Name = "Life",        IsConsumable = true };
                          public static readonly CurrencyDef Timer       = new CurrencyDef { Id = 140, Name = "Timer",       IsConsumable = true };
                          public static readonly CurrencyDef Firefly    = new CurrencyDef { Id = 150, Name = "Fireflay",     IsConsumable = true };
    
                          public static readonly CurrencyDef Resources   = new CurrencyDef { Id = 1000, Name = "Resources",  IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_A5       = new CurrencyDef { Id = 1001, Name = "PR_A5",      IsConsumable = true, Icon = "PR_A5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_B5       = new CurrencyDef { Id = 1002, Name = "PR_B5",      IsConsumable = true, Icon = "PR_B5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_C5       = new CurrencyDef { Id = 1003, Name = "PR_C5",      IsConsumable = true, Icon = "PR_C5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_D5       = new CurrencyDef { Id = 1004, Name = "PR_D5",      IsConsumable = true, Icon = "PR_D5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_E5       = new CurrencyDef { Id = 1005, Name = "PR_E5",      IsConsumable = true, Icon = "PR_E5" };
    
                          public static readonly CurrencyDef Hard1       = new CurrencyDef { Id = 2000, Name = "Hard1",      IsConsumable = true, Icon = "Hard1" };
                          public static readonly CurrencyDef Hard2       = new CurrencyDef { Id = 2001, Name = "Hard2",      IsConsumable = true, Icon = "Hard2" };
                          public static readonly CurrencyDef Hard3       = new CurrencyDef { Id = 2002, Name = "Hard3",      IsConsumable = true, Icon = "Hard3" };
                          public static readonly CurrencyDef Hard4       = new CurrencyDef { Id = 2003, Name = "Hard4",      IsConsumable = true, Icon = "Hard4" };
                          public static readonly CurrencyDef Hard5       = new CurrencyDef { Id = 2004, Name = "Hard5",      IsConsumable = true, Icon = "Hard5" };
                          public static readonly CurrencyDef Hard6       = new CurrencyDef { Id = 2005, Name = "Hard6",      IsConsumable = true, Icon = "Hard6" };
    
                          public static readonly CurrencyDef Soft1       = new CurrencyDef { Id = 3000, Name = "Soft1",      IsConsumable = true, Icon = "Soft1" };
                          public static readonly CurrencyDef Soft2       = new CurrencyDef { Id = 3001, Name = "Soft2",      IsConsumable = true, Icon = "Soft2" };
                          public static readonly CurrencyDef Soft3       = new CurrencyDef { Id = 3002, Name = "Soft3",      IsConsumable = true, Icon = "Soft3" };
                          public static readonly CurrencyDef Soft4       = new CurrencyDef { Id = 3003, Name = "Soft4",      IsConsumable = true, Icon = "Soft4" };
                          public static readonly CurrencyDef Soft5       = new CurrencyDef { Id = 3004, Name = "Soft5",      IsConsumable = true, Icon = "Soft5" };
                          public static readonly CurrencyDef Soft6       = new CurrencyDef { Id = 3005, Name = "Soft6",      IsConsumable = true, Icon = "Soft6" };
    
                          public static readonly CurrencyDef CH_Free     = new CurrencyDef { Id = 4000, Name = "CH_Free",    IsConsumable = true, Icon = "CH_Free"};
                          public static readonly CurrencyDef CH1_A       = new CurrencyDef { Id = 4001, Name = "CH1_A",      IsConsumable = true, Icon = "CH1_A" };
                          public static readonly CurrencyDef CH2_A       = new CurrencyDef { Id = 4002, Name = "CH2_A",      IsConsumable = true, Icon = "CH2_A" };
                          public static readonly CurrencyDef CH3_A       = new CurrencyDef { Id = 4003, Name = "CH3_A",      IsConsumable = true, Icon = "CH3_A" };
                          public static readonly CurrencyDef CH1_B       = new CurrencyDef { Id = 4004, Name = "CH1_B",      IsConsumable = true, Icon = "CH1_B" };
                          public static readonly CurrencyDef CH2_B       = new CurrencyDef { Id = 4005, Name = "CH2_B",      IsConsumable = true, Icon = "CH2_B" };
                          public static readonly CurrencyDef CH3_B       = new CurrencyDef { Id = 4006, Name = "CH3_B",      IsConsumable = true, Icon = "CH3_B" };
                          public static readonly CurrencyDef CH1_C       = new CurrencyDef { Id = 4007, Name = "CH1_C",      IsConsumable = true, Icon = "CH1_C" };
                          public static readonly CurrencyDef CH2_C       = new CurrencyDef { Id = 4008, Name = "CH2_C",      IsConsumable = true, Icon = "CH2_C" };
                          public static readonly CurrencyDef CH3_C       = new CurrencyDef { Id = 4009, Name = "CH3_C",      IsConsumable = true, Icon = "CH3_C" };
                          public static readonly CurrencyDef CH1_D       = new CurrencyDef { Id = 4010, Name = "CH1_D",      IsConsumable = true, Icon = "CH1_D" };
                          public static readonly CurrencyDef CH2_D       = new CurrencyDef { Id = 4011, Name = "CH2_D",      IsConsumable = true, Icon = "CH2_D" };
                          public static readonly CurrencyDef CH3_D       = new CurrencyDef { Id = 4012, Name = "CH3_D",      IsConsumable = true, Icon = "CH3_D" };
}