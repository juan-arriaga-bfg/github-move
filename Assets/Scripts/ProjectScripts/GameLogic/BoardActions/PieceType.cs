﻿using System;
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
            
            // ignore empty piece
            if (def.Id == PieceType.Empty.Id) continue;
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    /// <summary>
    /// Return List of ids for pieces that has specified *filter* and not has ANY of *exclude*
    /// </summary>
    /// <param name="filter">Select pieces that has specified flag. Multiply flags are not supported, provide only one:
    /// GetIdsByFilter(PieceTypeFilter.Obstacle | PieceTypeFilter.Bag, [exclude]) - incorrect
    /// GetIdsByFilter(PieceTypeFilter.Obstacle, [exclude]) - Correct
    /// </param>
    /// <param name="exclude">Pieces that has ANY of flag from this param will be excluded</param>
    /// Let Piece X1 has PieceTypeFilters: Simple, Obstacle, Tree 
    /// Let Piece X2 has PieceTypeFilters: Simple, Tree 
    /// Let Piece X3 has PieceTypeFilters: Simple 
    /// GetIdsByFilter(Simple, Obstacle | Tree) => X3
    /// GetIdsByFilter(Simple, Obstacle) => X2, X3
    /// GetIdsByFilter(Simple, Tree) => X3
    public static List<int> GetIdsByFilter(PieceTypeFilter filter, PieceTypeFilter exclude)
    {
#if DEBUG
        int count = BitsHelper.CountOfSetBits((int) filter);
        if (count > 1)
        {
            Debug.LogError($"[PieceType] => GetIdsByFilter: filter param contains more then one flag set ({filter.PrettyPrint()}. It will not work correctly!)");
        }
#endif
        
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            // ignore empty piece
            if (def.Id == PieceType.Empty.Id) continue;
            
            if(def.Filter.Has(filter) == false) continue;
            if ((int)(def.Filter & exclude) != 0) continue;// Exclude pieces with ANY of flags from 'exclude' param
            
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
    
    public static readonly PieceTypeDef NPC_A = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "NPC_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_SleepingBeautyPlaid = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "NPC_SleepingBeautyPlaid" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "NPC_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_C = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "NPC_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};   
    public static readonly PieceTypeDef NPC_Gnome = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "NPC_Gnome" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_E = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "NPC_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_F = new PieceTypeDef{Id = 106, Abbreviations = new List<string>{ "NPC_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_D = new PieceTypeDef{Id = 107, Abbreviations = new List<string>{ "NPC_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_8 = new PieceTypeDef{Id = 108, Abbreviations = new List<string>{ "NPC_8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_9 = new PieceTypeDef{Id = 109, Abbreviations = new List<string>{ "NPC_9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_10 = new PieceTypeDef{Id = 110, Abbreviations = new List<string>{ "NPC_10" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_11 = new PieceTypeDef{Id = 111, Abbreviations = new List<string>{ "NPC_11" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_12 = new PieceTypeDef{Id = 112, Abbreviations = new List<string>{ "NPC_12" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_13 = new PieceTypeDef{Id = 113, Abbreviations = new List<string>{ "NPC_13" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_14 = new PieceTypeDef{Id = 114, Abbreviations = new List<string>{ "NPC_14" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_15 = new PieceTypeDef{Id = 115, Abbreviations = new List<string>{ "NPC_15" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_16 = new PieceTypeDef{Id = 116, Abbreviations = new List<string>{ "NPC_16" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_17 = new PieceTypeDef{Id = 117, Abbreviations = new List<string>{ "NPC_17" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_18 = new PieceTypeDef{Id = 118, Abbreviations = new List<string>{ "NPC_18" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_19 = new PieceTypeDef{Id = 119, Abbreviations = new List<string>{ "NPC_19" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Progress};
    
#endregion
    
#region Character pieces

    public static readonly PieceTypeDef NPC_B1 = new PieceTypeDef{Id = 300000, Abbreviations = new List<string>{ "NPC_B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B2 = new PieceTypeDef{Id = 300001, Abbreviations = new List<string>{ "NPC_B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B3 = new PieceTypeDef{Id = 300002, Abbreviations = new List<string>{ "NPC_B3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B4 = new PieceTypeDef{Id = 300003, Abbreviations = new List<string>{ "NPC_B4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B5 = new PieceTypeDef{Id = 300004, Abbreviations = new List<string>{ "NPC_B5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B6 = new PieceTypeDef{Id = 300005, Abbreviations = new List<string>{ "NPC_B6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B7 = new PieceTypeDef{Id = 300006, Abbreviations = new List<string>{ "NPC_B7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    public static readonly PieceTypeDef NPC_B8 = new PieceTypeDef{Id = 300007, Abbreviations = new List<string>{ "NPC_B8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Progress};
    
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

    
#endregion

#region Enemies
    
    public static readonly PieceTypeDef Enemy1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "Enemy1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Enemy};
    
#endregion
    
#region Boosters
    
    public static readonly PieceTypeDef Boost_CR1 = new PieceTypeDef{Id = 2001, Abbreviations = new List<string>{ "Boost_CR1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_CR2 = new PieceTypeDef{Id = 2002, Abbreviations = new List<string>{ "Boost_CR2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_CR3 = new PieceTypeDef{Id = 2003, Abbreviations = new List<string>{ "Boost_CR3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_CR  = new PieceTypeDef{Id = 2004, Abbreviations = new List<string>{ "Boost_CR"  }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Booster};
    
    public static readonly PieceTypeDef Boost_WR = new PieceTypeDef{Id = 2100, Abbreviations = new List<string>{ "Boost_WR" }, Filter = PieceTypeFilter.Simple};
    
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
    
    public static readonly PieceTypeDef Hard1 = new PieceTypeDef{Id = 3200, Abbreviations = new List<string>{ "Hard1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Hard2 = new PieceTypeDef{Id = 3201, Abbreviations = new List<string>{ "Hard2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Hard3 = new PieceTypeDef{Id = 3202, Abbreviations = new List<string>{ "Hard3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource}; 
    public static readonly PieceTypeDef Hard4 = new PieceTypeDef{Id = 3203, Abbreviations = new List<string>{ "Hard4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Hard5 = new PieceTypeDef{Id = 3204, Abbreviations = new List<string>{ "Hard5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Hard6 = new PieceTypeDef{Id = 3205, Abbreviations = new List<string>{ "Hard6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
#endregion
    
#region Mines
    
    public static readonly PieceTypeDef MN_B = new PieceTypeDef{Id = 5001, Abbreviations = new List<string>{ "MN_B" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_C = new PieceTypeDef{Id = 5002, Abbreviations = new List<string>{ "MN_C" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_D = new PieceTypeDef{Id = 5003, Abbreviations = new List<string>{ "MN_D" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_F = new PieceTypeDef{Id = 5004, Abbreviations = new List<string>{ "MN_F" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Workplace};
    
#endregion
    
#region Chests
    
    public static readonly PieceTypeDef CH_Free = new PieceTypeDef{Id = 10001, Abbreviations = new List<string>{ "CH_Free" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
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
    
#endregion
    
#region Obstacles
    
    public static readonly PieceTypeDef Fog = new PieceTypeDef{Id = 20000, Abbreviations = new List<string>{ "Fog" }, Filter = PieceTypeFilter.Multicellular};
    
    public static readonly PieceTypeDef OB1_TT = new PieceTypeDef{Id = 20001, Abbreviations = new List<string>{ "OB1_TT" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_TT = new PieceTypeDef{Id = 20002, Abbreviations = new List<string>{ "OB2_TT" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    
    public static readonly PieceTypeDef OB1_A = new PieceTypeDef{Id = 20100, Abbreviations = new List<string>{ "OB1_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_A = new PieceTypeDef{Id = 20101, Abbreviations = new List<string>{ "OB2_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB3_A = new PieceTypeDef{Id = 20102, Abbreviations = new List<string>{ "OB3_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB4_A = new PieceTypeDef{Id = 20103, Abbreviations = new List<string>{ "OB4_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB5_A = new PieceTypeDef{Id = 20104, Abbreviations = new List<string>{ "OB5_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB6_A = new PieceTypeDef{Id = 20105, Abbreviations = new List<string>{ "OB6_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB7_A = new PieceTypeDef{Id = 20106, Abbreviations = new List<string>{ "OB7_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB8_A = new PieceTypeDef{Id = 20107, Abbreviations = new List<string>{ "OB8_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB9_A = new PieceTypeDef{Id = 20108, Abbreviations = new List<string>{ "OB9_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    
    public static readonly PieceTypeDef OB_PR_A = new PieceTypeDef{Id = 20200, Abbreviations = new List<string>{ "OB_PR_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_B = new PieceTypeDef{Id = 20201, Abbreviations = new List<string>{ "OB_PR_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_C = new PieceTypeDef{Id = 20202, Abbreviations = new List<string>{ "OB_PR_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_D = new PieceTypeDef{Id = 20203, Abbreviations = new List<string>{ "OB_PR_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_E = new PieceTypeDef{Id = 20204, Abbreviations = new List<string>{ "OB_PR_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_F = new PieceTypeDef{Id = 20205, Abbreviations = new List<string>{ "OB_PR_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_G = new PieceTypeDef{Id = 20206, Abbreviations = new List<string>{ "OB_PR_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.ProductionField};

    public static readonly PieceTypeDef OB1_E = new PieceTypeDef{Id = 20300, Abbreviations = new List<string>{ "OB1_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_E = new PieceTypeDef{Id = 20301, Abbreviations = new List<string>{ "OB2_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB3_E = new PieceTypeDef{Id = 20302, Abbreviations = new List<string>{ "OB3_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB4_E = new PieceTypeDef{Id = 20303, Abbreviations = new List<string>{ "OB4_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB5_E = new PieceTypeDef{Id = 20304, Abbreviations = new List<string>{ "OB5_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Workplace | PieceTypeFilter.Tree};
    
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
    public static readonly PieceTypeDef A9     = new PieceTypeDef{Id = 100015, Abbreviations = new List<string>{ "A9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef B11     = new PieceTypeDef{Id = 100119, Abbreviations = new List<string>{ "B11" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef C9     = new PieceTypeDef{Id = 100215, Abbreviations = new List<string>{ "C9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef D9     = new PieceTypeDef{Id = 100315, Abbreviations = new List<string>{ "D9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef E9     = new PieceTypeDef{Id = 100415, Abbreviations = new List<string>{ "E9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef F9     = new PieceTypeDef{Id = 100515, Abbreviations = new List<string>{ "F9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef G9     = new PieceTypeDef{Id = 100615, Abbreviations = new List<string>{ "G9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef H9     = new PieceTypeDef{Id = 100715, Abbreviations = new List<string>{ "H9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef I9     = new PieceTypeDef{Id = 100815, Abbreviations = new List<string>{ "I9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
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
    public static readonly PieceTypeDef J9     = new PieceTypeDef{Id = 100915, Abbreviations = new List<string>{ "J9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Progress};
    
    #endregion
    
#endregion
    
#region Reproduction Pieces
    
    #region PR_A
    
    public static readonly PieceTypeDef PR_A1 = new PieceTypeDef{Id = 200000, Abbreviations = new List<string>{ "PR_A1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A2 = new PieceTypeDef{Id = 200001, Abbreviations = new List<string>{ "PR_A2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A3 = new PieceTypeDef{Id = 200002, Abbreviations = new List<string>{ "PR_A3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A4 = new PieceTypeDef{Id = 200003, Abbreviations = new List<string>{ "PR_A4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_A5 = new PieceTypeDef{Id = 200004, Abbreviations = new List<string>{ "PR_A5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_B
    
    public static readonly PieceTypeDef PR_B1 = new PieceTypeDef{Id = 200100, Abbreviations = new List<string>{ "PR_B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B2 = new PieceTypeDef{Id = 200101, Abbreviations = new List<string>{ "PR_B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B3 = new PieceTypeDef{Id = 200102, Abbreviations = new List<string>{ "PR_B3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B4 = new PieceTypeDef{Id = 200103, Abbreviations = new List<string>{ "PR_B4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_B5 = new PieceTypeDef{Id = 200104, Abbreviations = new List<string>{ "PR_B5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_C
    
    public static readonly PieceTypeDef PR_C1 = new PieceTypeDef{Id = 200200, Abbreviations = new List<string>{ "PR_C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C2 = new PieceTypeDef{Id = 200201, Abbreviations = new List<string>{ "PR_C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C3 = new PieceTypeDef{Id = 200202, Abbreviations = new List<string>{ "PR_C3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C4 = new PieceTypeDef{Id = 200203, Abbreviations = new List<string>{ "PR_C4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_C5 = new PieceTypeDef{Id = 200204, Abbreviations = new List<string>{ "PR_C5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_D
    
    public static readonly PieceTypeDef PR_D1 = new PieceTypeDef{Id = 200300, Abbreviations = new List<string>{ "PR_D1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D2 = new PieceTypeDef{Id = 200301, Abbreviations = new List<string>{ "PR_D2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D3 = new PieceTypeDef{Id = 200302, Abbreviations = new List<string>{ "PR_D3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D4 = new PieceTypeDef{Id = 200303, Abbreviations = new List<string>{ "PR_D4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_D5 = new PieceTypeDef{Id = 200304, Abbreviations = new List<string>{ "PR_D5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_E
    
    public static readonly PieceTypeDef PR_E1 = new PieceTypeDef{Id = 200400, Abbreviations = new List<string>{ "PR_E1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E2 = new PieceTypeDef{Id = 200401, Abbreviations = new List<string>{ "PR_E2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E3 = new PieceTypeDef{Id = 200402, Abbreviations = new List<string>{ "PR_E3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E4 = new PieceTypeDef{Id = 200403, Abbreviations = new List<string>{ "PR_E4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_E5 = new PieceTypeDef{Id = 200404, Abbreviations = new List<string>{ "PR_E5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_F
    
    public static readonly PieceTypeDef PR_F1 = new PieceTypeDef{Id = 200500, Abbreviations = new List<string>{ "PR_F1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F2 = new PieceTypeDef{Id = 200501, Abbreviations = new List<string>{ "PR_F2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F3 = new PieceTypeDef{Id = 200502, Abbreviations = new List<string>{ "PR_F3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F4 = new PieceTypeDef{Id = 200503, Abbreviations = new List<string>{ "PR_F4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_F5 = new PieceTypeDef{Id = 200504, Abbreviations = new List<string>{ "PR_F5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_G
    
    public static readonly PieceTypeDef PR_G1 = new PieceTypeDef{Id = 200600, Abbreviations = new List<string>{ "PR_G1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G2 = new PieceTypeDef{Id = 200601, Abbreviations = new List<string>{ "PR_G2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G3 = new PieceTypeDef{Id = 200602, Abbreviations = new List<string>{ "PR_G3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G4 = new PieceTypeDef{Id = 200603, Abbreviations = new List<string>{ "PR_G4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_G5 = new PieceTypeDef{Id = 200604, Abbreviations = new List<string>{ "PR_G5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
#endregion
}