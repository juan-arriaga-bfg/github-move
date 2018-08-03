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
    Reproduction = 256
}

public partial class PieceTypeDef
{
    public PieceTypeFilter Filter { get; set; }
}

public static partial class PieceType 
{
    private readonly static Dictionary<int, PieceTypeDef> defs = new Dictionary<int, PieceTypeDef>();
    
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
    
    public static readonly PieceTypeDef A1 = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "A1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A2 = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "A2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A3 = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "A3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A4 = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "A4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A5 = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "A5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A6 = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "A6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A7 = new PieceTypeDef{Id = 106, Abbreviations = new List<string>{ "A7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A8 = new PieceTypeDef{Id = 107, Abbreviations = new List<string>{ "A8" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A9 = new PieceTypeDef{Id = 108, Abbreviations = new List<string>{ "A9" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef B1 = new PieceTypeDef{Id = 200, Abbreviations = new List<string>{ "B1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B2 = new PieceTypeDef{Id = 201, Abbreviations = new List<string>{ "B2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B3 = new PieceTypeDef{Id = 202, Abbreviations = new List<string>{ "B3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B4 = new PieceTypeDef{Id = 203, Abbreviations = new List<string>{ "B4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B5 = new PieceTypeDef{Id = 204, Abbreviations = new List<string>{ "B5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef C1 = new PieceTypeDef{Id = 300, Abbreviations = new List<string>{ "C1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C2 = new PieceTypeDef{Id = 301, Abbreviations = new List<string>{ "C2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C3 = new PieceTypeDef{Id = 302, Abbreviations = new List<string>{ "C3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C4 = new PieceTypeDef{Id = 303, Abbreviations = new List<string>{ "C4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C5 = new PieceTypeDef{Id = 304, Abbreviations = new List<string>{ "C5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C6 = new PieceTypeDef{Id = 305, Abbreviations = new List<string>{ "C6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C7 = new PieceTypeDef{Id = 306, Abbreviations = new List<string>{ "C7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C8 = new PieceTypeDef{Id = 307, Abbreviations = new List<string>{ "C8" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C9 = new PieceTypeDef{Id = 308, Abbreviations = new List<string>{ "C9" }, Filter = PieceTypeFilter.Simple};
    
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
    public static readonly PieceTypeDef E6 = new PieceTypeDef{Id = 505, Abbreviations = new List<string>{ "E6" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef F1 = new PieceTypeDef{Id = 600, Abbreviations = new List<string>{ "F1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F2 = new PieceTypeDef{Id = 601, Abbreviations = new List<string>{ "F2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F3 = new PieceTypeDef{Id = 602, Abbreviations = new List<string>{ "F3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F4 = new PieceTypeDef{Id = 603, Abbreviations = new List<string>{ "F4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef F5 = new PieceTypeDef{Id = 604, Abbreviations = new List<string>{ "F5" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef G1 = new PieceTypeDef{Id = 700, Abbreviations = new List<string>{ "G1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G2 = new PieceTypeDef{Id = 701, Abbreviations = new List<string>{ "G2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G3 = new PieceTypeDef{Id = 702, Abbreviations = new List<string>{ "G3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef G4 = new PieceTypeDef{Id = 703, Abbreviations = new List<string>{ "G4" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef H1 = new PieceTypeDef{Id = 800, Abbreviations = new List<string>{ "H1" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H2 = new PieceTypeDef{Id = 801, Abbreviations = new List<string>{ "H2" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H3 = new PieceTypeDef{Id = 802, Abbreviations = new List<string>{ "H3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef H4 = new PieceTypeDef{Id = 803, Abbreviations = new List<string>{ "H4" }, Filter = PieceTypeFilter.Simple};
    
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
    
    public static readonly PieceTypeDef King = new PieceTypeDef{Id = 5301, Abbreviations = new List<string>{ "King" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef Coin1 = new PieceTypeDef{Id = 5400, Abbreviations = new List<string>{ "Coin1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin2 = new PieceTypeDef{Id = 5401, Abbreviations = new List<string>{ "Coin2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin3 = new PieceTypeDef{Id = 5402, Abbreviations = new List<string>{ "Coin3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource}; 
    public static readonly PieceTypeDef Coin4 = new PieceTypeDef{Id = 5403, Abbreviations = new List<string>{ "Coin4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Coin5 = new PieceTypeDef{Id = 5404, Abbreviations = new List<string>{ "Coin5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef MineA = new PieceTypeDef{Id = 5500, Abbreviations = new List<string>{ "MineA" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MineC = new PieceTypeDef{Id = 5501, Abbreviations = new List<string>{ "MineC" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MineX = new PieceTypeDef{Id = 5502, Abbreviations = new List<string>{ "MineX" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
    public static readonly PieceTypeDef Mine1 = new PieceTypeDef{Id = 10000, Abbreviations = new List<string>{ "Mine1" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef Mine2 = new PieceTypeDef{Id = 10001, Abbreviations = new List<string>{ "Mine2" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef Mine3 = new PieceTypeDef{Id = 10002, Abbreviations = new List<string>{ "Mine3" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef Mine4 = new PieceTypeDef{Id = 10003, Abbreviations = new List<string>{ "Mine4" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef Mine5 = new PieceTypeDef{Id = 10004, Abbreviations = new List<string>{ "Mine5" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef Mine6 = new PieceTypeDef{Id = 10005, Abbreviations = new List<string>{ "Mine6" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef Mine7 = new PieceTypeDef{Id = 10006, Abbreviations = new List<string>{ "Mine7" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
    public static readonly PieceTypeDef Sawmill1 = new PieceTypeDef{Id = 11000, Abbreviations = new List<string>{ "Sawmill1" }};
    public static readonly PieceTypeDef Sawmill2 = new PieceTypeDef{Id = 11001, Abbreviations = new List<string>{ "Sawmill2" }};
    public static readonly PieceTypeDef Sawmill3 = new PieceTypeDef{Id = 11002, Abbreviations = new List<string>{ "Sawmill3" }};
    public static readonly PieceTypeDef Sawmill4 = new PieceTypeDef{Id = 11003, Abbreviations = new List<string>{ "Sawmill4" }};
    public static readonly PieceTypeDef Sawmill5 = new PieceTypeDef{Id = 11004, Abbreviations = new List<string>{ "Sawmill5" }};
    public static readonly PieceTypeDef Sawmill6 = new PieceTypeDef{Id = 11005, Abbreviations = new List<string>{ "Sawmill6" }};
    public static readonly PieceTypeDef Sawmill7 = new PieceTypeDef{Id = 11006, Abbreviations = new List<string>{ "Sawmill7" }};
    
    public static readonly PieceTypeDef Sheepfold1 = new PieceTypeDef{Id = 12000, Abbreviations = new List<string>{ "Sheepfold1" }};
    public static readonly PieceTypeDef Sheepfold2 = new PieceTypeDef{Id = 12001, Abbreviations = new List<string>{ "Sheepfold2" }};
    public static readonly PieceTypeDef Sheepfold3 = new PieceTypeDef{Id = 12002, Abbreviations = new List<string>{ "Sheepfold3" }};
    public static readonly PieceTypeDef Sheepfold4 = new PieceTypeDef{Id = 12003, Abbreviations = new List<string>{ "Sheepfold4" }};
    public static readonly PieceTypeDef Sheepfold5 = new PieceTypeDef{Id = 12004, Abbreviations = new List<string>{ "Sheepfold5" }};
    public static readonly PieceTypeDef Sheepfold6 = new PieceTypeDef{Id = 12005, Abbreviations = new List<string>{ "Sheepfold6" }};
    public static readonly PieceTypeDef Sheepfold7 = new PieceTypeDef{Id = 12006, Abbreviations = new List<string>{ "Sheepfold7" }};
    
    public static readonly PieceTypeDef ChestA1 = new PieceTypeDef{Id = 13101, Abbreviations = new List<string>{ "ChestA1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestA2 = new PieceTypeDef{Id = 13102, Abbreviations = new List<string>{ "ChestA2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestA3 = new PieceTypeDef{Id = 13103, Abbreviations = new List<string>{ "ChestA3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestX1 = new PieceTypeDef{Id = 13201, Abbreviations = new List<string>{ "ChestX1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestX2 = new PieceTypeDef{Id = 13202, Abbreviations = new List<string>{ "ChestX2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestX3 = new PieceTypeDef{Id = 13203, Abbreviations = new List<string>{ "ChestX3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef ChestC1 = new PieceTypeDef{Id = 13301, Abbreviations = new List<string>{ "ChestC1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestC2 = new PieceTypeDef{Id = 13302, Abbreviations = new List<string>{ "ChestC2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef ChestC3 = new PieceTypeDef{Id = 13303, Abbreviations = new List<string>{ "ChestC3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef Basket1 = new PieceTypeDef{Id = 13401, Abbreviations = new List<string>{ "Basket1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Energy};
    public static readonly PieceTypeDef Basket2 = new PieceTypeDef{Id = 13402, Abbreviations = new List<string>{ "Basket2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Energy};
    public static readonly PieceTypeDef Basket3 = new PieceTypeDef{Id = 13403, Abbreviations = new List<string>{ "Basket3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Energy};
    
    public static readonly PieceTypeDef Chest1 = new PieceTypeDef{Id = 13001, Abbreviations = new List<string>{ "Chest1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest2 = new PieceTypeDef{Id = 13002, Abbreviations = new List<string>{ "Chest2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest3 = new PieceTypeDef{Id = 13003, Abbreviations = new List<string>{ "Chest3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest4 = new PieceTypeDef{Id = 13004, Abbreviations = new List<string>{ "Chest4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest5 = new PieceTypeDef{Id = 13005, Abbreviations = new List<string>{ "Chest5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest6 = new PieceTypeDef{Id = 13006, Abbreviations = new List<string>{ "Chest6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest7 = new PieceTypeDef{Id = 13007, Abbreviations = new List<string>{ "Chest7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest8 = new PieceTypeDef{Id = 13008, Abbreviations = new List<string>{ "Chest8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef Chest9 = new PieceTypeDef{Id = 13009, Abbreviations = new List<string>{ "Chest9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef Castle1 = new PieceTypeDef{Id = 14000, Abbreviations = new List<string>{ "Castle1" }, Filter = PieceTypeFilter.Multicellular};
    public static readonly PieceTypeDef Castle2 = new PieceTypeDef{Id = 14001, Abbreviations = new List<string>{ "Castle2" }, Filter = PieceTypeFilter.Multicellular};
    public static readonly PieceTypeDef Castle3 = new PieceTypeDef{Id = 14002, Abbreviations = new List<string>{ "Castle3" }, Filter = PieceTypeFilter.Multicellular};
    public static readonly PieceTypeDef Castle4 = new PieceTypeDef{Id = 14003, Abbreviations = new List<string>{ "Castle4" }, Filter = PieceTypeFilter.Multicellular};
    public static readonly PieceTypeDef Castle5 = new PieceTypeDef{Id = 14004, Abbreviations = new List<string>{ "Castle5" }, Filter = PieceTypeFilter.Multicellular};
    public static readonly PieceTypeDef Castle6 = new PieceTypeDef{Id = 14005, Abbreviations = new List<string>{ "Castle6" }, Filter = PieceTypeFilter.Multicellular};
    public static readonly PieceTypeDef Castle7 = new PieceTypeDef{Id = 14006, Abbreviations = new List<string>{ "Castle7" }, Filter = PieceTypeFilter.Multicellular};
    public static readonly PieceTypeDef Castle8 = new PieceTypeDef{Id = 14007, Abbreviations = new List<string>{ "Castle8" }, Filter = PieceTypeFilter.Multicellular};
    public static readonly PieceTypeDef Castle9 = new PieceTypeDef{Id = 14008, Abbreviations = new List<string>{ "Castle9" }, Filter = PieceTypeFilter.Multicellular};
    
    public static readonly PieceTypeDef Market1 = new PieceTypeDef{Id = 15000, Abbreviations = new List<string>{ "Market1" }};
    public static readonly PieceTypeDef Market2 = new PieceTypeDef{Id = 15001, Abbreviations = new List<string>{ "Market2" }};
    public static readonly PieceTypeDef Market3 = new PieceTypeDef{Id = 15002, Abbreviations = new List<string>{ "Market3" }};
    public static readonly PieceTypeDef Market4 = new PieceTypeDef{Id = 15003, Abbreviations = new List<string>{ "Market4" }};
    public static readonly PieceTypeDef Market5 = new PieceTypeDef{Id = 15004, Abbreviations = new List<string>{ "Market5" }};
    public static readonly PieceTypeDef Market6 = new PieceTypeDef{Id = 15005, Abbreviations = new List<string>{ "Market6" }};
    public static readonly PieceTypeDef Market7 = new PieceTypeDef{Id = 15006, Abbreviations = new List<string>{ "Market7" }};
    public static readonly PieceTypeDef Market8 = new PieceTypeDef{Id = 15007, Abbreviations = new List<string>{ "Market8" }};
    public static readonly PieceTypeDef Market9 = new PieceTypeDef{Id = 15008, Abbreviations = new List<string>{ "Market9" }};
    
    public static readonly PieceTypeDef Storage1 = new PieceTypeDef{Id = 16000, Abbreviations = new List<string>{ "Storage1" }};
    public static readonly PieceTypeDef Storage2 = new PieceTypeDef{Id = 16001, Abbreviations = new List<string>{ "Storage2" }};
    public static readonly PieceTypeDef Storage3 = new PieceTypeDef{Id = 16002, Abbreviations = new List<string>{ "Storage3" }};
    public static readonly PieceTypeDef Storage4 = new PieceTypeDef{Id = 16003, Abbreviations = new List<string>{ "Storage4" }};
    public static readonly PieceTypeDef Storage5 = new PieceTypeDef{Id = 16004, Abbreviations = new List<string>{ "Storage5" }};
    public static readonly PieceTypeDef Storage6 = new PieceTypeDef{Id = 16005, Abbreviations = new List<string>{ "Storage6" }};
    public static readonly PieceTypeDef Storage7 = new PieceTypeDef{Id = 16006, Abbreviations = new List<string>{ "Storage7" }};
    public static readonly PieceTypeDef Storage8 = new PieceTypeDef{Id = 16007, Abbreviations = new List<string>{ "Storage8" }};
    public static readonly PieceTypeDef Storage9 = new PieceTypeDef{Id = 16008, Abbreviations = new List<string>{ "Storage9" }};
    
    public static readonly PieceTypeDef Factory1 = new PieceTypeDef{Id = 17000, Abbreviations = new List<string>{ "Factory1" }};
    public static readonly PieceTypeDef Factory2 = new PieceTypeDef{Id = 17001, Abbreviations = new List<string>{ "Factory2" }};
    public static readonly PieceTypeDef Factory3 = new PieceTypeDef{Id = 17002, Abbreviations = new List<string>{ "Factory3" }};
    public static readonly PieceTypeDef Factory4 = new PieceTypeDef{Id = 17003, Abbreviations = new List<string>{ "Factory4" }};
    public static readonly PieceTypeDef Factory5 = new PieceTypeDef{Id = 17004, Abbreviations = new List<string>{ "Factory5" }};
    public static readonly PieceTypeDef Factory6 = new PieceTypeDef{Id = 17005, Abbreviations = new List<string>{ "Factory6" }};
    public static readonly PieceTypeDef Factory7 = new PieceTypeDef{Id = 17006, Abbreviations = new List<string>{ "Factory7" }};
    public static readonly PieceTypeDef Factory8 = new PieceTypeDef{Id = 17007, Abbreviations = new List<string>{ "Factory8" }};
    public static readonly PieceTypeDef Factory9 = new PieceTypeDef{Id = 17008, Abbreviations = new List<string>{ "Factory9" }};
    
    public static readonly PieceTypeDef Zord1 = new PieceTypeDef{Id = 20001, Abbreviations = new List<string>{ "Zord1" }};
    public static readonly PieceTypeDef Zord2 = new PieceTypeDef{Id = 20002, Abbreviations = new List<string>{ "Zord2" }};
    public static readonly PieceTypeDef Zord3 = new PieceTypeDef{Id = 20003, Abbreviations = new List<string>{ "Zord3" }};
    public static readonly PieceTypeDef Zord4 = new PieceTypeDef{Id = 20004, Abbreviations = new List<string>{ "Zord4" }};
    public static readonly PieceTypeDef MegaZord = new PieceTypeDef{Id = 20000, Abbreviations = new List<string>{ "MegaZord" }};
}