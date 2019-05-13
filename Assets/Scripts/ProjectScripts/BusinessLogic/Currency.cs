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
                          public static readonly CurrencyDef Mana        = new CurrencyDef { Id = 30,  Name = "Mana",        IsConsumable = true };
                          public static readonly CurrencyDef ManaFake    = new CurrencyDef { Id = 31,  Name = "ManaFake",    IsConsumable = true };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Energy      = new CurrencyDef { Id = 40,  Name = "Energy",      IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef EnergyLimit = new CurrencyDef { Id = 41,  Name = "EnergyLimit", IsConsumable = true, Icon = "icon_Energy" };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Worker      = new CurrencyDef { Id = 50,  Name = "Worker",      IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef WorkerLimit = new CurrencyDef { Id = 51,  Name = "WorkerLimit", IsConsumable = true, Icon = "icon_Worker" };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Level       = new CurrencyDef { Id = 60,  Name = "Level",       IsConsumable = true, Icon = "icon_Experience" };
    [IncludeToCheatSheet] public static readonly CurrencyDef Experience  = new CurrencyDef { Id = 61,  Name = "Experience",  IsConsumable = true };
    
                          public static readonly CurrencyDef Codex       = new CurrencyDef { Id = 70,  Name = "Codex",       IsConsumable = false };
                          public static readonly CurrencyDef Order       = new CurrencyDef { Id = 71,  Name = "Order",       IsConsumable = false };
                          
                          public static readonly CurrencyDef Market      = new CurrencyDef { Id = 80,  Name = "Market",      IsConsumable = true };
                          public static readonly CurrencyDef Workplace   = new CurrencyDef { Id = 90,  Name = "Workplace",   IsConsumable = true };
                          public static readonly CurrencyDef Fog         = new CurrencyDef { Id = 100, Name = "Fog",         IsConsumable = true };
                          
                          public static readonly CurrencyDef Quest       = new CurrencyDef { Id = 110, Name = "Quest",       IsConsumable = true };
                          public static readonly CurrencyDef Damage      = new CurrencyDef { Id = 120, Name = "Damage",      IsConsumable = true };
                          public static readonly CurrencyDef Timer       = new CurrencyDef { Id = 130, Name = "Timer",       IsConsumable = true };
                          public static readonly CurrencyDef Firefly     = new CurrencyDef { Id = 140, Name = "Firefly",     IsConsumable = true };
                          public static readonly CurrencyDef Character   = new CurrencyDef { Id = 150, Name = "Character",   IsConsumable = true };
                          public static readonly CurrencyDef Offer       = new CurrencyDef { Id = 160, Name = "Offer",       IsConsumable = true };
                          
    [IncludeToCheatSheet] public static readonly CurrencyDef Token       = new CurrencyDef { Id = 170, Name = "Token",       IsConsumable = true };
                          public static readonly CurrencyDef EventStep   = new CurrencyDef { Id = 171, Name = "EventStep",   IsConsumable = true };
                          
                          public static readonly CurrencyDef Island      = new CurrencyDef { Id = 180, Name = "Island",       IsConsumable = true };
                          
                          public static readonly CurrencyDef Resources   = new CurrencyDef { Id = 1000, Name = "Resources",  IsConsumable = true };
                          
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_A5       = new CurrencyDef { Id = 1001, Name = "PR_A5",      IsConsumable = true, Icon = "PR_A5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_B5       = new CurrencyDef { Id = 1002, Name = "PR_B5",      IsConsumable = true, Icon = "PR_B5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_C5       = new CurrencyDef { Id = 1003, Name = "PR_C5",      IsConsumable = true, Icon = "PR_C5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_D5       = new CurrencyDef { Id = 1004, Name = "PR_D5",      IsConsumable = true, Icon = "PR_D5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_E5       = new CurrencyDef { Id = 1005, Name = "PR_E5",      IsConsumable = true, Icon = "PR_E5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_F5       = new CurrencyDef { Id = 1006, Name = "PR_F5",      IsConsumable = true, Icon = "PR_F5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef PR_G5       = new CurrencyDef { Id = 1007, Name = "PR_G5",      IsConsumable = true, Icon = "PR_G5" };
    
                          public static readonly CurrencyDef Extra       = new CurrencyDef { Id = 1100, Name = "Extra",      IsConsumable = true };
}