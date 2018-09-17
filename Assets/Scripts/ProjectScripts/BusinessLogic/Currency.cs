public static partial class Currency
{
    [IncludeToCheatSheet] public static readonly CurrencyDef Crystals    = new CurrencyDef { Id = 20,  Name = "Crystals",    IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef Mana        = new CurrencyDef { Id = 30,  Name = "Mana",        IsConsumable = true };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Energy      = new CurrencyDef { Id = 40,  Name = "Energy",      IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef EnergyLimit = new CurrencyDef { Id = 41,  Name = "EnergyLimit", IsConsumable = true };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Worker      = new CurrencyDef { Id = 50,  Name = "Worker",      IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef WorkerLimit = new CurrencyDef { Id = 51,  Name = "WorkerLimit", IsConsumable = true };
                                                                                                                           
    [IncludeToCheatSheet] public static readonly CurrencyDef Level       = new CurrencyDef { Id = 60,  Name = "Level",       IsConsumable = true };
    [IncludeToCheatSheet] public static readonly CurrencyDef Experience  = new CurrencyDef { Id = 61,  Name = "Experience",  IsConsumable = true };
                                                                                 
                          public static readonly CurrencyDef Piece       = new CurrencyDef { Id = 70,  Name = "Piece",       IsConsumable = true };
                          public static readonly CurrencyDef Chest       = new CurrencyDef { Id = 80,  Name = "Chest",       IsConsumable = true };
                          public static readonly CurrencyDef Mine        = new CurrencyDef { Id = 90,  Name = "Mine",        IsConsumable = true };
                          public static readonly CurrencyDef Fog         = new CurrencyDef { Id = 100, Name = "Fog",         IsConsumable = true };
                                                                                                                            
                          public static readonly CurrencyDef Quest       = new CurrencyDef { Id = 110, Name = "Quest",       IsConsumable = true };
                          public static readonly CurrencyDef Damage      = new CurrencyDef { Id = 120, Name = "Damage",      IsConsumable = true };
                          public static readonly CurrencyDef Life        = new CurrencyDef { Id = 130, Name = "Life",        IsConsumable = true };
                          public static readonly CurrencyDef Building    = new CurrencyDef { Id = 140, Name = "Product",     IsConsumable = true };
                          public static readonly CurrencyDef Timer       = new CurrencyDef { Id = 150, Name = "Timer",       IsConsumable = true };
}