using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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
    Ingredient = 128,
    Reproduction = 256,
    Character = 512,
    Fake = 1024,
    Enemy = 2048,
    Booster = 4096,
    Workplace = 8192,
    Tree = 16384,
    ProductionField = 32768, // Грядка (bed)
    Removable = 65536,
    Bag = 131072,
    Normal = 262144,
    Progress = 524288, // top_pieces (аналитика)
    OrderPiece = 1048576, // piece droped from order
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

    public static List<int> GetIdsByFilter(PieceTypeFilter include)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            // ignore empty piece
            if (def.Id == Empty.Id) continue;
            if ((int) (def.Filter & include) != (int) include) continue;
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    /// <summary>
    /// Return List of ids for pieces that have ALL of *include* and have no ANY of *exclude*
    /// </summary>
    /// <param name="include">Select pieces that have one of specified flags.
    /// Let Piece X1 has PieceTypeFilters: Simple, Obstacle, Tree 
    /// Let Piece X2 has PieceTypeFilters: Simple, Tree 
    /// Let Piece X3 has PieceTypeFilters: Simple
    /// GetIdsByFilter(Simple, Workplace) => X1, X2, X3
    /// GetIdsByFilter(Simple | Obstacle, Workplace) => X1
    /// </param>
    /// <param name="exclude">Pieces that has ANY of flag from this param will be excluded</param>
    /// Let Piece X1 has PieceTypeFilters: Simple, Obstacle, Tree 
    /// Let Piece X2 has PieceTypeFilters: Simple, Tree 
    /// Let Piece X3 has PieceTypeFilters: Simple
    /// GetIdsByFilter(Simple, Obstacle | Tree) => X3
    /// GetIdsByFilter(Simple, Obstacle) => X2, X3
    /// GetIdsByFilter(Simple, Tree) => X3
    public static List<int> GetIdsByFilter(PieceTypeFilter include, PieceTypeFilter exclude)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            // ignore empty piece
            if (def.Id == Empty.Id) continue;
            if ((int) (def.Filter & include) != (int) include) continue;
            if ((int) (def.Filter & exclude) != 0) continue; // Exclude pieces with ANY of flags from 'exclude' param
            
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
    
#region Characters
    
    public static readonly PieceTypeDef NPC_A = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "NPC_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_SleepingBeautyPlaid = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "NPC_SleepingBeautyPlaid" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_B = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "NPC_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "NPC_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};   
    public static readonly PieceTypeDef NPC_Gnome = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "NPC_Gnome" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_D = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "NPC_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E = new PieceTypeDef{Id = 106, Abbreviations = new List<string>{ "NPC_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F = new PieceTypeDef{Id = 107, Abbreviations = new List<string>{ "NPC_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_G = new PieceTypeDef{Id = 108, Abbreviations = new List<string>{ "NPC_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_H = new PieceTypeDef{Id = 109, Abbreviations = new List<string>{ "NPC_H" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_I = new PieceTypeDef{Id = 110, Abbreviations = new List<string>{ "NPC_I" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_J = new PieceTypeDef{Id = 111, Abbreviations = new List<string>{ "NPC_J" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_K = new PieceTypeDef{Id = 112, Abbreviations = new List<string>{ "NPC_K" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_L = new PieceTypeDef{Id = 113, Abbreviations = new List<string>{ "NPC_L" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_M = new PieceTypeDef{Id = 114, Abbreviations = new List<string>{ "NPC_M" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_N = new PieceTypeDef{Id = 115, Abbreviations = new List<string>{ "NPC_N" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_O = new PieceTypeDef{Id = 116, Abbreviations = new List<string>{ "NPC_O" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_P = new PieceTypeDef{Id = 117, Abbreviations = new List<string>{ "NPC_P" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_Q = new PieceTypeDef{Id = 118, Abbreviations = new List<string>{ "NPC_Q" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_R = new PieceTypeDef{Id = 119, Abbreviations = new List<string>{ "NPC_R" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress | PieceTypeFilter.Fake};
    
#endregion
    
#region Character pieces

    public static readonly PieceTypeDef NPC_B1 = new PieceTypeDef{Id = 300000, Abbreviations = new List<string>{ "NPC_B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B2 = new PieceTypeDef{Id = 300001, Abbreviations = new List<string>{ "NPC_B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B3 = new PieceTypeDef{Id = 300002, Abbreviations = new List<string>{ "NPC_B3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B4 = new PieceTypeDef{Id = 300003, Abbreviations = new List<string>{ "NPC_B4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    
    public static readonly PieceTypeDef NPC_C1 = new PieceTypeDef{Id = 300100, Abbreviations = new List<string>{ "NPC_C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C2 = new PieceTypeDef{Id = 300101, Abbreviations = new List<string>{ "NPC_C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C3 = new PieceTypeDef{Id = 300102, Abbreviations = new List<string>{ "NPC_C3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C4 = new PieceTypeDef{Id = 300103, Abbreviations = new List<string>{ "NPC_C4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C5 = new PieceTypeDef{Id = 300104, Abbreviations = new List<string>{ "NPC_C5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C6 = new PieceTypeDef{Id = 300105, Abbreviations = new List<string>{ "NPC_C6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C7 = new PieceTypeDef{Id = 300106, Abbreviations = new List<string>{ "NPC_C7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C8 = new PieceTypeDef{Id = 300107, Abbreviations = new List<string>{ "NPC_C8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    
    public static readonly PieceTypeDef NPC_D1 = new PieceTypeDef{Id = 300200, Abbreviations = new List<string>{ "NPC_D1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_D2 = new PieceTypeDef{Id = 300201, Abbreviations = new List<string>{ "NPC_D2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_D3 = new PieceTypeDef{Id = 300202, Abbreviations = new List<string>{ "NPC_D3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_D4 = new PieceTypeDef{Id = 300203, Abbreviations = new List<string>{ "NPC_D4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_D5 = new PieceTypeDef{Id = 300204, Abbreviations = new List<string>{ "NPC_D5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_D6 = new PieceTypeDef{Id = 300205, Abbreviations = new List<string>{ "NPC_D6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_D7 = new PieceTypeDef{Id = 300206, Abbreviations = new List<string>{ "NPC_D7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_D8 = new PieceTypeDef{Id = 300207, Abbreviations = new List<string>{ "NPC_D8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
     
    public static readonly PieceTypeDef NPC_E1 = new PieceTypeDef{Id = 300300, Abbreviations = new List<string>{ "NPC_E1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E2 = new PieceTypeDef{Id = 300301, Abbreviations = new List<string>{ "NPC_E2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E3 = new PieceTypeDef{Id = 300302, Abbreviations = new List<string>{ "NPC_E3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E4 = new PieceTypeDef{Id = 300303, Abbreviations = new List<string>{ "NPC_E4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E5 = new PieceTypeDef{Id = 300304, Abbreviations = new List<string>{ "NPC_E5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E6 = new PieceTypeDef{Id = 300305, Abbreviations = new List<string>{ "NPC_E6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E7 = new PieceTypeDef{Id = 300306, Abbreviations = new List<string>{ "NPC_E7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E8 = new PieceTypeDef{Id = 300307, Abbreviations = new List<string>{ "NPC_E8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
 
    public static readonly PieceTypeDef NPC_F1 = new PieceTypeDef{Id = 300400, Abbreviations = new List<string>{ "NPC_F1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F2 = new PieceTypeDef{Id = 300401, Abbreviations = new List<string>{ "NPC_F2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F3 = new PieceTypeDef{Id = 300402, Abbreviations = new List<string>{ "NPC_F3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F4 = new PieceTypeDef{Id = 300403, Abbreviations = new List<string>{ "NPC_F4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F5 = new PieceTypeDef{Id = 300404, Abbreviations = new List<string>{ "NPC_F5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F6 = new PieceTypeDef{Id = 300405, Abbreviations = new List<string>{ "NPC_F6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F7 = new PieceTypeDef{Id = 300406, Abbreviations = new List<string>{ "NPC_F7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F8 = new PieceTypeDef{Id = 300407, Abbreviations = new List<string>{ "NPC_F8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_G1 = new PieceTypeDef{Id = 300500, Abbreviations = new List<string>{ "NPC_G1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_G2 = new PieceTypeDef{Id = 300501, Abbreviations = new List<string>{ "NPC_G2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_G3 = new PieceTypeDef{Id = 300502, Abbreviations = new List<string>{ "NPC_G3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_G4 = new PieceTypeDef{Id = 300503, Abbreviations = new List<string>{ "NPC_G4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_G5 = new PieceTypeDef{Id = 300504, Abbreviations = new List<string>{ "NPC_G5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_G6 = new PieceTypeDef{Id = 300505, Abbreviations = new List<string>{ "NPC_G6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_G7 = new PieceTypeDef{Id = 300506, Abbreviations = new List<string>{ "NPC_G7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_G8 = new PieceTypeDef{Id = 300507, Abbreviations = new List<string>{ "NPC_G8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_H1 = new PieceTypeDef{Id = 300600, Abbreviations = new List<string>{ "NPC_H1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_H2 = new PieceTypeDef{Id = 300601, Abbreviations = new List<string>{ "NPC_H2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_H3 = new PieceTypeDef{Id = 300602, Abbreviations = new List<string>{ "NPC_H3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_H4 = new PieceTypeDef{Id = 300603, Abbreviations = new List<string>{ "NPC_H4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_H5 = new PieceTypeDef{Id = 300604, Abbreviations = new List<string>{ "NPC_H5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_H6 = new PieceTypeDef{Id = 300605, Abbreviations = new List<string>{ "NPC_H6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_H7 = new PieceTypeDef{Id = 300606, Abbreviations = new List<string>{ "NPC_H7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_H8 = new PieceTypeDef{Id = 300607, Abbreviations = new List<string>{ "NPC_H8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_I1 = new PieceTypeDef{Id = 300700, Abbreviations = new List<string>{ "NPC_I1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_I2 = new PieceTypeDef{Id = 300701, Abbreviations = new List<string>{ "NPC_I2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_I3 = new PieceTypeDef{Id = 300702, Abbreviations = new List<string>{ "NPC_I3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_I4 = new PieceTypeDef{Id = 300703, Abbreviations = new List<string>{ "NPC_I4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_I5 = new PieceTypeDef{Id = 300704, Abbreviations = new List<string>{ "NPC_I5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_I6 = new PieceTypeDef{Id = 300705, Abbreviations = new List<string>{ "NPC_I6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_I7 = new PieceTypeDef{Id = 300706, Abbreviations = new List<string>{ "NPC_I7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_I8 = new PieceTypeDef{Id = 300707, Abbreviations = new List<string>{ "NPC_I8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_J1 = new PieceTypeDef{Id = 300800, Abbreviations = new List<string>{ "NPC_J1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_J2 = new PieceTypeDef{Id = 300801, Abbreviations = new List<string>{ "NPC_J2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_J3 = new PieceTypeDef{Id = 300802, Abbreviations = new List<string>{ "NPC_J3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_J4 = new PieceTypeDef{Id = 300803, Abbreviations = new List<string>{ "NPC_J4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_J5 = new PieceTypeDef{Id = 300804, Abbreviations = new List<string>{ "NPC_J5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_J6 = new PieceTypeDef{Id = 300805, Abbreviations = new List<string>{ "NPC_J6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_J7 = new PieceTypeDef{Id = 300806, Abbreviations = new List<string>{ "NPC_J7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_J8 = new PieceTypeDef{Id = 300807, Abbreviations = new List<string>{ "NPC_J8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_K1 = new PieceTypeDef{Id = 300900, Abbreviations = new List<string>{ "NPC_K1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_K2 = new PieceTypeDef{Id = 300901, Abbreviations = new List<string>{ "NPC_K2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_K3 = new PieceTypeDef{Id = 300902, Abbreviations = new List<string>{ "NPC_K3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_K4 = new PieceTypeDef{Id = 300903, Abbreviations = new List<string>{ "NPC_K4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_K5 = new PieceTypeDef{Id = 300904, Abbreviations = new List<string>{ "NPC_K5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_K6 = new PieceTypeDef{Id = 300905, Abbreviations = new List<string>{ "NPC_K6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_K7 = new PieceTypeDef{Id = 300906, Abbreviations = new List<string>{ "NPC_K7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_K8 = new PieceTypeDef{Id = 300907, Abbreviations = new List<string>{ "NPC_K8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_L1 = new PieceTypeDef{Id = 301000, Abbreviations = new List<string>{ "NPC_L1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_L2 = new PieceTypeDef{Id = 301001, Abbreviations = new List<string>{ "NPC_L2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_L3 = new PieceTypeDef{Id = 301002, Abbreviations = new List<string>{ "NPC_L3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_L4 = new PieceTypeDef{Id = 301003, Abbreviations = new List<string>{ "NPC_L4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_L5 = new PieceTypeDef{Id = 301004, Abbreviations = new List<string>{ "NPC_L5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_L6 = new PieceTypeDef{Id = 301005, Abbreviations = new List<string>{ "NPC_L6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_L7 = new PieceTypeDef{Id = 301006, Abbreviations = new List<string>{ "NPC_L7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_L8 = new PieceTypeDef{Id = 301007, Abbreviations = new List<string>{ "NPC_L8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_M1 = new PieceTypeDef{Id = 301100, Abbreviations = new List<string>{ "NPC_M1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_M2 = new PieceTypeDef{Id = 301101, Abbreviations = new List<string>{ "NPC_M2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_M3 = new PieceTypeDef{Id = 301102, Abbreviations = new List<string>{ "NPC_M3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_M4 = new PieceTypeDef{Id = 301103, Abbreviations = new List<string>{ "NPC_M4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_M5 = new PieceTypeDef{Id = 301104, Abbreviations = new List<string>{ "NPC_M5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_M6 = new PieceTypeDef{Id = 301105, Abbreviations = new List<string>{ "NPC_M6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_M7 = new PieceTypeDef{Id = 301106, Abbreviations = new List<string>{ "NPC_M7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_M8 = new PieceTypeDef{Id = 301107, Abbreviations = new List<string>{ "NPC_M8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_N1 = new PieceTypeDef{Id = 301200, Abbreviations = new List<string>{ "NPC_N1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_N2 = new PieceTypeDef{Id = 301201, Abbreviations = new List<string>{ "NPC_N2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_N3 = new PieceTypeDef{Id = 301202, Abbreviations = new List<string>{ "NPC_N3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_N4 = new PieceTypeDef{Id = 301203, Abbreviations = new List<string>{ "NPC_N4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_N5 = new PieceTypeDef{Id = 301204, Abbreviations = new List<string>{ "NPC_N5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_N6 = new PieceTypeDef{Id = 301205, Abbreviations = new List<string>{ "NPC_N6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_N7 = new PieceTypeDef{Id = 301206, Abbreviations = new List<string>{ "NPC_N7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_N8 = new PieceTypeDef{Id = 301207, Abbreviations = new List<string>{ "NPC_N8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_O1 = new PieceTypeDef{Id = 301300, Abbreviations = new List<string>{ "NPC_O1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_O2 = new PieceTypeDef{Id = 301301, Abbreviations = new List<string>{ "NPC_O2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_O3 = new PieceTypeDef{Id = 301302, Abbreviations = new List<string>{ "NPC_O3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_O4 = new PieceTypeDef{Id = 301303, Abbreviations = new List<string>{ "NPC_O4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_O5 = new PieceTypeDef{Id = 301304, Abbreviations = new List<string>{ "NPC_O5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_O6 = new PieceTypeDef{Id = 301305, Abbreviations = new List<string>{ "NPC_O6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_O7 = new PieceTypeDef{Id = 301306, Abbreviations = new List<string>{ "NPC_O7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_O8 = new PieceTypeDef{Id = 301307, Abbreviations = new List<string>{ "NPC_O8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_P1 = new PieceTypeDef{Id = 301400, Abbreviations = new List<string>{ "NPC_P1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_P2 = new PieceTypeDef{Id = 301401, Abbreviations = new List<string>{ "NPC_P2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_P3 = new PieceTypeDef{Id = 301402, Abbreviations = new List<string>{ "NPC_P3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_P4 = new PieceTypeDef{Id = 301403, Abbreviations = new List<string>{ "NPC_P4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_P5 = new PieceTypeDef{Id = 301404, Abbreviations = new List<string>{ "NPC_P5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_P6 = new PieceTypeDef{Id = 301405, Abbreviations = new List<string>{ "NPC_P6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_P7 = new PieceTypeDef{Id = 301406, Abbreviations = new List<string>{ "NPC_P7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_P8 = new PieceTypeDef{Id = 301407, Abbreviations = new List<string>{ "NPC_P8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_Q1 = new PieceTypeDef{Id = 301500, Abbreviations = new List<string>{ "NPC_Q1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_Q2 = new PieceTypeDef{Id = 301501, Abbreviations = new List<string>{ "NPC_Q2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_Q3 = new PieceTypeDef{Id = 301502, Abbreviations = new List<string>{ "NPC_Q3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_Q4 = new PieceTypeDef{Id = 301503, Abbreviations = new List<string>{ "NPC_Q4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_Q5 = new PieceTypeDef{Id = 301504, Abbreviations = new List<string>{ "NPC_Q5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_Q6 = new PieceTypeDef{Id = 301505, Abbreviations = new List<string>{ "NPC_Q6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_Q7 = new PieceTypeDef{Id = 301506, Abbreviations = new List<string>{ "NPC_Q7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_Q8 = new PieceTypeDef{Id = 301507, Abbreviations = new List<string>{ "NPC_Q8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    public static readonly PieceTypeDef NPC_R1 = new PieceTypeDef{Id = 301600, Abbreviations = new List<string>{ "NPC_R1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_R2 = new PieceTypeDef{Id = 301601, Abbreviations = new List<string>{ "NPC_R2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_R3 = new PieceTypeDef{Id = 301602, Abbreviations = new List<string>{ "NPC_R3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_R4 = new PieceTypeDef{Id = 301603, Abbreviations = new List<string>{ "NPC_R4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_R5 = new PieceTypeDef{Id = 301604, Abbreviations = new List<string>{ "NPC_R5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_R6 = new PieceTypeDef{Id = 301605, Abbreviations = new List<string>{ "NPC_R6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_R7 = new PieceTypeDef{Id = 301606, Abbreviations = new List<string>{ "NPC_R7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_R8 = new PieceTypeDef{Id = 301607, Abbreviations = new List<string>{ "NPC_R8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};

    
#endregion

#region Enemies
    
    public static readonly PieceTypeDef Enemy1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "Enemy1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Enemy};
    
#endregion
    
#region Boosters
    
    public static readonly PieceTypeDef Boost_CR1 = new PieceTypeDef{Id = 1000001, Abbreviations = new List<string>{ "Boost_CR1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_CR2 = new PieceTypeDef{Id = 1000002, Abbreviations = new List<string>{ "Boost_CR2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_CR3 = new PieceTypeDef{Id = 1000003, Abbreviations = new List<string>{ "Boost_CR3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_CR  = new PieceTypeDef{Id = 1000004, Abbreviations = new List<string>{ "Boost_CR"  }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Booster};
    
    public static readonly PieceTypeDef Boost_WR = new PieceTypeDef{Id = 1000100, Abbreviations = new List<string>{ "Boost_WR" }, Filter = PieceTypeFilter.Simple};
    
#endregion
    
#region Currencies
    
    public static readonly PieceTypeDef Mana1 = new PieceTypeDef{Id = 3000, Abbreviations = new List<string>{ "Mana1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Mana2 = new PieceTypeDef{Id = 3001, Abbreviations = new List<string>{ "Mana2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Mana3 = new PieceTypeDef{Id = 3002, Abbreviations = new List<string>{ "Mana3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource}; 
    public static readonly PieceTypeDef Mana4 = new PieceTypeDef{Id = 3003, Abbreviations = new List<string>{ "Mana4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Mana5 = new PieceTypeDef{Id = 3004, Abbreviations = new List<string>{ "Mana5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Mana6 = new PieceTypeDef{Id = 3005, Abbreviations = new List<string>{ "Mana6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef Soft1 = new PieceTypeDef{Id = 3100, Abbreviations = new List<string>{ "Soft1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Soft2 = new PieceTypeDef{Id = 3101, Abbreviations = new List<string>{ "Soft2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Soft3 = new PieceTypeDef{Id = 3102, Abbreviations = new List<string>{ "Soft3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource}; 
    public static readonly PieceTypeDef Soft4 = new PieceTypeDef{Id = 3103, Abbreviations = new List<string>{ "Soft4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Soft5 = new PieceTypeDef{Id = 3104, Abbreviations = new List<string>{ "Soft5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Soft6 = new PieceTypeDef{Id = 3105, Abbreviations = new List<string>{ "Soft6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Soft7 = new PieceTypeDef{Id = 3106, Abbreviations = new List<string>{ "Soft7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Soft8 = new PieceTypeDef{Id = 3107, Abbreviations = new List<string>{ "Soft8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    public static readonly PieceTypeDef Hard1 = new PieceTypeDef{Id = 3200, Abbreviations = new List<string>{ "Hard1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Hard2 = new PieceTypeDef{Id = 3201, Abbreviations = new List<string>{ "Hard2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Hard3 = new PieceTypeDef{Id = 3202, Abbreviations = new List<string>{ "Hard3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource}; 
    public static readonly PieceTypeDef Hard4 = new PieceTypeDef{Id = 3203, Abbreviations = new List<string>{ "Hard4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Hard5 = new PieceTypeDef{Id = 3204, Abbreviations = new List<string>{ "Hard5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Hard6 = new PieceTypeDef{Id = 3205, Abbreviations = new List<string>{ "Hard6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
#endregion
    
#region Mines
    
    public static readonly PieceTypeDef MN_B      = new PieceTypeDef{Id = 5010, Abbreviations = new List<string>{ "MN_B1Fake", "MN_B" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_B1     = new PieceTypeDef{Id = 5011, Abbreviations = new List<string>{ "MN_B1" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_B2Fake = new PieceTypeDef{Id = 5012, Abbreviations = new List<string>{ "MN_B2Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_B2     = new PieceTypeDef{Id = 5013, Abbreviations = new List<string>{ "MN_B2" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_B3Fake = new PieceTypeDef{Id = 5014, Abbreviations = new List<string>{ "MN_B3Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_B3     = new PieceTypeDef{Id = 5015, Abbreviations = new List<string>{ "MN_B3" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
    public static readonly PieceTypeDef MN_C      = new PieceTypeDef{Id = 5020, Abbreviations = new List<string>{ "MN_C1Fake", "MN_C" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_C1     = new PieceTypeDef{Id = 5021, Abbreviations = new List<string>{ "MN_C1" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_C2Fake = new PieceTypeDef{Id = 5022, Abbreviations = new List<string>{ "MN_C2Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_C2     = new PieceTypeDef{Id = 5023, Abbreviations = new List<string>{ "MN_C2" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_C3Fake = new PieceTypeDef{Id = 5024, Abbreviations = new List<string>{ "MN_C3Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_C3     = new PieceTypeDef{Id = 5025, Abbreviations = new List<string>{ "MN_C3" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
    public static readonly PieceTypeDef MN_E      = new PieceTypeDef{Id = 5030, Abbreviations = new List<string>{ "MN_C1Fake", "MN_C" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_E1     = new PieceTypeDef{Id = 5031, Abbreviations = new List<string>{ "MN_E1" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_E2Fake = new PieceTypeDef{Id = 5032, Abbreviations = new List<string>{ "MN_E2Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_E2     = new PieceTypeDef{Id = 5033, Abbreviations = new List<string>{ "MN_E2" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_E3Fake = new PieceTypeDef{Id = 5034, Abbreviations = new List<string>{ "MN_E3Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_E3     = new PieceTypeDef{Id = 5035, Abbreviations = new List<string>{ "MN_E3" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
    public static readonly PieceTypeDef MN_F      = new PieceTypeDef{Id = 5040, Abbreviations = new List<string>{ "MN_F1Fake", "MN_F" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_F1     = new PieceTypeDef{Id = 5041, Abbreviations = new List<string>{ "MN_F1" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_F2Fake = new PieceTypeDef{Id = 5042, Abbreviations = new List<string>{ "MN_F2Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_F2     = new PieceTypeDef{Id = 5043, Abbreviations = new List<string>{ "MN_F2" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_F3Fake = new PieceTypeDef{Id = 5044, Abbreviations = new List<string>{ "MN_F3Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_F3     = new PieceTypeDef{Id = 5045, Abbreviations = new List<string>{ "MN_F3" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
    public static readonly PieceTypeDef MN_H      = new PieceTypeDef{Id = 5050, Abbreviations = new List<string>{ "MN_H1Fake", "MN_H" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_H1     = new PieceTypeDef{Id = 5051, Abbreviations = new List<string>{ "MN_H1" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_H2Fake = new PieceTypeDef{Id = 5052, Abbreviations = new List<string>{ "MN_H2Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_H2     = new PieceTypeDef{Id = 5053, Abbreviations = new List<string>{ "MN_H2" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_H3Fake = new PieceTypeDef{Id = 5054, Abbreviations = new List<string>{ "MN_H3Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_H3     = new PieceTypeDef{Id = 5055, Abbreviations = new List<string>{ "MN_H3" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
    public static readonly PieceTypeDef MN_I      = new PieceTypeDef{Id = 5060, Abbreviations = new List<string>{ "MN_I1Fake", "MN_I" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_I1     = new PieceTypeDef{Id = 5061, Abbreviations = new List<string>{ "MN_I1" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_I2Fake = new PieceTypeDef{Id = 5062, Abbreviations = new List<string>{ "MN_I2Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_I2     = new PieceTypeDef{Id = 5063, Abbreviations = new List<string>{ "MN_I2" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_I3Fake = new PieceTypeDef{Id = 5064, Abbreviations = new List<string>{ "MN_I3Fake" },         Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_I3     = new PieceTypeDef{Id = 5065, Abbreviations = new List<string>{ "MN_I3" },             Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    
#endregion
    
#region Chests
    
    public static readonly PieceTypeDef CH_Free = new PieceTypeDef{Id = 10001, Abbreviations = new List<string>{ "CH_Free" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH_NPC = new PieceTypeDef{Id = 10002, Abbreviations = new List<string>{ "CH_NPC" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef SK1_PR = new PieceTypeDef{Id = 10011, Abbreviations = new List<string>{ "SK1_PR" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Bag};
    public static readonly PieceTypeDef SK2_PR = new PieceTypeDef{Id = 10012, Abbreviations = new List<string>{ "SK2_PR" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Bag};
    public static readonly PieceTypeDef SK3_PR = new PieceTypeDef{Id = 10013, Abbreviations = new List<string>{ "SK3_PR" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Bag};
    
    public static readonly PieceTypeDef CH1_A = new PieceTypeDef{Id = 10100, Abbreviations = new List<string>{ "CH1_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_A = new PieceTypeDef{Id = 10101, Abbreviations = new List<string>{ "CH2_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_A = new PieceTypeDef{Id = 10102, Abbreviations = new List<string>{ "CH3_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_B = new PieceTypeDef{Id = 10201, Abbreviations = new List<string>{ "CH1_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_B = new PieceTypeDef{Id = 10202, Abbreviations = new List<string>{ "CH2_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_B = new PieceTypeDef{Id = 10203, Abbreviations = new List<string>{ "CH3_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_C = new PieceTypeDef{Id = 10301, Abbreviations = new List<string>{ "CH1_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_C = new PieceTypeDef{Id = 10302, Abbreviations = new List<string>{ "CH2_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_C = new PieceTypeDef{Id = 10303, Abbreviations = new List<string>{ "CH3_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_D = new PieceTypeDef{Id = 10401, Abbreviations = new List<string>{ "CH1_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_D = new PieceTypeDef{Id = 10402, Abbreviations = new List<string>{ "CH2_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_D = new PieceTypeDef{Id = 10403, Abbreviations = new List<string>{ "CH3_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_E = new PieceTypeDef{Id = 10501, Abbreviations = new List<string>{ "CH1_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_E = new PieceTypeDef{Id = 10502, Abbreviations = new List<string>{ "CH2_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_E = new PieceTypeDef{Id = 10503, Abbreviations = new List<string>{ "CH3_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_F = new PieceTypeDef{Id = 10601, Abbreviations = new List<string>{ "CH1_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_F = new PieceTypeDef{Id = 10602, Abbreviations = new List<string>{ "CH2_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_F = new PieceTypeDef{Id = 10603, Abbreviations = new List<string>{ "CH3_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_G = new PieceTypeDef{Id = 10701, Abbreviations = new List<string>{ "CH1_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_G = new PieceTypeDef{Id = 10702, Abbreviations = new List<string>{ "CH2_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_G = new PieceTypeDef{Id = 10703, Abbreviations = new List<string>{ "CH3_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_H = new PieceTypeDef{Id = 10801, Abbreviations = new List<string>{ "CH1_H" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_H = new PieceTypeDef{Id = 10802, Abbreviations = new List<string>{ "CH2_H" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_H = new PieceTypeDef{Id = 10803, Abbreviations = new List<string>{ "CH3_H" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_I = new PieceTypeDef{Id = 10901, Abbreviations = new List<string>{ "CH1_I" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_I = new PieceTypeDef{Id = 10902, Abbreviations = new List<string>{ "CH2_I" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_I = new PieceTypeDef{Id = 10903, Abbreviations = new List<string>{ "CH3_I" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
#endregion
    
#region Obstacles
    
    public static readonly PieceTypeDef Fog = new PieceTypeDef{Id = 20000, Abbreviations = new List<string>{ "Fog" }, Filter = PieceTypeFilter.Multicellular};
    
    public static readonly PieceTypeDef OB1_TT = new PieceTypeDef{Id = 20001, Abbreviations = new List<string>{ "OB1_TT" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_TT = new PieceTypeDef{Id = 20002, Abbreviations = new List<string>{ "OB2_TT" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    
    public static readonly PieceTypeDef OB1_A = new PieceTypeDef{Id = 20100, Abbreviations = new List<string>{ "OB1_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_A = new PieceTypeDef{Id = 20101, Abbreviations = new List<string>{ "OB2_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB3_A = new PieceTypeDef{Id = 20102, Abbreviations = new List<string>{ "OB3_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB4_A = new PieceTypeDef{Id = 20103, Abbreviations = new List<string>{ "OB4_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB5_A = new PieceTypeDef{Id = 20104, Abbreviations = new List<string>{ "OB5_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB6_A = new PieceTypeDef{Id = 20105, Abbreviations = new List<string>{ "OB6_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB7_A = new PieceTypeDef{Id = 20106, Abbreviations = new List<string>{ "OB7_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB8_A = new PieceTypeDef{Id = 20107, Abbreviations = new List<string>{ "OB8_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB9_A = new PieceTypeDef{Id = 20108, Abbreviations = new List<string>{ "OB9_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    
    public static readonly PieceTypeDef OB_PR_A = new PieceTypeDef{Id = 20200, Abbreviations = new List<string>{ "OB_PR_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_B = new PieceTypeDef{Id = 20201, Abbreviations = new List<string>{ "OB_PR_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_C = new PieceTypeDef{Id = 20202, Abbreviations = new List<string>{ "OB_PR_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_D = new PieceTypeDef{Id = 20203, Abbreviations = new List<string>{ "OB_PR_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_E = new PieceTypeDef{Id = 20204, Abbreviations = new List<string>{ "OB_PR_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_F = new PieceTypeDef{Id = 20205, Abbreviations = new List<string>{ "OB_PR_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_G = new PieceTypeDef{Id = 20206, Abbreviations = new List<string>{ "OB_PR_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};

    public static readonly PieceTypeDef OB1_G = new PieceTypeDef{Id = 20300, Abbreviations = new List<string>{ "OB1_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_G = new PieceTypeDef{Id = 20301, Abbreviations = new List<string>{ "OB2_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB3_G = new PieceTypeDef{Id = 20302, Abbreviations = new List<string>{ "OB3_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB4_G = new PieceTypeDef{Id = 20303, Abbreviations = new List<string>{ "OB4_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB5_G = new PieceTypeDef{Id = 20304, Abbreviations = new List<string>{ "OB5_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    
    public static readonly PieceTypeDef OB1_D = new PieceTypeDef{Id = 20400, Abbreviations = new List<string>{ "OB1_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_D = new PieceTypeDef{Id = 20401, Abbreviations = new List<string>{ "OB2_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB3_D = new PieceTypeDef{Id = 20402, Abbreviations = new List<string>{ "OB3_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB4_D = new PieceTypeDef{Id = 20403, Abbreviations = new List<string>{ "OB4_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB5_D = new PieceTypeDef{Id = 20404, Abbreviations = new List<string>{ "OB5_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB6_D = new PieceTypeDef{Id = 20405, Abbreviations = new List<string>{ "OB6_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB7_D = new PieceTypeDef{Id = 20406, Abbreviations = new List<string>{ "OB7_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB8_D = new PieceTypeDef{Id = 20407, Abbreviations = new List<string>{ "OB8_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB9_D = new PieceTypeDef{Id = 20408, Abbreviations = new List<string>{ "OB9_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    
#endregion
    
#region Simple Pieces
    
    #region A
    
    public static readonly PieceTypeDef A1     = new PieceTypeDef{Id = 100000, Abbreviations = new List<string>{ "A1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A2     = new PieceTypeDef{Id = 100001, Abbreviations = new List<string>{ "A2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A3Fake = new PieceTypeDef{Id = 100002, Abbreviations = new List<string>{ "A3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A3     = new PieceTypeDef{Id = 100003, Abbreviations = new List<string>{ "A3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A4Fake = new PieceTypeDef{Id = 100004, Abbreviations = new List<string>{ "A4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A4     = new PieceTypeDef{Id = 100005, Abbreviations = new List<string>{ "A4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A5Fake = new PieceTypeDef{Id = 100006, Abbreviations = new List<string>{ "A5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A5     = new PieceTypeDef{Id = 100007, Abbreviations = new List<string>{ "A5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A6Fake = new PieceTypeDef{Id = 100008, Abbreviations = new List<string>{ "A6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A6     = new PieceTypeDef{Id = 100009, Abbreviations = new List<string>{ "A6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A7Fake = new PieceTypeDef{Id = 100010, Abbreviations = new List<string>{ "A7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A7     = new PieceTypeDef{Id = 100011, Abbreviations = new List<string>{ "A7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A8Fake = new PieceTypeDef{Id = 100012, Abbreviations = new List<string>{ "A8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A8     = new PieceTypeDef{Id = 100013, Abbreviations = new List<string>{ "A8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A9Fake = new PieceTypeDef{Id = 100014, Abbreviations = new List<string>{ "A9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef A9     = new PieceTypeDef{Id = 100015, Abbreviations = new List<string>{ "A9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
    #region B
    
    public static readonly PieceTypeDef B1      = new PieceTypeDef{Id = 100100, Abbreviations = new List<string>{ "B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B2      = new PieceTypeDef{Id = 100101, Abbreviations = new List<string>{ "B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B3Fake  = new PieceTypeDef{Id = 100102, Abbreviations = new List<string>{ "B3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B3      = new PieceTypeDef{Id = 100103, Abbreviations = new List<string>{ "B3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B4Fake  = new PieceTypeDef{Id = 100104, Abbreviations = new List<string>{ "B4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B4      = new PieceTypeDef{Id = 100105, Abbreviations = new List<string>{ "B4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B5Fake  = new PieceTypeDef{Id = 100106, Abbreviations = new List<string>{ "B5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B5      = new PieceTypeDef{Id = 100107, Abbreviations = new List<string>{ "B5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B6Fake  = new PieceTypeDef{Id = 100108, Abbreviations = new List<string>{ "B6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B6      = new PieceTypeDef{Id = 100109, Abbreviations = new List<string>{ "B6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B7Fake  = new PieceTypeDef{Id = 100110, Abbreviations = new List<string>{ "B7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B7      = new PieceTypeDef{Id = 100111, Abbreviations = new List<string>{ "B7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B8Fake  = new PieceTypeDef{Id = 100112, Abbreviations = new List<string>{ "B8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B8      = new PieceTypeDef{Id = 100113, Abbreviations = new List<string>{ "B8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B9Fake  = new PieceTypeDef{Id = 100114, Abbreviations = new List<string>{ "B9Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B9      = new PieceTypeDef{Id = 100115, Abbreviations = new List<string>{ "B9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B10Fake = new PieceTypeDef{Id = 100116, Abbreviations = new List<string>{ "B10Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B10     = new PieceTypeDef{Id = 100117, Abbreviations = new List<string>{ "B10" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B11Fake = new PieceTypeDef{Id = 100118, Abbreviations = new List<string>{ "B11Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef B11     = new PieceTypeDef{Id = 100119, Abbreviations = new List<string>{ "B11" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
    #region C
    
    public static readonly PieceTypeDef C1     = new PieceTypeDef{Id = 100200, Abbreviations = new List<string>{ "C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C2     = new PieceTypeDef{Id = 100201, Abbreviations = new List<string>{ "C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C3Fake = new PieceTypeDef{Id = 100202, Abbreviations = new List<string>{ "C3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C3     = new PieceTypeDef{Id = 100203, Abbreviations = new List<string>{ "C3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C4Fake = new PieceTypeDef{Id = 100204, Abbreviations = new List<string>{ "C4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C4     = new PieceTypeDef{Id = 100205, Abbreviations = new List<string>{ "C4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C5Fake = new PieceTypeDef{Id = 100206, Abbreviations = new List<string>{ "C5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C5     = new PieceTypeDef{Id = 100207, Abbreviations = new List<string>{ "C5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C6Fake = new PieceTypeDef{Id = 100208, Abbreviations = new List<string>{ "C6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C6     = new PieceTypeDef{Id = 100209, Abbreviations = new List<string>{ "C6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C7Fake = new PieceTypeDef{Id = 100210, Abbreviations = new List<string>{ "C7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C7     = new PieceTypeDef{Id = 100211, Abbreviations = new List<string>{ "C7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C8Fake = new PieceTypeDef{Id = 100212, Abbreviations = new List<string>{ "C8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C8     = new PieceTypeDef{Id = 100213, Abbreviations = new List<string>{ "C8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C9Fake = new PieceTypeDef{Id = 100214, Abbreviations = new List<string>{ "C9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef C9     = new PieceTypeDef{Id = 100215, Abbreviations = new List<string>{ "C9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
    #region D
    
    public static readonly PieceTypeDef D1     = new PieceTypeDef{Id = 100300, Abbreviations = new List<string>{ "D1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D2     = new PieceTypeDef{Id = 100301, Abbreviations = new List<string>{ "D2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D3Fake = new PieceTypeDef{Id = 100302, Abbreviations = new List<string>{ "D3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D3     = new PieceTypeDef{Id = 100303, Abbreviations = new List<string>{ "D3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D4Fake = new PieceTypeDef{Id = 100304, Abbreviations = new List<string>{ "D4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D4     = new PieceTypeDef{Id = 100305, Abbreviations = new List<string>{ "D4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D5Fake = new PieceTypeDef{Id = 100306, Abbreviations = new List<string>{ "D5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D5     = new PieceTypeDef{Id = 100307, Abbreviations = new List<string>{ "D5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D6Fake = new PieceTypeDef{Id = 100308, Abbreviations = new List<string>{ "D6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D6     = new PieceTypeDef{Id = 100309, Abbreviations = new List<string>{ "D6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D7Fake = new PieceTypeDef{Id = 100310, Abbreviations = new List<string>{ "D7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D7     = new PieceTypeDef{Id = 100311, Abbreviations = new List<string>{ "D7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D8Fake = new PieceTypeDef{Id = 100312, Abbreviations = new List<string>{ "D8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D8     = new PieceTypeDef{Id = 100313, Abbreviations = new List<string>{ "D8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D9Fake = new PieceTypeDef{Id = 100314, Abbreviations = new List<string>{ "D9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef D9     = new PieceTypeDef{Id = 100315, Abbreviations = new List<string>{ "D9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
    #region E
    
    public static readonly PieceTypeDef E1     = new PieceTypeDef{Id = 100400, Abbreviations = new List<string>{ "E1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E2     = new PieceTypeDef{Id = 100401, Abbreviations = new List<string>{ "E2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E3Fake = new PieceTypeDef{Id = 100402, Abbreviations = new List<string>{ "E3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E3     = new PieceTypeDef{Id = 100403, Abbreviations = new List<string>{ "E3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E4Fake = new PieceTypeDef{Id = 100404, Abbreviations = new List<string>{ "E4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E4     = new PieceTypeDef{Id = 100405, Abbreviations = new List<string>{ "E4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E5Fake = new PieceTypeDef{Id = 100406, Abbreviations = new List<string>{ "E5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E5     = new PieceTypeDef{Id = 100407, Abbreviations = new List<string>{ "E5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E6Fake = new PieceTypeDef{Id = 100408, Abbreviations = new List<string>{ "E6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E6     = new PieceTypeDef{Id = 100409, Abbreviations = new List<string>{ "E6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E7Fake = new PieceTypeDef{Id = 100410, Abbreviations = new List<string>{ "E7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E7     = new PieceTypeDef{Id = 100411, Abbreviations = new List<string>{ "E7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E8Fake = new PieceTypeDef{Id = 100412, Abbreviations = new List<string>{ "E8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E8     = new PieceTypeDef{Id = 100413, Abbreviations = new List<string>{ "E8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E9Fake = new PieceTypeDef{Id = 100414, Abbreviations = new List<string>{ "E9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef E9     = new PieceTypeDef{Id = 100415, Abbreviations = new List<string>{ "E9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
  
    #region F
    
    public static readonly PieceTypeDef F1     = new PieceTypeDef{Id = 100500, Abbreviations = new List<string>{ "F1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F2     = new PieceTypeDef{Id = 100501, Abbreviations = new List<string>{ "F2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F3Fake = new PieceTypeDef{Id = 100502, Abbreviations = new List<string>{ "F3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F3     = new PieceTypeDef{Id = 100503, Abbreviations = new List<string>{ "F3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F4Fake = new PieceTypeDef{Id = 100504, Abbreviations = new List<string>{ "F4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F4     = new PieceTypeDef{Id = 100505, Abbreviations = new List<string>{ "F4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F5Fake = new PieceTypeDef{Id = 100506, Abbreviations = new List<string>{ "F5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F5     = new PieceTypeDef{Id = 100507, Abbreviations = new List<string>{ "F5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F6Fake = new PieceTypeDef{Id = 100508, Abbreviations = new List<string>{ "F6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F6     = new PieceTypeDef{Id = 100509, Abbreviations = new List<string>{ "F6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F7Fake = new PieceTypeDef{Id = 100510, Abbreviations = new List<string>{ "F7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F7     = new PieceTypeDef{Id = 100511, Abbreviations = new List<string>{ "F7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F8Fake = new PieceTypeDef{Id = 100512, Abbreviations = new List<string>{ "F8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F8     = new PieceTypeDef{Id = 100513, Abbreviations = new List<string>{ "F8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F9Fake = new PieceTypeDef{Id = 100514, Abbreviations = new List<string>{ "F9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef F9     = new PieceTypeDef{Id = 100515, Abbreviations = new List<string>{ "F9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
    #region G
    
    public static readonly PieceTypeDef G1     = new PieceTypeDef{Id = 100600, Abbreviations = new List<string>{ "G1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G2     = new PieceTypeDef{Id = 100601, Abbreviations = new List<string>{ "G2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G3Fake = new PieceTypeDef{Id = 100602, Abbreviations = new List<string>{ "G3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G3     = new PieceTypeDef{Id = 100603, Abbreviations = new List<string>{ "G3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G4Fake = new PieceTypeDef{Id = 100604, Abbreviations = new List<string>{ "G4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G4     = new PieceTypeDef{Id = 100605, Abbreviations = new List<string>{ "G4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G5Fake = new PieceTypeDef{Id = 100606, Abbreviations = new List<string>{ "G5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G5     = new PieceTypeDef{Id = 100607, Abbreviations = new List<string>{ "G5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G6Fake = new PieceTypeDef{Id = 100608, Abbreviations = new List<string>{ "G6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G6     = new PieceTypeDef{Id = 100609, Abbreviations = new List<string>{ "G6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G7Fake = new PieceTypeDef{Id = 100610, Abbreviations = new List<string>{ "G7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G7     = new PieceTypeDef{Id = 100611, Abbreviations = new List<string>{ "G7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G8Fake = new PieceTypeDef{Id = 100612, Abbreviations = new List<string>{ "G8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G8     = new PieceTypeDef{Id = 100613, Abbreviations = new List<string>{ "G8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G9Fake = new PieceTypeDef{Id = 100614, Abbreviations = new List<string>{ "G9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef G9     = new PieceTypeDef{Id = 100615, Abbreviations = new List<string>{ "G9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
    #region H
    
    public static readonly PieceTypeDef H1     = new PieceTypeDef{Id = 100700, Abbreviations = new List<string>{ "H1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H2     = new PieceTypeDef{Id = 100701, Abbreviations = new List<string>{ "H2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H3Fake = new PieceTypeDef{Id = 100702, Abbreviations = new List<string>{ "H3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H3     = new PieceTypeDef{Id = 100703, Abbreviations = new List<string>{ "H3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H4Fake = new PieceTypeDef{Id = 100704, Abbreviations = new List<string>{ "H4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H4     = new PieceTypeDef{Id = 100705, Abbreviations = new List<string>{ "H4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H5Fake = new PieceTypeDef{Id = 100706, Abbreviations = new List<string>{ "H5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H5     = new PieceTypeDef{Id = 100707, Abbreviations = new List<string>{ "H5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H6Fake = new PieceTypeDef{Id = 100708, Abbreviations = new List<string>{ "H6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H6     = new PieceTypeDef{Id = 100709, Abbreviations = new List<string>{ "H6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H7Fake = new PieceTypeDef{Id = 100710, Abbreviations = new List<string>{ "H7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H7     = new PieceTypeDef{Id = 100711, Abbreviations = new List<string>{ "H7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H8Fake = new PieceTypeDef{Id = 100712, Abbreviations = new List<string>{ "H8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H8     = new PieceTypeDef{Id = 100713, Abbreviations = new List<string>{ "H8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H9Fake = new PieceTypeDef{Id = 100714, Abbreviations = new List<string>{ "H9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef H9     = new PieceTypeDef{Id = 100715, Abbreviations = new List<string>{ "H9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
    #region I
    
    public static readonly PieceTypeDef I1     = new PieceTypeDef{Id = 100800, Abbreviations = new List<string>{ "I1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I2     = new PieceTypeDef{Id = 100801, Abbreviations = new List<string>{ "I2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I3Fake = new PieceTypeDef{Id = 100802, Abbreviations = new List<string>{ "I3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I3     = new PieceTypeDef{Id = 100803, Abbreviations = new List<string>{ "I3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I4Fake = new PieceTypeDef{Id = 100804, Abbreviations = new List<string>{ "I4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I4     = new PieceTypeDef{Id = 100805, Abbreviations = new List<string>{ "I4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I5Fake = new PieceTypeDef{Id = 100806, Abbreviations = new List<string>{ "I5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I5     = new PieceTypeDef{Id = 100807, Abbreviations = new List<string>{ "I5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I6Fake = new PieceTypeDef{Id = 100808, Abbreviations = new List<string>{ "I6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I6     = new PieceTypeDef{Id = 100809, Abbreviations = new List<string>{ "I6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I7Fake = new PieceTypeDef{Id = 100810, Abbreviations = new List<string>{ "I7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I7     = new PieceTypeDef{Id = 100811, Abbreviations = new List<string>{ "I7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I8Fake = new PieceTypeDef{Id = 100812, Abbreviations = new List<string>{ "I8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I8     = new PieceTypeDef{Id = 100813, Abbreviations = new List<string>{ "I8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I9Fake = new PieceTypeDef{Id = 100814, Abbreviations = new List<string>{ "I9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef I9     = new PieceTypeDef{Id = 100815, Abbreviations = new List<string>{ "I9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
    #region J
    
    public static readonly PieceTypeDef J1     = new PieceTypeDef{Id = 100900, Abbreviations = new List<string>{ "J1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J2     = new PieceTypeDef{Id = 100901, Abbreviations = new List<string>{ "J2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J3Fake = new PieceTypeDef{Id = 100902, Abbreviations = new List<string>{ "J3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J3     = new PieceTypeDef{Id = 100903, Abbreviations = new List<string>{ "J3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J4Fake = new PieceTypeDef{Id = 100904, Abbreviations = new List<string>{ "J4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J4     = new PieceTypeDef{Id = 100905, Abbreviations = new List<string>{ "J4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J5Fake = new PieceTypeDef{Id = 100906, Abbreviations = new List<string>{ "J5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J5     = new PieceTypeDef{Id = 100907, Abbreviations = new List<string>{ "J5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J6Fake = new PieceTypeDef{Id = 100908, Abbreviations = new List<string>{ "J6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J6     = new PieceTypeDef{Id = 100909, Abbreviations = new List<string>{ "J6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J7Fake = new PieceTypeDef{Id = 100910, Abbreviations = new List<string>{ "J7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J7     = new PieceTypeDef{Id = 100911, Abbreviations = new List<string>{ "J7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J8Fake = new PieceTypeDef{Id = 100912, Abbreviations = new List<string>{ "J8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J8     = new PieceTypeDef{Id = 100913, Abbreviations = new List<string>{ "J8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J9Fake = new PieceTypeDef{Id = 100914, Abbreviations = new List<string>{ "J9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef J9     = new PieceTypeDef{Id = 100915, Abbreviations = new List<string>{ "J9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
#endregion

#region Reproduction Pieces
    
    #region PR_A
    
    public static readonly PieceTypeDef PR_A1 = new PieceTypeDef{Id = 200000, Abbreviations = new List<string>{ "PR_A1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A2 = new PieceTypeDef{Id = 200001, Abbreviations = new List<string>{ "PR_A2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A3 = new PieceTypeDef{Id = 200002, Abbreviations = new List<string>{ "PR_A3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A4 = new PieceTypeDef{Id = 200003, Abbreviations = new List<string>{ "PR_A4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_A5 = new PieceTypeDef{Id = 200004, Abbreviations = new List<string>{ "PR_A5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_B
    
    public static readonly PieceTypeDef PR_B1 = new PieceTypeDef{Id = 200100, Abbreviations = new List<string>{ "PR_B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B2 = new PieceTypeDef{Id = 200101, Abbreviations = new List<string>{ "PR_B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B3 = new PieceTypeDef{Id = 200102, Abbreviations = new List<string>{ "PR_B3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B4 = new PieceTypeDef{Id = 200103, Abbreviations = new List<string>{ "PR_B4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_B5 = new PieceTypeDef{Id = 200104, Abbreviations = new List<string>{ "PR_B5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_C
    
    public static readonly PieceTypeDef PR_C1 = new PieceTypeDef{Id = 200200, Abbreviations = new List<string>{ "PR_C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C2 = new PieceTypeDef{Id = 200201, Abbreviations = new List<string>{ "PR_C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C3 = new PieceTypeDef{Id = 200202, Abbreviations = new List<string>{ "PR_C3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C4 = new PieceTypeDef{Id = 200203, Abbreviations = new List<string>{ "PR_C4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_C5 = new PieceTypeDef{Id = 200204, Abbreviations = new List<string>{ "PR_C5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_D
    
    public static readonly PieceTypeDef PR_D1 = new PieceTypeDef{Id = 200300, Abbreviations = new List<string>{ "PR_D1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D2 = new PieceTypeDef{Id = 200301, Abbreviations = new List<string>{ "PR_D2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D3 = new PieceTypeDef{Id = 200302, Abbreviations = new List<string>{ "PR_D3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D4 = new PieceTypeDef{Id = 200303, Abbreviations = new List<string>{ "PR_D4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_D5 = new PieceTypeDef{Id = 200304, Abbreviations = new List<string>{ "PR_D5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_E
    
    public static readonly PieceTypeDef PR_E1 = new PieceTypeDef{Id = 200400, Abbreviations = new List<string>{ "PR_E1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E2 = new PieceTypeDef{Id = 200401, Abbreviations = new List<string>{ "PR_E2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E3 = new PieceTypeDef{Id = 200402, Abbreviations = new List<string>{ "PR_E3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E4 = new PieceTypeDef{Id = 200403, Abbreviations = new List<string>{ "PR_E4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_E5 = new PieceTypeDef{Id = 200404, Abbreviations = new List<string>{ "PR_E5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_F
    
    public static readonly PieceTypeDef PR_F1 = new PieceTypeDef{Id = 200500, Abbreviations = new List<string>{ "PR_F1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F2 = new PieceTypeDef{Id = 200501, Abbreviations = new List<string>{ "PR_F2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F3 = new PieceTypeDef{Id = 200502, Abbreviations = new List<string>{ "PR_F3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F4 = new PieceTypeDef{Id = 200503, Abbreviations = new List<string>{ "PR_F4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_F5 = new PieceTypeDef{Id = 200504, Abbreviations = new List<string>{ "PR_F5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_G
    
    public static readonly PieceTypeDef PR_G1 = new PieceTypeDef{Id = 200600, Abbreviations = new List<string>{ "PR_G1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G2 = new PieceTypeDef{Id = 200601, Abbreviations = new List<string>{ "PR_G2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G3 = new PieceTypeDef{Id = 200602, Abbreviations = new List<string>{ "PR_G3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G4 = new PieceTypeDef{Id = 200603, Abbreviations = new List<string>{ "PR_G4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_G5 = new PieceTypeDef{Id = 200604, Abbreviations = new List<string>{ "PR_G5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
#endregion
    
#region Orders
    
    public static readonly PieceTypeDef Order1 = new PieceTypeDef{Id = 400001, Abbreviations = new List<string>{ "Order1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order2 = new PieceTypeDef{Id = 400002, Abbreviations = new List<string>{ "Order2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order3 = new PieceTypeDef{Id = 400003, Abbreviations = new List<string>{ "Order3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order4 = new PieceTypeDef{Id = 400004, Abbreviations = new List<string>{ "Order4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order5 = new PieceTypeDef{Id = 400005, Abbreviations = new List<string>{ "Order5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order6 = new PieceTypeDef{Id = 400006, Abbreviations = new List<string>{ "Order6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order7 = new PieceTypeDef{Id = 400007, Abbreviations = new List<string>{ "Order7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order8 = new PieceTypeDef{Id = 400008, Abbreviations = new List<string>{ "Order8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order9 = new PieceTypeDef{Id = 400009, Abbreviations = new List<string>{ "Order9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order10 = new PieceTypeDef{Id = 400010, Abbreviations = new List<string>{ "Order10" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order11 = new PieceTypeDef{Id = 400011, Abbreviations = new List<string>{ "Order11" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order12 = new PieceTypeDef{Id = 400012, Abbreviations = new List<string>{ "Order12" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order13 = new PieceTypeDef{Id = 400013, Abbreviations = new List<string>{ "Order13" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order14 = new PieceTypeDef{Id = 400014, Abbreviations = new List<string>{ "Order14" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order15 = new PieceTypeDef{Id = 400015, Abbreviations = new List<string>{ "Order15" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order16 = new PieceTypeDef{Id = 400016, Abbreviations = new List<string>{ "Order16" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order17 = new PieceTypeDef{Id = 400017, Abbreviations = new List<string>{ "Order17" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order18 = new PieceTypeDef{Id = 400018, Abbreviations = new List<string>{ "Order18" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order19 = new PieceTypeDef{Id = 400019, Abbreviations = new List<string>{ "Order19" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order20 = new PieceTypeDef{Id = 400020, Abbreviations = new List<string>{ "Order20" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order21 = new PieceTypeDef{Id = 400021, Abbreviations = new List<string>{ "Order21" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order22 = new PieceTypeDef{Id = 400022, Abbreviations = new List<string>{ "Order22" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef Order23 = new PieceTypeDef{Id = 400023, Abbreviations = new List<string>{ "Order23" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    

#endregion

#region Extended Pieces

    #region EXT_A
    
    public static readonly PieceTypeDef EXT_A1     = new PieceTypeDef{Id = 500000, Abbreviations = new List<string>{ "EXT_A1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A2     = new PieceTypeDef{Id = 500001, Abbreviations = new List<string>{ "EXT_A2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A3Fake = new PieceTypeDef{Id = 500002, Abbreviations = new List<string>{ "EXT_A3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A3     = new PieceTypeDef{Id = 500003, Abbreviations = new List<string>{ "EXT_A3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A4Fake = new PieceTypeDef{Id = 500004, Abbreviations = new List<string>{ "EXT_A4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A4     = new PieceTypeDef{Id = 500005, Abbreviations = new List<string>{ "EXT_A4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A5Fake = new PieceTypeDef{Id = 500006, Abbreviations = new List<string>{ "EXT_A5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A5     = new PieceTypeDef{Id = 500007, Abbreviations = new List<string>{ "EXT_A5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A6Fake = new PieceTypeDef{Id = 500008, Abbreviations = new List<string>{ "EXT_A6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A6     = new PieceTypeDef{Id = 500009, Abbreviations = new List<string>{ "EXT_A6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A7Fake = new PieceTypeDef{Id = 500010, Abbreviations = new List<string>{ "EXT_A7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A7     = new PieceTypeDef{Id = 500011, Abbreviations = new List<string>{ "EXT_A7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A8Fake = new PieceTypeDef{Id = 500012, Abbreviations = new List<string>{ "EXT_A8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A8     = new PieceTypeDef{Id = 500013, Abbreviations = new List<string>{ "EXT_A8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A9Fake = new PieceTypeDef{Id = 500014, Abbreviations = new List<string>{ "EXT_A9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_A9     = new PieceTypeDef{Id = 500015, Abbreviations = new List<string>{ "EXT_A9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion

    #region EXT_B
    
    public static readonly PieceTypeDef EXT_B1     = new PieceTypeDef{Id = 500100, Abbreviations = new List<string>{ "EXT_B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B2     = new PieceTypeDef{Id = 500101, Abbreviations = new List<string>{ "EXT_B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B3Fake = new PieceTypeDef{Id = 500102, Abbreviations = new List<string>{ "EXT_B3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B3     = new PieceTypeDef{Id = 500103, Abbreviations = new List<string>{ "EXT_B3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B4Fake = new PieceTypeDef{Id = 500104, Abbreviations = new List<string>{ "EXT_B4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B4     = new PieceTypeDef{Id = 500105, Abbreviations = new List<string>{ "EXT_B4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B5Fake = new PieceTypeDef{Id = 500106, Abbreviations = new List<string>{ "EXT_B5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B5     = new PieceTypeDef{Id = 500107, Abbreviations = new List<string>{ "EXT_B5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B6Fake = new PieceTypeDef{Id = 500108, Abbreviations = new List<string>{ "EXT_B6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B6     = new PieceTypeDef{Id = 500109, Abbreviations = new List<string>{ "EXT_B6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B7Fake = new PieceTypeDef{Id = 500110, Abbreviations = new List<string>{ "EXT_B7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B7     = new PieceTypeDef{Id = 500111, Abbreviations = new List<string>{ "EXT_B7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B8Fake = new PieceTypeDef{Id = 500112, Abbreviations = new List<string>{ "EXT_B8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B8     = new PieceTypeDef{Id = 500113, Abbreviations = new List<string>{ "EXT_B8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B9Fake = new PieceTypeDef{Id = 500114, Abbreviations = new List<string>{ "EXT_B9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_B9     = new PieceTypeDef{Id = 500115, Abbreviations = new List<string>{ "EXT_B9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion

    #region EXT_C
    
    public static readonly PieceTypeDef EXT_C1     = new PieceTypeDef{Id = 500200, Abbreviations = new List<string>{ "EXT_C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C2     = new PieceTypeDef{Id = 500201, Abbreviations = new List<string>{ "EXT_C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C3Fake = new PieceTypeDef{Id = 500202, Abbreviations = new List<string>{ "EXT_C3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C3     = new PieceTypeDef{Id = 500203, Abbreviations = new List<string>{ "EXT_C3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C4Fake = new PieceTypeDef{Id = 500204, Abbreviations = new List<string>{ "EXT_C4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C4     = new PieceTypeDef{Id = 500205, Abbreviations = new List<string>{ "EXT_C4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C5Fake = new PieceTypeDef{Id = 500206, Abbreviations = new List<string>{ "EXT_C5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C5     = new PieceTypeDef{Id = 500207, Abbreviations = new List<string>{ "EXT_C5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C6Fake = new PieceTypeDef{Id = 500208, Abbreviations = new List<string>{ "EXT_C6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C6     = new PieceTypeDef{Id = 500209, Abbreviations = new List<string>{ "EXT_C6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C7Fake = new PieceTypeDef{Id = 500210, Abbreviations = new List<string>{ "EXT_C7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C7     = new PieceTypeDef{Id = 500211, Abbreviations = new List<string>{ "EXT_C7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C8Fake = new PieceTypeDef{Id = 500212, Abbreviations = new List<string>{ "EXT_C8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C8     = new PieceTypeDef{Id = 500213, Abbreviations = new List<string>{ "EXT_C8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C9Fake = new PieceTypeDef{Id = 500214, Abbreviations = new List<string>{ "EXT_C9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_C9     = new PieceTypeDef{Id = 500215, Abbreviations = new List<string>{ "EXT_C9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion

    #region EXT_D
    
    public static readonly PieceTypeDef EXT_D1     = new PieceTypeDef{Id = 500300, Abbreviations = new List<string>{ "EXT_D1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D2     = new PieceTypeDef{Id = 500301, Abbreviations = new List<string>{ "EXT_D2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D3Fake = new PieceTypeDef{Id = 500302, Abbreviations = new List<string>{ "EXT_D3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D3     = new PieceTypeDef{Id = 500303, Abbreviations = new List<string>{ "EXT_D3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D4Fake = new PieceTypeDef{Id = 500304, Abbreviations = new List<string>{ "EXT_D4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D4     = new PieceTypeDef{Id = 500305, Abbreviations = new List<string>{ "EXT_D4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D5Fake = new PieceTypeDef{Id = 500306, Abbreviations = new List<string>{ "EXT_D5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D5     = new PieceTypeDef{Id = 500307, Abbreviations = new List<string>{ "EXT_D5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D6Fake = new PieceTypeDef{Id = 500308, Abbreviations = new List<string>{ "EXT_D6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D6     = new PieceTypeDef{Id = 500309, Abbreviations = new List<string>{ "EXT_D6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D7Fake = new PieceTypeDef{Id = 500310, Abbreviations = new List<string>{ "EXT_D7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D7     = new PieceTypeDef{Id = 500311, Abbreviations = new List<string>{ "EXT_D7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D8Fake = new PieceTypeDef{Id = 500312, Abbreviations = new List<string>{ "EXT_D8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D8     = new PieceTypeDef{Id = 500313, Abbreviations = new List<string>{ "EXT_D8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D9Fake = new PieceTypeDef{Id = 500314, Abbreviations = new List<string>{ "EXT_D9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_D9     = new PieceTypeDef{Id = 500315, Abbreviations = new List<string>{ "EXT_D9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion

    #region EXT_E
    
    public static readonly PieceTypeDef EXT_E1     = new PieceTypeDef{Id = 500400, Abbreviations = new List<string>{ "EXT_E1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E2     = new PieceTypeDef{Id = 500401, Abbreviations = new List<string>{ "EXT_E2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E3Fake = new PieceTypeDef{Id = 500402, Abbreviations = new List<string>{ "EXT_E3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E3     = new PieceTypeDef{Id = 500403, Abbreviations = new List<string>{ "EXT_E3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E4Fake = new PieceTypeDef{Id = 500404, Abbreviations = new List<string>{ "EXT_E4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E4     = new PieceTypeDef{Id = 500405, Abbreviations = new List<string>{ "EXT_E4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E5Fake = new PieceTypeDef{Id = 500406, Abbreviations = new List<string>{ "EXT_E5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E5     = new PieceTypeDef{Id = 500407, Abbreviations = new List<string>{ "EXT_E5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E6Fake = new PieceTypeDef{Id = 500408, Abbreviations = new List<string>{ "EXT_E6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E6     = new PieceTypeDef{Id = 500409, Abbreviations = new List<string>{ "EXT_E6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E7Fake = new PieceTypeDef{Id = 500410, Abbreviations = new List<string>{ "EXT_E7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E7     = new PieceTypeDef{Id = 500411, Abbreviations = new List<string>{ "EXT_E7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E8Fake = new PieceTypeDef{Id = 500412, Abbreviations = new List<string>{ "EXT_E8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E8     = new PieceTypeDef{Id = 500413, Abbreviations = new List<string>{ "EXT_E8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E9Fake = new PieceTypeDef{Id = 500414, Abbreviations = new List<string>{ "EXT_E9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef EXT_E9     = new PieceTypeDef{Id = 500415, Abbreviations = new List<string>{ "EXT_E9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
#endregion
}