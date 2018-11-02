using System;
using System.Collections.Generic;
using System.Reflection;

[Flags]
public enum PieceTypeFilter
{
    Default = 0x0,
    // Not used
    Simple = 2,
    Multicellular = 4,
    Obstacle = 8,
    Chest = 16,
    Mine = 32,
    Resource = 64,
    Energy = 128,
    Reproduction = 256,
    Character = 512,
    Fake = 1024,
    Enemy = 2048,
    Booster = 4096,
    WorkPlace = 8192,
    Tree = 16384,
}

public partial class PieceTypeDef
{
    public PieceTypeFilter Filter { get; set; }
}

public static partial class PieceType 
{
    private static readonly Dictionary<int, PieceTypeDef> defs = new Dictionary<int, PieceTypeDef>();
    
    static PieceType()
    {
        var t = typeof(PieceType);
        var fieldInfos = t.GetFields(BindingFlags.Public | BindingFlags.Static);
        
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            var fieldInfo = fieldInfos[i];
            if (fieldInfo.FieldType != typeof(PieceTypeDef)) continue;
            
            var fieldValue = (PieceTypeDef)fieldInfo.GetValue(null);
            
            RegisterType(fieldValue);
            defs.Add(fieldValue.Id, fieldValue);
        }
    }

    public static PieceTypeDef GetDefById(int id)
    {
        return defs.ContainsKey(id) == false ? defs[PieceType.None.Id] : defs[id];
    }

    public static List<int> GetIdsByFilter(PieceTypeFilter filter)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            if(def.Filter.Has(filter) == false) continue;
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    public static List<int> GetAllIds()
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    // -------------------------- Characters
    
    public static readonly PieceTypeDef Char1 = new PieceTypeDef{Id = 10, Abbreviations = new List<string>{ "Char1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char2 = new PieceTypeDef{Id = 11, Abbreviations = new List<string>{ "Char2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char3 = new PieceTypeDef{Id = 12, Abbreviations = new List<string>{ "Char3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char4 = new PieceTypeDef{Id = 13, Abbreviations = new List<string>{ "Char4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char5 = new PieceTypeDef{Id = 14, Abbreviations = new List<string>{ "Char5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char6 = new PieceTypeDef{Id = 15, Abbreviations = new List<string>{ "Char6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char7 = new PieceTypeDef{Id = 16, Abbreviations = new List<string>{ "Char7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char8 = new PieceTypeDef{Id = 17, Abbreviations = new List<string>{ "Char8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char9 = new PieceTypeDef{Id = 18, Abbreviations = new List<string>{ "Char9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    
    // -------------------------- Enemies
    
    public static readonly PieceTypeDef Enemy1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "Enemy1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Enemy};
    
    // -------------------------- Boosters
    
    public static readonly PieceTypeDef Magic1 = new PieceTypeDef{Id = 2001, Abbreviations = new List<string>{ "Magic1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef Magic2 = new PieceTypeDef{Id = 2002, Abbreviations = new List<string>{ "Magic2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef Magic3 = new PieceTypeDef{Id = 2003, Abbreviations = new List<string>{ "Magic3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef Magic  = new PieceTypeDef{Id = 2004, Abbreviations = new List<string>{ "Magic"  }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Booster};
    
    public static readonly PieceTypeDef Worker1 = new PieceTypeDef{Id = 2100, Abbreviations = new List<string>{ "Worker1" }, Filter = PieceTypeFilter.Simple};
    
    // -------------------------- Currencies
    
    public static readonly PieceTypeDef Coin1 = new PieceTypeDef{Id = 3100, Abbreviations = new List<string>{ "Coin1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin2 = new PieceTypeDef{Id = 3101, Abbreviations = new List<string>{ "Coin2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin3 = new PieceTypeDef{Id = 3102, Abbreviations = new List<string>{ "Coin3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource}; 
    public static readonly PieceTypeDef Coin4 = new PieceTypeDef{Id = 3103, Abbreviations = new List<string>{ "Coin4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin5 = new PieceTypeDef{Id = 3104, Abbreviations = new List<string>{ "Coin5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin6 = new PieceTypeDef{Id = 3105, Abbreviations = new List<string>{ "Coin6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef Crystal1 = new PieceTypeDef{Id = 3200, Abbreviations = new List<string>{ "Crystal1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Crystal2 = new PieceTypeDef{Id = 3201, Abbreviations = new List<string>{ "Crystal2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Crystal3 = new PieceTypeDef{Id = 3202, Abbreviations = new List<string>{ "Crystal3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource}; 
    public static readonly PieceTypeDef Crystal4 = new PieceTypeDef{Id = 3203, Abbreviations = new List<string>{ "Crystal4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Crystal5 = new PieceTypeDef{Id = 3204, Abbreviations = new List<string>{ "Crystal5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    // -------------------------- Mines
    
    public static readonly PieceTypeDef MineC = new PieceTypeDef{Id = 5001, Abbreviations = new List<string>{ "MineC" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef MineK = new PieceTypeDef{Id = 5002, Abbreviations = new List<string>{ "MineK" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef MineL = new PieceTypeDef{Id = 5003, Abbreviations = new List<string>{ "MineL" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.WorkPlace};
    
    // -------------------------- Chests
    
    public static readonly PieceTypeDef Chest1 = new PieceTypeDef{Id = 10001, Abbreviations = new List<string>{ "Chest1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestA1 = new PieceTypeDef{Id = 10100, Abbreviations = new List<string>{ "ChestA1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestA2 = new PieceTypeDef{Id = 10101, Abbreviations = new List<string>{ "ChestA2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestA3 = new PieceTypeDef{Id = 10102, Abbreviations = new List<string>{ "ChestA3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestC1 = new PieceTypeDef{Id = 10201, Abbreviations = new List<string>{ "ChestC1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestC2 = new PieceTypeDef{Id = 10202, Abbreviations = new List<string>{ "ChestC2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestC3 = new PieceTypeDef{Id = 10203, Abbreviations = new List<string>{ "ChestC3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestK1 = new PieceTypeDef{Id = 10301, Abbreviations = new List<string>{ "ChestK1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestK2 = new PieceTypeDef{Id = 10302, Abbreviations = new List<string>{ "ChestK2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestK3 = new PieceTypeDef{Id = 10303, Abbreviations = new List<string>{ "ChestK3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestL1 = new PieceTypeDef{Id = 10401, Abbreviations = new List<string>{ "ChestL1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestL2 = new PieceTypeDef{Id = 10402, Abbreviations = new List<string>{ "ChestL2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestL3 = new PieceTypeDef{Id = 10403, Abbreviations = new List<string>{ "ChestL3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    // -------------------------- Obstacles
    
    public static readonly PieceTypeDef Fog = new PieceTypeDef{Id = 20000, Abbreviations = new List<string>{ "Fog" }, Filter = PieceTypeFilter.Multicellular};
    
    public static readonly PieceTypeDef O1 = new PieceTypeDef{Id = 20100, Abbreviations = new List<string>{ "O1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef O2 = new PieceTypeDef{Id = 20101, Abbreviations = new List<string>{ "O2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef O3 = new PieceTypeDef{Id = 20102, Abbreviations = new List<string>{ "O3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef O4 = new PieceTypeDef{Id = 20103, Abbreviations = new List<string>{ "O4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef O5 = new PieceTypeDef{Id = 20104, Abbreviations = new List<string>{ "O5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef O6 = new PieceTypeDef{Id = 20105, Abbreviations = new List<string>{ "O6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef O7 = new PieceTypeDef{Id = 20106, Abbreviations = new List<string>{ "O7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef O8 = new PieceTypeDef{Id = 20107, Abbreviations = new List<string>{ "O8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef O9 = new PieceTypeDef{Id = 20108, Abbreviations = new List<string>{ "O9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace | PieceTypeFilter.Tree};
    
    public static readonly PieceTypeDef ObstacleD = new PieceTypeDef{Id = 20200, Abbreviations = new List<string>{ "ObstacleD" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef ObstacleE = new PieceTypeDef{Id = 20201, Abbreviations = new List<string>{ "ObstacleE" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef ObstacleF = new PieceTypeDef{Id = 20202, Abbreviations = new List<string>{ "ObstacleF" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef ObstacleG = new PieceTypeDef{Id = 20203, Abbreviations = new List<string>{ "ObstacleG" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef ObstacleH = new PieceTypeDef{Id = 20204, Abbreviations = new List<string>{ "ObstacleH" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.WorkPlace};
    
    // -------------------------- Simple Pieces
    
    public static readonly PieceTypeDef A1 = new PieceTypeDef{Id = 100000, Abbreviations = new List<string>{ "A1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A2 = new PieceTypeDef{Id = 100001, Abbreviations = new List<string>{ "A2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A3Fake = new PieceTypeDef{Id = 100002, Abbreviations = new List<string>{ "A3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef A3 = new PieceTypeDef{Id = 100003, Abbreviations = new List<string>{ "A3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A4Fake = new PieceTypeDef{Id = 100004, Abbreviations = new List<string>{ "A4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef A4 = new PieceTypeDef{Id = 100005, Abbreviations = new List<string>{ "A4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A5Fake = new PieceTypeDef{Id = 100006, Abbreviations = new List<string>{ "A5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef A5 = new PieceTypeDef{Id = 100007, Abbreviations = new List<string>{ "A5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A6Fake = new PieceTypeDef{Id = 100008, Abbreviations = new List<string>{ "A6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef A6 = new PieceTypeDef{Id = 100009, Abbreviations = new List<string>{ "A6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A7Fake = new PieceTypeDef{Id = 100010, Abbreviations = new List<string>{ "A7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef A7 = new PieceTypeDef{Id = 100011, Abbreviations = new List<string>{ "A7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A8Fake = new PieceTypeDef{Id = 100012, Abbreviations = new List<string>{ "A8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef A8 = new PieceTypeDef{Id = 100013, Abbreviations = new List<string>{ "A8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef A9Fake = new PieceTypeDef{Id = 100014, Abbreviations = new List<string>{ "A9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef A9 = new PieceTypeDef{Id = 100015, Abbreviations = new List<string>{ "A9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.WorkPlace};
    
    public static readonly PieceTypeDef C1 = new PieceTypeDef{Id = 100100, Abbreviations = new List<string>{ "C1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C2 = new PieceTypeDef{Id = 100101, Abbreviations = new List<string>{ "C2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C3Fake = new PieceTypeDef{Id = 100102, Abbreviations = new List<string>{ "C3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C3 = new PieceTypeDef{Id = 100103, Abbreviations = new List<string>{ "C3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C4Fake = new PieceTypeDef{Id = 100104, Abbreviations = new List<string>{ "C4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C4 = new PieceTypeDef{Id = 100105, Abbreviations = new List<string>{ "C4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C5Fake = new PieceTypeDef{Id = 100106, Abbreviations = new List<string>{ "C5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C5 = new PieceTypeDef{Id = 100107, Abbreviations = new List<string>{ "C5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C6Fake = new PieceTypeDef{Id = 100108, Abbreviations = new List<string>{ "C6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C6 = new PieceTypeDef{Id = 100109, Abbreviations = new List<string>{ "C6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C7Fake = new PieceTypeDef{Id = 100110, Abbreviations = new List<string>{ "C7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C7 = new PieceTypeDef{Id = 100111, Abbreviations = new List<string>{ "C7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C8Fake = new PieceTypeDef{Id = 100112, Abbreviations = new List<string>{ "C8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C8 = new PieceTypeDef{Id = 100113, Abbreviations = new List<string>{ "C8" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C9Fake = new PieceTypeDef{Id = 100114, Abbreviations = new List<string>{ "C9Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C9 = new PieceTypeDef{Id = 100115, Abbreviations = new List<string>{ "C9" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C10Fake = new PieceTypeDef{Id = 100116, Abbreviations = new List<string>{ "C10Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C10 = new PieceTypeDef{Id = 100117, Abbreviations = new List<string>{ "C10" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C11Fake = new PieceTypeDef{Id = 100118, Abbreviations = new List<string>{ "C11Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef C11 = new PieceTypeDef{Id = 100119, Abbreviations = new List<string>{ "C11" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.WorkPlace};
    
    public static readonly PieceTypeDef K1 = new PieceTypeDef{Id = 100200, Abbreviations = new List<string>{ "K1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef K2 = new PieceTypeDef{Id = 100201, Abbreviations = new List<string>{ "K2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef K3Fake = new PieceTypeDef{Id = 100202, Abbreviations = new List<string>{ "K3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K3 = new PieceTypeDef{Id = 100203, Abbreviations = new List<string>{ "K3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef K4Fake = new PieceTypeDef{Id = 100204, Abbreviations = new List<string>{ "K4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K4 = new PieceTypeDef{Id = 100205, Abbreviations = new List<string>{ "K4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef K5Fake = new PieceTypeDef{Id = 100206, Abbreviations = new List<string>{ "K5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K5 = new PieceTypeDef{Id = 100207, Abbreviations = new List<string>{ "K5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef K6Fake = new PieceTypeDef{Id = 100208, Abbreviations = new List<string>{ "K6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K6 = new PieceTypeDef{Id = 100209, Abbreviations = new List<string>{ "K6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef K7Fake = new PieceTypeDef{Id = 100210, Abbreviations = new List<string>{ "K7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K7 = new PieceTypeDef{Id = 100211, Abbreviations = new List<string>{ "K7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef K8Fake = new PieceTypeDef{Id = 100212, Abbreviations = new List<string>{ "K8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K8 = new PieceTypeDef{Id = 100213, Abbreviations = new List<string>{ "K8" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef K9Fake = new PieceTypeDef{Id = 100214, Abbreviations = new List<string>{ "K9Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K9 = new PieceTypeDef{Id = 100215, Abbreviations = new List<string>{ "K9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K10Fake = new PieceTypeDef{Id = 100216, Abbreviations = new List<string>{ "K10Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef K10 = new PieceTypeDef{Id = 100217, Abbreviations = new List<string>{ "K10" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.WorkPlace};
    
    public static readonly PieceTypeDef L1 = new PieceTypeDef{Id = 100300, Abbreviations = new List<string>{ "L1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef L2 = new PieceTypeDef{Id = 100301, Abbreviations = new List<string>{ "L2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef L3Fake = new PieceTypeDef{Id = 100302, Abbreviations = new List<string>{ "L3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef L3 = new PieceTypeDef{Id = 100303, Abbreviations = new List<string>{ "L3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef L4Fake = new PieceTypeDef{Id = 100304, Abbreviations = new List<string>{ "L4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef L4 = new PieceTypeDef{Id = 100305, Abbreviations = new List<string>{ "L4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef L5Fake = new PieceTypeDef{Id = 100306, Abbreviations = new List<string>{ "L5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef L5 = new PieceTypeDef{Id = 100307, Abbreviations = new List<string>{ "L5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef L6Fake = new PieceTypeDef{Id = 100308, Abbreviations = new List<string>{ "L6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef L6 = new PieceTypeDef{Id = 100309, Abbreviations = new List<string>{ "L6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef L7Fake = new PieceTypeDef{Id = 100310, Abbreviations = new List<string>{ "L7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef L7 = new PieceTypeDef{Id = 100311, Abbreviations = new List<string>{ "L7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef L8Fake = new PieceTypeDef{Id = 100312, Abbreviations = new List<string>{ "L8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef L8 = new PieceTypeDef{Id = 100313, Abbreviations = new List<string>{ "L8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef L9Fake = new PieceTypeDef{Id = 100314, Abbreviations = new List<string>{ "L9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef L9 = new PieceTypeDef{Id = 100315, Abbreviations = new List<string>{ "L9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.WorkPlace};
    
    // -------------------------- Reproduction Pieces
    
    public static readonly PieceTypeDef D1 = new PieceTypeDef{Id = 200000, Abbreviations = new List<string>{ "D1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D2 = new PieceTypeDef{Id = 200001, Abbreviations = new List<string>{ "D2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D3 = new PieceTypeDef{Id = 200002, Abbreviations = new List<string>{ "D3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D4 = new PieceTypeDef{Id = 200003, Abbreviations = new List<string>{ "D4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef D5 = new PieceTypeDef{Id = 200004, Abbreviations = new List<string>{ "D5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef E1 = new PieceTypeDef{Id = 200100, Abbreviations = new List<string>{ "E1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef E2 = new PieceTypeDef{Id = 200101, Abbreviations = new List<string>{ "E2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef E3 = new PieceTypeDef{Id = 200102, Abbreviations = new List<string>{ "E3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef E4 = new PieceTypeDef{Id = 200103, Abbreviations = new List<string>{ "E4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef E5 = new PieceTypeDef{Id = 200104, Abbreviations = new List<string>{ "E5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef F1 = new PieceTypeDef{Id = 200200, Abbreviations = new List<string>{ "F1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F2 = new PieceTypeDef{Id = 200201, Abbreviations = new List<string>{ "F2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F3 = new PieceTypeDef{Id = 200202, Abbreviations = new List<string>{ "F3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F4 = new PieceTypeDef{Id = 200203, Abbreviations = new List<string>{ "F4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef F5 = new PieceTypeDef{Id = 200204, Abbreviations = new List<string>{ "F5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef G1 = new PieceTypeDef{Id = 200300, Abbreviations = new List<string>{ "G1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G2 = new PieceTypeDef{Id = 200301, Abbreviations = new List<string>{ "G2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G3 = new PieceTypeDef{Id = 200302, Abbreviations = new List<string>{ "G3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G4 = new PieceTypeDef{Id = 200303, Abbreviations = new List<string>{ "G4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef G5 = new PieceTypeDef{Id = 200304, Abbreviations = new List<string>{ "G5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef H1 = new PieceTypeDef{Id = 200400, Abbreviations = new List<string>{ "H1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H2 = new PieceTypeDef{Id = 200401, Abbreviations = new List<string>{ "H2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H3 = new PieceTypeDef{Id = 200402, Abbreviations = new List<string>{ "H3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H4 = new PieceTypeDef{Id = 200403, Abbreviations = new List<string>{ "H4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef H5 = new PieceTypeDef{Id = 200404, Abbreviations = new List<string>{ "H5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef I1 = new PieceTypeDef{Id = 200500, Abbreviations = new List<string>{ "I1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef I2 = new PieceTypeDef{Id = 200501, Abbreviations = new List<string>{ "I2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef I3 = new PieceTypeDef{Id = 200502, Abbreviations = new List<string>{ "I3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef I4 = new PieceTypeDef{Id = 200503, Abbreviations = new List<string>{ "I4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef I5 = new PieceTypeDef{Id = 200504, Abbreviations = new List<string>{ "I5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef J1 = new PieceTypeDef{Id = 200600, Abbreviations = new List<string>{ "J1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef J2 = new PieceTypeDef{Id = 200601, Abbreviations = new List<string>{ "J2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef J3 = new PieceTypeDef{Id = 200602, Abbreviations = new List<string>{ "J3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef J4 = new PieceTypeDef{Id = 200603, Abbreviations = new List<string>{ "J4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.WorkPlace};
    public static readonly PieceTypeDef J5 = new PieceTypeDef{Id = 200604, Abbreviations = new List<string>{ "J5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
}