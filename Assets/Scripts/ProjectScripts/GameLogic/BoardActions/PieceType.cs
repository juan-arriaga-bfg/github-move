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
    Enemy = 2048
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
    
    public static readonly PieceTypeDef Char1 = new PieceTypeDef{Id = 10, Abbreviations = new List<string>{ "Char1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char2 = new PieceTypeDef{Id = 11, Abbreviations = new List<string>{ "Char2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char3 = new PieceTypeDef{Id = 12, Abbreviations = new List<string>{ "Char3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char4 = new PieceTypeDef{Id = 13, Abbreviations = new List<string>{ "Char4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char5 = new PieceTypeDef{Id = 14, Abbreviations = new List<string>{ "Char5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char6 = new PieceTypeDef{Id = 15, Abbreviations = new List<string>{ "Char6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char7 = new PieceTypeDef{Id = 16, Abbreviations = new List<string>{ "Char7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char8 = new PieceTypeDef{Id = 17, Abbreviations = new List<string>{ "Char8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef Char9 = new PieceTypeDef{Id = 18, Abbreviations = new List<string>{ "Char9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    
    public static readonly PieceTypeDef A1 = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "A1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A2 = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "A2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A3Fake = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "A3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef A3 = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "A3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A4Fake = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "A4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef A4 = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "A4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A5Fake = new PieceTypeDef{Id = 106, Abbreviations = new List<string>{ "A5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef A5 = new PieceTypeDef{Id = 107, Abbreviations = new List<string>{ "A5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A6Fake = new PieceTypeDef{Id = 108, Abbreviations = new List<string>{ "A6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef A6 = new PieceTypeDef{Id = 109, Abbreviations = new List<string>{ "A6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A7Fake = new PieceTypeDef{Id = 110, Abbreviations = new List<string>{ "A7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef A7 = new PieceTypeDef{Id = 111, Abbreviations = new List<string>{ "A7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A8Fake = new PieceTypeDef{Id = 112, Abbreviations = new List<string>{ "A8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef A8 = new PieceTypeDef{Id = 113, Abbreviations = new List<string>{ "A8" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A9Fake = new PieceTypeDef{Id = 114, Abbreviations = new List<string>{ "A9Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef A9 = new PieceTypeDef{Id = 115, Abbreviations = new List<string>{ "A9" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A10Fake = new PieceTypeDef{Id = 116, Abbreviations = new List<string>{ "A10Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef A10 = new PieceTypeDef{Id = 117, Abbreviations = new List<string>{ "A10" }, Filter = PieceTypeFilter.Multicellular};
    
    public static readonly PieceTypeDef B1 = new PieceTypeDef{Id = 200, Abbreviations = new List<string>{ "B1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B2 = new PieceTypeDef{Id = 201, Abbreviations = new List<string>{ "B2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B3 = new PieceTypeDef{Id = 202, Abbreviations = new List<string>{ "B3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B4 = new PieceTypeDef{Id = 203, Abbreviations = new List<string>{ "B4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B5 = new PieceTypeDef{Id = 204, Abbreviations = new List<string>{ "B5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef C1 = new PieceTypeDef{Id = 300, Abbreviations = new List<string>{ "C1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C2 = new PieceTypeDef{Id = 301, Abbreviations = new List<string>{ "C2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C3Fake = new PieceTypeDef{Id = 302, Abbreviations = new List<string>{ "C3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C3 = new PieceTypeDef{Id = 303, Abbreviations = new List<string>{ "C3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C4Fake = new PieceTypeDef{Id = 304, Abbreviations = new List<string>{ "C4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C4 = new PieceTypeDef{Id = 305, Abbreviations = new List<string>{ "C4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C5Fake = new PieceTypeDef{Id = 306, Abbreviations = new List<string>{ "C5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C5 = new PieceTypeDef{Id = 307, Abbreviations = new List<string>{ "C5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C6Fake = new PieceTypeDef{Id = 308, Abbreviations = new List<string>{ "C6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C6 = new PieceTypeDef{Id = 309, Abbreviations = new List<string>{ "C6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C7Fake = new PieceTypeDef{Id = 310, Abbreviations = new List<string>{ "C7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C7 = new PieceTypeDef{Id = 311, Abbreviations = new List<string>{ "C7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C8Fake = new PieceTypeDef{Id = 312, Abbreviations = new List<string>{ "C8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C8 = new PieceTypeDef{Id = 313, Abbreviations = new List<string>{ "C8" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C9Fake = new PieceTypeDef{Id = 314, Abbreviations = new List<string>{ "C9Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C9 = new PieceTypeDef{Id = 315, Abbreviations = new List<string>{ "C9" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C10Fake = new PieceTypeDef{Id = 316, Abbreviations = new List<string>{ "C10Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C10 = new PieceTypeDef{Id = 317, Abbreviations = new List<string>{ "C10" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C11Fake = new PieceTypeDef{Id = 318, Abbreviations = new List<string>{ "C11Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C11 = new PieceTypeDef{Id = 319, Abbreviations = new List<string>{ "C11" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C12Fake = new PieceTypeDef{Id = 320, Abbreviations = new List<string>{ "C12Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef C12 = new PieceTypeDef{Id = 321, Abbreviations = new List<string>{ "C12" }, Filter = PieceTypeFilter.Multicellular};
    
    public static readonly PieceTypeDef D1 = new PieceTypeDef{Id = 400, Abbreviations = new List<string>{ "D1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D2 = new PieceTypeDef{Id = 401, Abbreviations = new List<string>{ "D2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D3 = new PieceTypeDef{Id = 402, Abbreviations = new List<string>{ "D3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D4 = new PieceTypeDef{Id = 403, Abbreviations = new List<string>{ "D4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D5 = new PieceTypeDef{Id = 404, Abbreviations = new List<string>{ "D5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef E1 = new PieceTypeDef{Id = 500, Abbreviations = new List<string>{ "E1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef E2 = new PieceTypeDef{Id = 501, Abbreviations = new List<string>{ "E2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef E3 = new PieceTypeDef{Id = 502, Abbreviations = new List<string>{ "E3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef E4 = new PieceTypeDef{Id = 503, Abbreviations = new List<string>{ "E4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef E5 = new PieceTypeDef{Id = 504, Abbreviations = new List<string>{ "E5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef F1 = new PieceTypeDef{Id = 600, Abbreviations = new List<string>{ "F1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F2 = new PieceTypeDef{Id = 601, Abbreviations = new List<string>{ "F2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F3 = new PieceTypeDef{Id = 602, Abbreviations = new List<string>{ "F3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F4 = new PieceTypeDef{Id = 603, Abbreviations = new List<string>{ "F4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F5 = new PieceTypeDef{Id = 604, Abbreviations = new List<string>{ "F5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef G1 = new PieceTypeDef{Id = 700, Abbreviations = new List<string>{ "G1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G2 = new PieceTypeDef{Id = 701, Abbreviations = new List<string>{ "G2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G3 = new PieceTypeDef{Id = 702, Abbreviations = new List<string>{ "G3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G4 = new PieceTypeDef{Id = 703, Abbreviations = new List<string>{ "G4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G5 = new PieceTypeDef{Id = 704, Abbreviations = new List<string>{ "G5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef H1 = new PieceTypeDef{Id = 800, Abbreviations = new List<string>{ "H1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H2 = new PieceTypeDef{Id = 801, Abbreviations = new List<string>{ "H2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H3 = new PieceTypeDef{Id = 802, Abbreviations = new List<string>{ "H3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H4 = new PieceTypeDef{Id = 803, Abbreviations = new List<string>{ "H4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H5 = new PieceTypeDef{Id = 804, Abbreviations = new List<string>{ "H5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef I1 = new PieceTypeDef{Id = 900, Abbreviations = new List<string>{ "I1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef I2 = new PieceTypeDef{Id = 901, Abbreviations = new List<string>{ "I2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef I3 = new PieceTypeDef{Id = 902, Abbreviations = new List<string>{ "I3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef I4 = new PieceTypeDef{Id = 903, Abbreviations = new List<string>{ "I4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef I5 = new PieceTypeDef{Id = 904, Abbreviations = new List<string>{ "I5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef J1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "J1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef J2 = new PieceTypeDef{Id = 1001, Abbreviations = new List<string>{ "J2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef J3 = new PieceTypeDef{Id = 1002, Abbreviations = new List<string>{ "J3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef J4 = new PieceTypeDef{Id = 1003, Abbreviations = new List<string>{ "J4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef J5 = new PieceTypeDef{Id = 1004, Abbreviations = new List<string>{ "J5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef X1 = new PieceTypeDef{Id = 1100, Abbreviations = new List<string>{ "X1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef X2 = new PieceTypeDef{Id = 1101, Abbreviations = new List<string>{ "X2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef X3 = new PieceTypeDef{Id = 1102, Abbreviations = new List<string>{ "X3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef X4 = new PieceTypeDef{Id = 1103, Abbreviations = new List<string>{ "X4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef X5 = new PieceTypeDef{Id = 1104, Abbreviations = new List<string>{ "X5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef O1 = new PieceTypeDef{Id = 5000, Abbreviations = new List<string>{ "O1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef O2 = new PieceTypeDef{Id = 5001, Abbreviations = new List<string>{ "O2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef O3 = new PieceTypeDef{Id = 5002, Abbreviations = new List<string>{ "O3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef O4 = new PieceTypeDef{Id = 5003, Abbreviations = new List<string>{ "O4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef O5 = new PieceTypeDef{Id = 5004, Abbreviations = new List<string>{ "O5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef O6 = new PieceTypeDef{Id = 5005, Abbreviations = new List<string>{ "O6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef O7 = new PieceTypeDef{Id = 5006, Abbreviations = new List<string>{ "O7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef O8 = new PieceTypeDef{Id = 5007, Abbreviations = new List<string>{ "O8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef O9 = new PieceTypeDef{Id = 5008, Abbreviations = new List<string>{ "O9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    
    public static readonly PieceTypeDef OX1 = new PieceTypeDef{Id = 5100, Abbreviations = new List<string>{ "OX1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef OX2 = new PieceTypeDef{Id = 5101, Abbreviations = new List<string>{ "OX2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef OX3 = new PieceTypeDef{Id = 5102, Abbreviations = new List<string>{ "OX3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef OX4 = new PieceTypeDef{Id = 5103, Abbreviations = new List<string>{ "OX4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef OX5 = new PieceTypeDef{Id = 5104, Abbreviations = new List<string>{ "OX5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef OX6 = new PieceTypeDef{Id = 5105, Abbreviations = new List<string>{ "OX6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef OX7 = new PieceTypeDef{Id = 5106, Abbreviations = new List<string>{ "OX7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef OX8 = new PieceTypeDef{Id = 5107, Abbreviations = new List<string>{ "OX8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    public static readonly PieceTypeDef OX9 = new PieceTypeDef{Id = 5108, Abbreviations = new List<string>{ "OX9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle};
    
    public static readonly PieceTypeDef Fog = new PieceTypeDef{Id = 5201, Abbreviations = new List<string>{ "Fog" }, Filter = PieceTypeFilter.Multicellular};
    
    public static readonly PieceTypeDef Mana1 = new PieceTypeDef{Id = 5300, Abbreviations = new List<string>{ "Mana1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef Coin1 = new PieceTypeDef{Id = 5400, Abbreviations = new List<string>{ "Coin1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin2 = new PieceTypeDef{Id = 5401, Abbreviations = new List<string>{ "Coin2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin3 = new PieceTypeDef{Id = 5402, Abbreviations = new List<string>{ "Coin3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource}; 
    public static readonly PieceTypeDef Coin4 = new PieceTypeDef{Id = 5403, Abbreviations = new List<string>{ "Coin4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin5 = new PieceTypeDef{Id = 5404, Abbreviations = new List<string>{ "Coin5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef MineC = new PieceTypeDef{Id = 5501, Abbreviations = new List<string>{ "MineC" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MineX = new PieceTypeDef{Id = 5502, Abbreviations = new List<string>{ "MineX" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MineY = new PieceTypeDef{Id = 5503, Abbreviations = new List<string>{ "MineY" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MineZ = new PieceTypeDef{Id = 5504, Abbreviations = new List<string>{ "MineZ" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
    public static readonly PieceTypeDef ChestA1 = new PieceTypeDef{Id = 13101, Abbreviations = new List<string>{ "ChestA1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestA2 = new PieceTypeDef{Id = 13102, Abbreviations = new List<string>{ "ChestA2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestA3 = new PieceTypeDef{Id = 13103, Abbreviations = new List<string>{ "ChestA3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestX1 = new PieceTypeDef{Id = 13201, Abbreviations = new List<string>{ "ChestX1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestX2 = new PieceTypeDef{Id = 13202, Abbreviations = new List<string>{ "ChestX2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestX3 = new PieceTypeDef{Id = 13203, Abbreviations = new List<string>{ "ChestX3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestC1 = new PieceTypeDef{Id = 13301, Abbreviations = new List<string>{ "ChestC1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestC2 = new PieceTypeDef{Id = 13302, Abbreviations = new List<string>{ "ChestC2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestC3 = new PieceTypeDef{Id = 13303, Abbreviations = new List<string>{ "ChestC3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestZ1 = new PieceTypeDef{Id = 13401, Abbreviations = new List<string>{ "ChestZ1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestZ2 = new PieceTypeDef{Id = 13402, Abbreviations = new List<string>{ "ChestZ2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestZ3 = new PieceTypeDef{Id = 13403, Abbreviations = new List<string>{ "ChestZ3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef Basket1 = new PieceTypeDef{Id = 13501, Abbreviations = new List<string>{ "Basket1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Energy};
    public static readonly PieceTypeDef Basket2 = new PieceTypeDef{Id = 13502, Abbreviations = new List<string>{ "Basket2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Energy};
    public static readonly PieceTypeDef Basket3 = new PieceTypeDef{Id = 13503, Abbreviations = new List<string>{ "Basket3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Energy};
    
    public static readonly PieceTypeDef Chest1 = new PieceTypeDef{Id = 13001, Abbreviations = new List<string>{ "Chest1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest2 = new PieceTypeDef{Id = 13002, Abbreviations = new List<string>{ "Chest2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest3 = new PieceTypeDef{Id = 13003, Abbreviations = new List<string>{ "Chest3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest4 = new PieceTypeDef{Id = 13004, Abbreviations = new List<string>{ "Chest4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest5 = new PieceTypeDef{Id = 13005, Abbreviations = new List<string>{ "Chest5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest6 = new PieceTypeDef{Id = 13006, Abbreviations = new List<string>{ "Chest6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest7 = new PieceTypeDef{Id = 13007, Abbreviations = new List<string>{ "Chest7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest8 = new PieceTypeDef{Id = 13008, Abbreviations = new List<string>{ "Chest8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest9 = new PieceTypeDef{Id = 13009, Abbreviations = new List<string>{ "Chest9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef Enemy1 = new PieceTypeDef{Id = 20000, Abbreviations = new List<string>{ "Enemy1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Enemy};
    
    public static readonly PieceTypeDef Magic = new PieceTypeDef{Id = 1000000, Abbreviations = new List<string>{ "Magic" }};
}