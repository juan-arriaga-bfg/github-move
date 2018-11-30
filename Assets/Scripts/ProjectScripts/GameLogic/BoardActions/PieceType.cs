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
    Workplace = 8192,
    Tree = 16384,
    ProductionField = 32768, // Грядка (bed)
    Removable = 65536,
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
    
#region Characters
    
    public static readonly PieceTypeDef NPC_SleepingBeauty = new PieceTypeDef{Id = 10, Abbreviations = new List<string>{ "NPC_SleepingBeauty" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_SleepingBeautyPlaid = new PieceTypeDef{Id = 11, Abbreviations = new List<string>{ "NPC_SleepingBeautyPlaid" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_Rapunzel = new PieceTypeDef{Id = 12, Abbreviations = new List<string>{ "NPC_Rapunzel" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_PussInBoots = new PieceTypeDef{Id = 13, Abbreviations = new List<string>{ "NPC_PussInBoots" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_Gnome = new PieceTypeDef{Id = 14, Abbreviations = new List<string>{ "NPC_Gnome" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_5 = new PieceTypeDef{Id = 15, Abbreviations = new List<string>{ "NPC_5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_6 = new PieceTypeDef{Id = 16, Abbreviations = new List<string>{ "NPC_6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_7 = new PieceTypeDef{Id = 17, Abbreviations = new List<string>{ "NPC_7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_8 = new PieceTypeDef{Id = 18, Abbreviations = new List<string>{ "NPC_8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_9 = new PieceTypeDef{Id = 19, Abbreviations = new List<string>{ "NPC_9" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    
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
    
#endregion
    
#region Chests
    
    public static readonly PieceTypeDef CH_Free = new PieceTypeDef{Id = 10001, Abbreviations = new List<string>{ "CH_Free" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
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
    
#endregion
    
#region Simple Pieces
    
    #region A
    
    public static readonly PieceTypeDef A1 = new PieceTypeDef{Id = 100000, Abbreviations = new List<string>{ "A1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef A2 = new PieceTypeDef{Id = 100001, Abbreviations = new List<string>{ "A2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef A3Fake = new PieceTypeDef{Id = 100002, Abbreviations = new List<string>{ "A3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef A3 = new PieceTypeDef{Id = 100003, Abbreviations = new List<string>{ "A3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A4Fake = new PieceTypeDef{Id = 100004, Abbreviations = new List<string>{ "A4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef A4 = new PieceTypeDef{Id = 100005, Abbreviations = new List<string>{ "A4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A5Fake = new PieceTypeDef{Id = 100006, Abbreviations = new List<string>{ "A5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef A5 = new PieceTypeDef{Id = 100007, Abbreviations = new List<string>{ "A5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A6Fake = new PieceTypeDef{Id = 100008, Abbreviations = new List<string>{ "A6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef A6 = new PieceTypeDef{Id = 100009, Abbreviations = new List<string>{ "A6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A7Fake = new PieceTypeDef{Id = 100010, Abbreviations = new List<string>{ "A7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef A7 = new PieceTypeDef{Id = 100011, Abbreviations = new List<string>{ "A7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef A8Fake = new PieceTypeDef{Id = 100012, Abbreviations = new List<string>{ "A8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef A8 = new PieceTypeDef{Id = 100013, Abbreviations = new List<string>{ "A8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef A9Fake = new PieceTypeDef{Id = 100014, Abbreviations = new List<string>{ "A9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef A9 = new PieceTypeDef{Id = 100015, Abbreviations = new List<string>{ "A9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace};
    
    #endregion
    
    #region B
    
    public static readonly PieceTypeDef B1 = new PieceTypeDef{Id = 100100, Abbreviations = new List<string>{ "B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef B2 = new PieceTypeDef{Id = 100101, Abbreviations = new List<string>{ "B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef B3Fake = new PieceTypeDef{Id = 100102, Abbreviations = new List<string>{ "B3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B3 = new PieceTypeDef{Id = 100103, Abbreviations = new List<string>{ "B3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B4Fake = new PieceTypeDef{Id = 100104, Abbreviations = new List<string>{ "B4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B4 = new PieceTypeDef{Id = 100105, Abbreviations = new List<string>{ "B4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B5Fake = new PieceTypeDef{Id = 100106, Abbreviations = new List<string>{ "B5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B5 = new PieceTypeDef{Id = 100107, Abbreviations = new List<string>{ "B5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B6Fake = new PieceTypeDef{Id = 100108, Abbreviations = new List<string>{ "B6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B6 = new PieceTypeDef{Id = 100109, Abbreviations = new List<string>{ "B6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B7Fake = new PieceTypeDef{Id = 100110, Abbreviations = new List<string>{ "B7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B7 = new PieceTypeDef{Id = 100111, Abbreviations = new List<string>{ "B7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B8Fake = new PieceTypeDef{Id = 100112, Abbreviations = new List<string>{ "B8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B8 = new PieceTypeDef{Id = 100113, Abbreviations = new List<string>{ "B8" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B9Fake = new PieceTypeDef{Id = 100114, Abbreviations = new List<string>{ "B9Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B9 = new PieceTypeDef{Id = 100115, Abbreviations = new List<string>{ "B9" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef B10Fake = new PieceTypeDef{Id = 100116, Abbreviations = new List<string>{ "B10Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B10 = new PieceTypeDef{Id = 100117, Abbreviations = new List<string>{ "B10" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B11Fake = new PieceTypeDef{Id = 100118, Abbreviations = new List<string>{ "B11Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef B11 = new PieceTypeDef{Id = 100119, Abbreviations = new List<string>{ "B11" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace};
    
    #endregion
    
    #region C
    
    public static readonly PieceTypeDef C1 = new PieceTypeDef{Id = 100200, Abbreviations = new List<string>{ "C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef C2 = new PieceTypeDef{Id = 100201, Abbreviations = new List<string>{ "C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef C3Fake = new PieceTypeDef{Id = 100202, Abbreviations = new List<string>{ "C3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef C3 = new PieceTypeDef{Id = 100203, Abbreviations = new List<string>{ "C3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C4Fake = new PieceTypeDef{Id = 100204, Abbreviations = new List<string>{ "C4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef C4 = new PieceTypeDef{Id = 100205, Abbreviations = new List<string>{ "C4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C5Fake = new PieceTypeDef{Id = 100206, Abbreviations = new List<string>{ "C5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef C5 = new PieceTypeDef{Id = 100207, Abbreviations = new List<string>{ "C5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C6Fake = new PieceTypeDef{Id = 100208, Abbreviations = new List<string>{ "C6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef C6 = new PieceTypeDef{Id = 100209, Abbreviations = new List<string>{ "C6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C7Fake = new PieceTypeDef{Id = 100210, Abbreviations = new List<string>{ "C7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef C7 = new PieceTypeDef{Id = 100211, Abbreviations = new List<string>{ "C7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef C8Fake = new PieceTypeDef{Id = 100212, Abbreviations = new List<string>{ "C8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef C8 = new PieceTypeDef{Id = 100213, Abbreviations = new List<string>{ "C8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef C9Fake = new PieceTypeDef{Id = 100214, Abbreviations = new List<string>{ "C9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef C9 = new PieceTypeDef{Id = 100215, Abbreviations = new List<string>{ "C9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace};
    
    #endregion
    
    #region D
    
    public static readonly PieceTypeDef D1 = new PieceTypeDef{Id = 100300, Abbreviations = new List<string>{ "D1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef D2 = new PieceTypeDef{Id = 100301, Abbreviations = new List<string>{ "D2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef D3Fake = new PieceTypeDef{Id = 100302, Abbreviations = new List<string>{ "D3Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef D3 = new PieceTypeDef{Id = 100303, Abbreviations = new List<string>{ "D3" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D4Fake = new PieceTypeDef{Id = 100304, Abbreviations = new List<string>{ "D4Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef D4 = new PieceTypeDef{Id = 100305, Abbreviations = new List<string>{ "D4" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D5Fake = new PieceTypeDef{Id = 100306, Abbreviations = new List<string>{ "D5Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef D5 = new PieceTypeDef{Id = 100307, Abbreviations = new List<string>{ "D5" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D6Fake = new PieceTypeDef{Id = 100308, Abbreviations = new List<string>{ "D6Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef D6 = new PieceTypeDef{Id = 100309, Abbreviations = new List<string>{ "D6" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D7Fake = new PieceTypeDef{Id = 100310, Abbreviations = new List<string>{ "D7Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef D7 = new PieceTypeDef{Id = 100311, Abbreviations = new List<string>{ "D7" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef D8Fake = new PieceTypeDef{Id = 100312, Abbreviations = new List<string>{ "D8Fake" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef D8 = new PieceTypeDef{Id = 100313, Abbreviations = new List<string>{ "D8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef D9Fake = new PieceTypeDef{Id = 100314, Abbreviations = new List<string>{ "D9Fake" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef D9 = new PieceTypeDef{Id = 100315, Abbreviations = new List<string>{ "D9" }, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Workplace};
    
    #endregion
    
#endregion
    
#region Reproduction Pieces
    
    #region PR_A
    
    public static readonly PieceTypeDef PR_A1 = new PieceTypeDef{Id = 200000, Abbreviations = new List<string>{ "PR_A1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A2 = new PieceTypeDef{Id = 200001, Abbreviations = new List<string>{ "PR_A2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A3 = new PieceTypeDef{Id = 200002, Abbreviations = new List<string>{ "PR_A3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A4 = new PieceTypeDef{Id = 200003, Abbreviations = new List<string>{ "PR_A4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A5 = new PieceTypeDef{Id = 200004, Abbreviations = new List<string>{ "PR_A5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    #endregion
    
    #region PR_B
    
    public static readonly PieceTypeDef PR_B1 = new PieceTypeDef{Id = 200100, Abbreviations = new List<string>{ "PR_B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B2 = new PieceTypeDef{Id = 200101, Abbreviations = new List<string>{ "PR_B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B3 = new PieceTypeDef{Id = 200102, Abbreviations = new List<string>{ "PR_B3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B4 = new PieceTypeDef{Id = 200103, Abbreviations = new List<string>{ "PR_B4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B5 = new PieceTypeDef{Id = 200104, Abbreviations = new List<string>{ "PR_B5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    #endregion
    
    #region PR_C
    
    public static readonly PieceTypeDef PR_C1 = new PieceTypeDef{Id = 200200, Abbreviations = new List<string>{ "PR_C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C2 = new PieceTypeDef{Id = 200201, Abbreviations = new List<string>{ "PR_C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C3 = new PieceTypeDef{Id = 200202, Abbreviations = new List<string>{ "PR_C3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C4 = new PieceTypeDef{Id = 200203, Abbreviations = new List<string>{ "PR_C4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C5 = new PieceTypeDef{Id = 200204, Abbreviations = new List<string>{ "PR_C5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    #endregion
    
    #region PR_D
    
    public static readonly PieceTypeDef PR_D1 = new PieceTypeDef{Id = 200300, Abbreviations = new List<string>{ "PR_D1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D2 = new PieceTypeDef{Id = 200301, Abbreviations = new List<string>{ "PR_D2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D3 = new PieceTypeDef{Id = 200302, Abbreviations = new List<string>{ "PR_D3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D4 = new PieceTypeDef{Id = 200303, Abbreviations = new List<string>{ "PR_D4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D5 = new PieceTypeDef{Id = 200304, Abbreviations = new List<string>{ "PR_D5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    #endregion
    
    #region PR_E
    
    public static readonly PieceTypeDef PR_E1 = new PieceTypeDef{Id = 200400, Abbreviations = new List<string>{ "PR_E1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E2 = new PieceTypeDef{Id = 200401, Abbreviations = new List<string>{ "PR_E2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E3 = new PieceTypeDef{Id = 200402, Abbreviations = new List<string>{ "PR_E3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E4 = new PieceTypeDef{Id = 200403, Abbreviations = new List<string>{ "PR_E4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E5 = new PieceTypeDef{Id = 200404, Abbreviations = new List<string>{ "PR_E5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
    #endregion
    
#endregion
}