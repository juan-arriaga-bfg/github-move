using System.Collections.Generic;

public enum CurrencyTag
{
    All,
    Food,
    Build,
    LevelUp
}

public partial class CurrencyDef
{
    public List<CurrencyTag> Tags;
}

public static partial class Currency
{
    public static readonly CurrencyDef Crystals = new CurrencyDef { Id = 2, Name = "Crystals", IsConsumable = true };
    
    public static readonly CurrencyDef Enemy = new CurrencyDef { Id = 4, Name = "Enemy", IsConsumable = true };
    
    public static readonly CurrencyDef Quest = new CurrencyDef { Id = 5, Name = "Quest", IsConsumable = true };
    public static readonly CurrencyDef Obstacle = new CurrencyDef { Id = 6, Name = "Obstacle", IsConsumable = true };
    public static readonly CurrencyDef Task = new CurrencyDef { Id = 7, Name = "Task", IsConsumable = true };
    public static readonly CurrencyDef Product = new CurrencyDef { Id = 8, Name = "Product", IsConsumable = true };
    
    public static readonly CurrencyDef Power = new CurrencyDef { Id = 100, Name = "Power", IsConsumable = true };
    public static readonly CurrencyDef Level = new CurrencyDef { Id = 200, Name = "Level", IsConsumable = true };
    public static readonly CurrencyDef Experience = new CurrencyDef { Id = 300, Name = "Experience", IsConsumable = true };
    
    public static readonly CurrencyDef Robin = new CurrencyDef { Id = 400, Name = "Robin", IsConsumable = false };
    public static readonly CurrencyDef John = new CurrencyDef { Id = 401, Name = "John", IsConsumable = false };
    public static readonly CurrencyDef Greenarrow = new CurrencyDef { Id = 402, Name = "Greenarrow", IsConsumable = false };
    
    public static readonly CurrencyDef LevelCastle = new CurrencyDef { Id = 10003, Name = "LevelCastle", IsConsumable = true };
    public static readonly CurrencyDef LevelMarket = new CurrencyDef { Id = 10004, Name = "LevelMarket", IsConsumable = true };
    public static readonly CurrencyDef LevelMine = new CurrencyDef { Id = 10005, Name = "LevelMine", IsConsumable = true };
    public static readonly CurrencyDef LevelSawmill = new CurrencyDef { Id = 10006, Name = "LevelSawmill", IsConsumable = true };
    public static readonly CurrencyDef LevelSheepfold = new CurrencyDef { Id = 10007, Name = "LevelSheepfold", IsConsumable = true };
    public static readonly CurrencyDef LevelStorage = new CurrencyDef { Id = 10008, Name = "LevelStorage", IsConsumable = true };
    public static readonly CurrencyDef LevelFactory = new CurrencyDef { Id = 10009, Name = "LevelFactory", IsConsumable = true };
    
    public static readonly CurrencyDef Charger1 = new CurrencyDef { Id = 1001, Name = "Charger1", IsConsumable = true };
    public static readonly CurrencyDef Charger2 = new CurrencyDef { Id = 1002, Name = "Charger2", IsConsumable = true };
    public static readonly CurrencyDef Charger3 = new CurrencyDef { Id = 1003, Name = "Charger3", IsConsumable = true };
    public static readonly CurrencyDef Charger4 = new CurrencyDef { Id = 1004, Name = "Charger4", IsConsumable = true };
    public static readonly CurrencyDef Charger5 = new CurrencyDef { Id = 1005, Name = "Charger5", IsConsumable = true };
    public static readonly CurrencyDef Charger6 = new CurrencyDef { Id = 1006, Name = "Charger6", IsConsumable = true };
    public static readonly CurrencyDef Charger7 = new CurrencyDef { Id = 1007, Name = "Charger7", IsConsumable = true };
    public static readonly CurrencyDef Charger8 = new CurrencyDef { Id = 1008, Name = "Charger8", IsConsumable = true };
    public static readonly CurrencyDef Charger9 = new CurrencyDef { Id = 1009, Name = "Charger9", IsConsumable = true };
    
    public static readonly CurrencyDef Resource1 = new CurrencyDef { Id = 1101, Name = "Resource1", IsConsumable = true, Tags = new List<CurrencyTag>()};
    public static readonly CurrencyDef Resource2 = new CurrencyDef { Id = 1102, Name = "Resource2", IsConsumable = true, Tags = new List<CurrencyTag>()};
    public static readonly CurrencyDef Resource3 = new CurrencyDef { Id = 1103, Name = "Resource3", IsConsumable = true, Tags = new List<CurrencyTag>()};
    public static readonly CurrencyDef Resource4 = new CurrencyDef { Id = 1104, Name = "Resource4", IsConsumable = true, Tags = new List<CurrencyTag>()};
    public static readonly CurrencyDef Resource5 = new CurrencyDef { Id = 1105, Name = "Resource5", IsConsumable = true, Tags = new List<CurrencyTag>()};
    public static readonly CurrencyDef Resource6 = new CurrencyDef { Id = 1106, Name = "Resource6", IsConsumable = true, Tags = new List<CurrencyTag>()};
    public static readonly CurrencyDef Resource7 = new CurrencyDef { Id = 1107, Name = "Resource7", IsConsumable = true, Tags = new List<CurrencyTag>()};
    public static readonly CurrencyDef Resource8 = new CurrencyDef { Id = 1108, Name = "Resource8", IsConsumable = true, Tags = new List<CurrencyTag>()};
    public static readonly CurrencyDef Resource9 = new CurrencyDef { Id = 1109, Name = "Resource9", IsConsumable = true, Tags = new List<CurrencyTag>()};
    
    public static readonly CurrencyDef RobinItem1 = new CurrencyDef { Id = 11001, Name = "RobinItem1", IsConsumable = false };
    public static readonly CurrencyDef RobinItem2 = new CurrencyDef { Id = 11002, Name = "RobinItem2", IsConsumable = false };
    public static readonly CurrencyDef RobinItem3 = new CurrencyDef { Id = 11003, Name = "RobinItem3", IsConsumable = false };
    public static readonly CurrencyDef RobinItem4 = new CurrencyDef { Id = 11004, Name = "RobinItem4", IsConsumable = false };
    
    public static readonly CurrencyDef JohnItem1 = new CurrencyDef { Id = 12001, Name = "JohnItem1", IsConsumable = false };
    public static readonly CurrencyDef JohnItem2 = new CurrencyDef { Id = 12002, Name = "JohnItem2", IsConsumable = false };
    public static readonly CurrencyDef JohnItem3 = new CurrencyDef { Id = 12003, Name = "JohnItem3", IsConsumable = false };
    public static readonly CurrencyDef JohnItem4 = new CurrencyDef { Id = 12004, Name = "JohnItem4", IsConsumable = false };
    
    public static readonly CurrencyDef GreenarrowItem1 = new CurrencyDef { Id = 13001, Name = "GreenarrowItem1", IsConsumable = false };
    public static readonly CurrencyDef GreenarrowItem2 = new CurrencyDef { Id = 13002, Name = "GreenarrowItem2", IsConsumable = false };
    public static readonly CurrencyDef GreenarrowItem3 = new CurrencyDef { Id = 13003, Name = "GreenarrowItem3", IsConsumable = false };
    public static readonly CurrencyDef GreenarrowItem4 = new CurrencyDef { Id = 13004, Name = "GreenarrowItem4", IsConsumable = false };
    
    public static List<CurrencyDef> GetCurrencyDefs(CurrencyTag tag)
    {
        var result = new List<CurrencyDef>();

        foreach (var def in cachedCurrencyIdsDefs.Values)
        {
            if(def.Tags == null || tag != CurrencyTag.All && def.Tags.IndexOf(tag) == -1) continue;
            
            result.Add(def);
        }
        
        return result;
    }
}