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
    
    [IncludeToCheatSheet] public static readonly CurrencyDef D5          = new CurrencyDef { Id = 1001, Name = "D5",         IsConsumable = true, Icon = "D5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef E5          = new CurrencyDef { Id = 1002, Name = "E5",         IsConsumable = true, Icon = "E5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef F5          = new CurrencyDef { Id = 1003, Name = "F5",         IsConsumable = true, Icon = "F5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef G5          = new CurrencyDef { Id = 1004, Name = "G5",         IsConsumable = true, Icon = "G5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef H5          = new CurrencyDef { Id = 1005, Name = "H5",         IsConsumable = true, Icon = "H5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef I5          = new CurrencyDef { Id = 1006, Name = "I5",         IsConsumable = true, Icon = "I5" };
    [IncludeToCheatSheet] public static readonly CurrencyDef J5          = new CurrencyDef { Id = 1007, Name = "J5",         IsConsumable = true, Icon = "J5" };
}