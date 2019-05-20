using Debug = IW.Logger;
using System.Collections.Generic;
using System.Reflection;

public partial class PieceTypeDef
{
    public PieceTypeFilter Filter { get; set; }
    public PieceTypeBranch Branch { get; set; }
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
    
#region Simple Pieces
    
    #region A
    
    public static readonly PieceTypeDef A1     = new PieceTypeDef{Id = 1000101, Abbreviations = new List<string>{ "A1" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A2     = new PieceTypeDef{Id = 1000102, Abbreviations = new List<string>{ "A2" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A3Fake = new PieceTypeDef{Id = 1000103, Abbreviations = new List<string>{ "A3Fake" }, Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A3     = new PieceTypeDef{Id = 1000104, Abbreviations = new List<string>{ "A3" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A4Fake = new PieceTypeDef{Id = 1000105, Abbreviations = new List<string>{ "A4Fake" }, Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A4     = new PieceTypeDef{Id = 1000106, Abbreviations = new List<string>{ "A4" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A5Fake = new PieceTypeDef{Id = 1000107, Abbreviations = new List<string>{ "A5Fake" }, Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A5     = new PieceTypeDef{Id = 1000108, Abbreviations = new List<string>{ "A5" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A6Fake = new PieceTypeDef{Id = 1000109, Abbreviations = new List<string>{ "A6Fake" }, Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A6     = new PieceTypeDef{Id = 1000110, Abbreviations = new List<string>{ "A6" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A7Fake = new PieceTypeDef{Id = 1000111, Abbreviations = new List<string>{ "A7Fake" }, Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A7     = new PieceTypeDef{Id = 1000112, Abbreviations = new List<string>{ "A7" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A8Fake = new PieceTypeDef{Id = 1000113, Abbreviations = new List<string>{ "A8Fake" }, Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef A8     = new PieceTypeDef{Id = 1000114, Abbreviations = new List<string>{ "A8" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef AMFake = new PieceTypeDef{Id = 1000190, Abbreviations = new List<string>{ "AMFake" }, Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef AM     = new PieceTypeDef{Id = 1000191, Abbreviations = new List<string>{ "AM" },     Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
    #region B
    
    public static readonly PieceTypeDef B1      = new PieceTypeDef{Id = 1000201, Abbreviations = new List<string>{ "B1" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B2      = new PieceTypeDef{Id = 1000202, Abbreviations = new List<string>{ "B2" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B3Fake  = new PieceTypeDef{Id = 1000203, Abbreviations = new List<string>{ "B3Fake" },  Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B3      = new PieceTypeDef{Id = 1000204, Abbreviations = new List<string>{ "B3" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B4Fake  = new PieceTypeDef{Id = 1000205, Abbreviations = new List<string>{ "B4Fake" },  Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B4      = new PieceTypeDef{Id = 1000206, Abbreviations = new List<string>{ "B4" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B5Fake  = new PieceTypeDef{Id = 1000207, Abbreviations = new List<string>{ "B5Fake" },  Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B5      = new PieceTypeDef{Id = 1000208, Abbreviations = new List<string>{ "B5" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B6Fake  = new PieceTypeDef{Id = 1000209, Abbreviations = new List<string>{ "B6Fake" },  Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B6      = new PieceTypeDef{Id = 1000210, Abbreviations = new List<string>{ "B6" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B7Fake  = new PieceTypeDef{Id = 1000211, Abbreviations = new List<string>{ "B7Fake" },  Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B7      = new PieceTypeDef{Id = 1000212, Abbreviations = new List<string>{ "B7" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B8Fake  = new PieceTypeDef{Id = 1000213, Abbreviations = new List<string>{ "B8Fake" },  Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B8      = new PieceTypeDef{Id = 1000214, Abbreviations = new List<string>{ "B8" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B9Fake  = new PieceTypeDef{Id = 1000215, Abbreviations = new List<string>{ "B9Fake" },  Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef B9      = new PieceTypeDef{Id = 1000216, Abbreviations = new List<string>{ "B9" },      Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef BMFake = new PieceTypeDef{Id = 1000290, Abbreviations = new List<string>{ "BMFake" },   Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef BM     = new PieceTypeDef{Id = 1000291, Abbreviations = new List<string>{ "BM" },       Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
    #region C
    
    public static readonly PieceTypeDef C1      = new PieceTypeDef{Id = 1000301, Abbreviations = new List<string>{ "C1" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C2      = new PieceTypeDef{Id = 1000302, Abbreviations = new List<string>{ "C2" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C3Fake  = new PieceTypeDef{Id = 1000303, Abbreviations = new List<string>{ "C3Fake" },  Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C3      = new PieceTypeDef{Id = 1000304, Abbreviations = new List<string>{ "C3" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C4Fake  = new PieceTypeDef{Id = 1000305, Abbreviations = new List<string>{ "C4Fake" },  Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C4      = new PieceTypeDef{Id = 1000306, Abbreviations = new List<string>{ "C4" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C5Fake  = new PieceTypeDef{Id = 1000307, Abbreviations = new List<string>{ "C5Fake" },  Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C5      = new PieceTypeDef{Id = 1000308, Abbreviations = new List<string>{ "C5" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C6Fake  = new PieceTypeDef{Id = 1000309, Abbreviations = new List<string>{ "C6Fake" },  Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C6      = new PieceTypeDef{Id = 1000310, Abbreviations = new List<string>{ "C6" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C7Fake  = new PieceTypeDef{Id = 1000311, Abbreviations = new List<string>{ "C7Fake" },  Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C7      = new PieceTypeDef{Id = 1000312, Abbreviations = new List<string>{ "C7" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C8Fake  = new PieceTypeDef{Id = 1000313, Abbreviations = new List<string>{ "C8Fake" },  Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C8      = new PieceTypeDef{Id = 1000314, Abbreviations = new List<string>{ "C8" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C9Fake  = new PieceTypeDef{Id = 1000315, Abbreviations = new List<string>{ "C9Fake" },  Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef C9      = new PieceTypeDef{Id = 1000316, Abbreviations = new List<string>{ "C9" },      Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef CMFake = new PieceTypeDef{Id = 1000390, Abbreviations = new List<string>{ "CMFake" },   Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef CM     = new PieceTypeDef{Id = 1000391, Abbreviations = new List<string>{ "CM" },       Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
    #region D
    
    public static readonly PieceTypeDef D1     = new PieceTypeDef{Id = 1000401, Abbreviations = new List<string>{ "D1" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D2     = new PieceTypeDef{Id = 1000402, Abbreviations = new List<string>{ "D2" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D3Fake = new PieceTypeDef{Id = 1000403, Abbreviations = new List<string>{ "D3Fake" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D3     = new PieceTypeDef{Id = 1000404, Abbreviations = new List<string>{ "D3" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D4Fake = new PieceTypeDef{Id = 1000405, Abbreviations = new List<string>{ "D4Fake" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D4     = new PieceTypeDef{Id = 1000406, Abbreviations = new List<string>{ "D4" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D5Fake = new PieceTypeDef{Id = 1000407, Abbreviations = new List<string>{ "D5Fake" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D5     = new PieceTypeDef{Id = 1000408, Abbreviations = new List<string>{ "D5" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D6Fake = new PieceTypeDef{Id = 1000409, Abbreviations = new List<string>{ "D6Fake" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D6     = new PieceTypeDef{Id = 1000410, Abbreviations = new List<string>{ "D6" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D7Fake = new PieceTypeDef{Id = 1000411, Abbreviations = new List<string>{ "D7Fake" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D7     = new PieceTypeDef{Id = 1000412, Abbreviations = new List<string>{ "D7" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D8Fake = new PieceTypeDef{Id = 1000413, Abbreviations = new List<string>{ "D8Fake" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D8     = new PieceTypeDef{Id = 1000414, Abbreviations = new List<string>{ "D8" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D9Fake = new PieceTypeDef{Id = 1000415, Abbreviations = new List<string>{ "D9Fake" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef D9     = new PieceTypeDef{Id = 1000416, Abbreviations = new List<string>{ "D9" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef DMFake = new PieceTypeDef{Id = 1000490, Abbreviations = new List<string>{ "DMFake" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef DM     = new PieceTypeDef{Id = 1000491, Abbreviations = new List<string>{ "DM" },     Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
    #region E
    
    public static readonly PieceTypeDef E1      = new PieceTypeDef{Id = 1000501, Abbreviations = new List<string>{ "E1" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E2      = new PieceTypeDef{Id = 1000502, Abbreviations = new List<string>{ "E2" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E3Fake  = new PieceTypeDef{Id = 1000503, Abbreviations = new List<string>{ "E3Fake" },  Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E3      = new PieceTypeDef{Id = 1000504, Abbreviations = new List<string>{ "E3" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E4Fake  = new PieceTypeDef{Id = 1000505, Abbreviations = new List<string>{ "E4Fake" },  Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E4      = new PieceTypeDef{Id = 1000506, Abbreviations = new List<string>{ "E4" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E5Fake  = new PieceTypeDef{Id = 1000507, Abbreviations = new List<string>{ "E5Fake" },  Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E5      = new PieceTypeDef{Id = 1000508, Abbreviations = new List<string>{ "E5" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E6Fake  = new PieceTypeDef{Id = 1000509, Abbreviations = new List<string>{ "E6Fake" },  Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E6      = new PieceTypeDef{Id = 1000510, Abbreviations = new List<string>{ "E6" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E7Fake  = new PieceTypeDef{Id = 1000511, Abbreviations = new List<string>{ "E7Fake" },  Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E7      = new PieceTypeDef{Id = 1000512, Abbreviations = new List<string>{ "E7" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E8Fake  = new PieceTypeDef{Id = 1000513, Abbreviations = new List<string>{ "E8Fake" },  Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E8      = new PieceTypeDef{Id = 1000514, Abbreviations = new List<string>{ "E8" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E9Fake  = new PieceTypeDef{Id = 1000515, Abbreviations = new List<string>{ "E9Fake" },  Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef E9      = new PieceTypeDef{Id = 1000516, Abbreviations = new List<string>{ "E9" },      Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef EMFake = new PieceTypeDef{Id = 1000590, Abbreviations = new List<string>{ "EMFake" },   Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef EM     = new PieceTypeDef{Id = 1000591, Abbreviations = new List<string>{ "EM" },       Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
  
    #region F
    
    public static readonly PieceTypeDef F1     = new PieceTypeDef{Id = 1000601, Abbreviations = new List<string>{ "F1" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F2     = new PieceTypeDef{Id = 1000602, Abbreviations = new List<string>{ "F2" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F3Fake = new PieceTypeDef{Id = 1000603, Abbreviations = new List<string>{ "F3Fake" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F3     = new PieceTypeDef{Id = 1000604, Abbreviations = new List<string>{ "F3" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F4Fake = new PieceTypeDef{Id = 1000605, Abbreviations = new List<string>{ "F4Fake" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F4     = new PieceTypeDef{Id = 1000606, Abbreviations = new List<string>{ "F4" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F5Fake = new PieceTypeDef{Id = 1000607, Abbreviations = new List<string>{ "F5Fake" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F5     = new PieceTypeDef{Id = 1000608, Abbreviations = new List<string>{ "F5" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F6Fake = new PieceTypeDef{Id = 1000609, Abbreviations = new List<string>{ "F6Fake" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F6     = new PieceTypeDef{Id = 1000610, Abbreviations = new List<string>{ "F6" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F7Fake = new PieceTypeDef{Id = 1000611, Abbreviations = new List<string>{ "F7Fake" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F7     = new PieceTypeDef{Id = 1000612, Abbreviations = new List<string>{ "F7" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F8Fake = new PieceTypeDef{Id = 1000613, Abbreviations = new List<string>{ "F8Fake" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F8     = new PieceTypeDef{Id = 1000614, Abbreviations = new List<string>{ "F8" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F9Fake = new PieceTypeDef{Id = 1000615, Abbreviations = new List<string>{ "F9Fake" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef F9     = new PieceTypeDef{Id = 1000616, Abbreviations = new List<string>{ "F9" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef FMFake = new PieceTypeDef{Id = 1000690, Abbreviations = new List<string>{ "FMFake" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef FM     = new PieceTypeDef{Id = 1000691, Abbreviations = new List<string>{ "FM" },     Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
    #region G
    
    public static readonly PieceTypeDef G1     = new PieceTypeDef{Id = 1000701, Abbreviations = new List<string>{ "G1" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G2     = new PieceTypeDef{Id = 1000702, Abbreviations = new List<string>{ "G2" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G3Fake = new PieceTypeDef{Id = 1000703, Abbreviations = new List<string>{ "G3Fake" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G3     = new PieceTypeDef{Id = 1000704, Abbreviations = new List<string>{ "G3" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G4Fake = new PieceTypeDef{Id = 1000705, Abbreviations = new List<string>{ "G4Fake" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G4     = new PieceTypeDef{Id = 1000706, Abbreviations = new List<string>{ "G4" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G5Fake = new PieceTypeDef{Id = 1000707, Abbreviations = new List<string>{ "G5Fake" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G5     = new PieceTypeDef{Id = 1000708, Abbreviations = new List<string>{ "G5" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G6Fake = new PieceTypeDef{Id = 1000709, Abbreviations = new List<string>{ "G6Fake" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G6     = new PieceTypeDef{Id = 1000710, Abbreviations = new List<string>{ "G6" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G7Fake = new PieceTypeDef{Id = 1000711, Abbreviations = new List<string>{ "G7Fake" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G7     = new PieceTypeDef{Id = 1000712, Abbreviations = new List<string>{ "G7" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G8Fake = new PieceTypeDef{Id = 1000713, Abbreviations = new List<string>{ "G8Fake" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G8     = new PieceTypeDef{Id = 1000714, Abbreviations = new List<string>{ "G8" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G9Fake = new PieceTypeDef{Id = 1000715, Abbreviations = new List<string>{ "G9Fake" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef G9     = new PieceTypeDef{Id = 1000716, Abbreviations = new List<string>{ "G9" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef GMFake = new PieceTypeDef{Id = 1000790, Abbreviations = new List<string>{ "GMFake" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef GM     = new PieceTypeDef{Id = 1000791, Abbreviations = new List<string>{ "GM" },     Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
    #region H
    
    public static readonly PieceTypeDef H1     = new PieceTypeDef{Id = 1000801, Abbreviations = new List<string>{ "H1" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H2     = new PieceTypeDef{Id = 1000802, Abbreviations = new List<string>{ "H2" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H3Fake = new PieceTypeDef{Id = 1000803, Abbreviations = new List<string>{ "H3Fake" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H3     = new PieceTypeDef{Id = 1000804, Abbreviations = new List<string>{ "H3" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H4Fake = new PieceTypeDef{Id = 1000805, Abbreviations = new List<string>{ "H4Fake" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H4     = new PieceTypeDef{Id = 1000806, Abbreviations = new List<string>{ "H4" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H5Fake = new PieceTypeDef{Id = 1000807, Abbreviations = new List<string>{ "H5Fake" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H5     = new PieceTypeDef{Id = 1000808, Abbreviations = new List<string>{ "H5" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H6Fake = new PieceTypeDef{Id = 1000809, Abbreviations = new List<string>{ "H6Fake" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H6     = new PieceTypeDef{Id = 1000810, Abbreviations = new List<string>{ "H6" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H7Fake = new PieceTypeDef{Id = 1000811, Abbreviations = new List<string>{ "H7Fake" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H7     = new PieceTypeDef{Id = 1000812, Abbreviations = new List<string>{ "H7" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H8Fake = new PieceTypeDef{Id = 1000813, Abbreviations = new List<string>{ "H8Fake" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H8     = new PieceTypeDef{Id = 1000814, Abbreviations = new List<string>{ "H8" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H9Fake = new PieceTypeDef{Id = 1000815, Abbreviations = new List<string>{ "H9Fake" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef H9     = new PieceTypeDef{Id = 1000816, Abbreviations = new List<string>{ "H9" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef HMFake = new PieceTypeDef{Id = 1000890, Abbreviations = new List<string>{ "HMFake" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef HM     = new PieceTypeDef{Id = 1000891, Abbreviations = new List<string>{ "HM" },     Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
    #region I
    
    public static readonly PieceTypeDef I1     = new PieceTypeDef{Id = 1000901, Abbreviations = new List<string>{ "I1" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I2     = new PieceTypeDef{Id = 1000902, Abbreviations = new List<string>{ "I2" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I3Fake = new PieceTypeDef{Id = 1000903, Abbreviations = new List<string>{ "I3Fake" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I3     = new PieceTypeDef{Id = 1000904, Abbreviations = new List<string>{ "I3" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I4Fake = new PieceTypeDef{Id = 1000905, Abbreviations = new List<string>{ "I4Fake" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I4     = new PieceTypeDef{Id = 1000906, Abbreviations = new List<string>{ "I4" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I5Fake = new PieceTypeDef{Id = 1000907, Abbreviations = new List<string>{ "I5Fake" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I5     = new PieceTypeDef{Id = 1000908, Abbreviations = new List<string>{ "I5" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I6Fake = new PieceTypeDef{Id = 1000909, Abbreviations = new List<string>{ "I6Fake" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I6     = new PieceTypeDef{Id = 1000910, Abbreviations = new List<string>{ "I6" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I7Fake = new PieceTypeDef{Id = 1000911, Abbreviations = new List<string>{ "I7Fake" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I7     = new PieceTypeDef{Id = 1000912, Abbreviations = new List<string>{ "I7" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I8Fake = new PieceTypeDef{Id = 1000913, Abbreviations = new List<string>{ "I8Fake" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I8     = new PieceTypeDef{Id = 1000914, Abbreviations = new List<string>{ "I8" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I9Fake = new PieceTypeDef{Id = 1000915, Abbreviations = new List<string>{ "I9Fake" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef I9     = new PieceTypeDef{Id = 1000916, Abbreviations = new List<string>{ "I9" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef IMFake = new PieceTypeDef{Id = 1000990, Abbreviations = new List<string>{ "IMFake" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef IM     = new PieceTypeDef{Id = 1000991, Abbreviations = new List<string>{ "IM" },     Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
    #region J
    
    public static readonly PieceTypeDef J1     = new PieceTypeDef{Id = 1001001, Abbreviations = new List<string>{ "J1" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J2     = new PieceTypeDef{Id = 1001002, Abbreviations = new List<string>{ "J2" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J3Fake = new PieceTypeDef{Id = 1001003, Abbreviations = new List<string>{ "J3Fake" }, Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J3     = new PieceTypeDef{Id = 1001004, Abbreviations = new List<string>{ "J3" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J4Fake = new PieceTypeDef{Id = 1001005, Abbreviations = new List<string>{ "J4Fake" }, Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J4     = new PieceTypeDef{Id = 1001006, Abbreviations = new List<string>{ "J4" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J5Fake = new PieceTypeDef{Id = 1001007, Abbreviations = new List<string>{ "J5Fake" }, Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J5     = new PieceTypeDef{Id = 1001008, Abbreviations = new List<string>{ "J5" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J6Fake = new PieceTypeDef{Id = 1001009, Abbreviations = new List<string>{ "J6Fake" }, Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J6     = new PieceTypeDef{Id = 1001010, Abbreviations = new List<string>{ "J6" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J7Fake = new PieceTypeDef{Id = 1001011, Abbreviations = new List<string>{ "J7Fake" }, Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J7     = new PieceTypeDef{Id = 1001012, Abbreviations = new List<string>{ "J7" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J8Fake = new PieceTypeDef{Id = 1001013, Abbreviations = new List<string>{ "J8Fake" }, Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J8     = new PieceTypeDef{Id = 1001014, Abbreviations = new List<string>{ "J8" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J9Fake = new PieceTypeDef{Id = 1001015, Abbreviations = new List<string>{ "J9Fake" }, Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef J9     = new PieceTypeDef{Id = 1001016, Abbreviations = new List<string>{ "J9" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef JMFake = new PieceTypeDef{Id = 1001090, Abbreviations = new List<string>{ "JMFake" }, Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef JM     = new PieceTypeDef{Id = 1001091, Abbreviations = new List<string>{ "JM" },     Branch = PieceTypeBranch.J, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Normal | PieceTypeFilter.Analytics};
    
    #endregion
    
#endregion

#region Characters

    public static readonly PieceTypeDef NPC_SleepingBeautyPlaid = new PieceTypeDef{Id = 3100000, Abbreviations = new List<string>{ "NPC_SleepingBeautyPlaid" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_Gnome = new PieceTypeDef{Id = 3100001, Abbreviations = new List<string>{ "NPC_Gnome" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Fake};

    public static readonly PieceTypeDef NPC_A = new PieceTypeDef{Id = 3000100, Abbreviations = new List<string>{ "NPC_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character};
    public static readonly PieceTypeDef NPC_B = new PieceTypeDef{Id = 3000200, Abbreviations = new List<string>{ "NPC_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_C = new PieceTypeDef{Id = 3000300, Abbreviations = new List<string>{ "NPC_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics};   
    public static readonly PieceTypeDef NPC_D = new PieceTypeDef{Id = 3000400, Abbreviations = new List<string>{ "NPC_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_E = new PieceTypeDef{Id = 3000500, Abbreviations = new List<string>{ "NPC_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_F = new PieceTypeDef{Id = 3000600, Abbreviations = new List<string>{ "NPC_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_G = new PieceTypeDef{Id = 3000700, Abbreviations = new List<string>{ "NPC_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_H = new PieceTypeDef{Id = 3000800, Abbreviations = new List<string>{ "NPC_H" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_I = new PieceTypeDef{Id = 3000900, Abbreviations = new List<string>{ "NPC_I" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_J = new PieceTypeDef{Id = 3001000, Abbreviations = new List<string>{ "NPC_J" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_K = new PieceTypeDef{Id = 3001100, Abbreviations = new List<string>{ "NPC_K" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_L = new PieceTypeDef{Id = 3001200, Abbreviations = new List<string>{ "NPC_L" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_M = new PieceTypeDef{Id = 3001300, Abbreviations = new List<string>{ "NPC_M" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_N = new PieceTypeDef{Id = 3001400, Abbreviations = new List<string>{ "NPC_N" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_O = new PieceTypeDef{Id = 3001500, Abbreviations = new List<string>{ "NPC_O" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_P = new PieceTypeDef{Id = 3001600, Abbreviations = new List<string>{ "NPC_P" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_Q = new PieceTypeDef{Id = 3001700, Abbreviations = new List<string>{ "NPC_Q" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    public static readonly PieceTypeDef NPC_R = new PieceTypeDef{Id = 3001800, Abbreviations = new List<string>{ "NPC_R" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Character | PieceTypeFilter.Analytics | PieceTypeFilter.Fake};
    
#endregion

#region Character pieces

    public static readonly PieceTypeDef NPC_B1 = new PieceTypeDef{Id = 3000201, Abbreviations = new List<string>{ "NPC_B1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_B2 = new PieceTypeDef{Id = 3000202, Abbreviations = new List<string>{ "NPC_B2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_B3 = new PieceTypeDef{Id = 3000203, Abbreviations = new List<string>{ "NPC_B3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_B4 = new PieceTypeDef{Id = 3000204, Abbreviations = new List<string>{ "NPC_B4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef NPC_C1 = new PieceTypeDef{Id = 3000301, Abbreviations = new List<string>{ "NPC_C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_C2 = new PieceTypeDef{Id = 3000302, Abbreviations = new List<string>{ "NPC_C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_C3 = new PieceTypeDef{Id = 3000303, Abbreviations = new List<string>{ "NPC_C3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_C4 = new PieceTypeDef{Id = 3000304, Abbreviations = new List<string>{ "NPC_C4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_C5 = new PieceTypeDef{Id = 3000305, Abbreviations = new List<string>{ "NPC_C5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_C6 = new PieceTypeDef{Id = 3000306, Abbreviations = new List<string>{ "NPC_C6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_C7 = new PieceTypeDef{Id = 3000307, Abbreviations = new List<string>{ "NPC_C7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_C8 = new PieceTypeDef{Id = 3000308, Abbreviations = new List<string>{ "NPC_C8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef NPC_D1 = new PieceTypeDef{Id = 3000401, Abbreviations = new List<string>{ "NPC_D1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_D2 = new PieceTypeDef{Id = 3000402, Abbreviations = new List<string>{ "NPC_D2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_D3 = new PieceTypeDef{Id = 3000403, Abbreviations = new List<string>{ "NPC_D3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_D4 = new PieceTypeDef{Id = 3000404, Abbreviations = new List<string>{ "NPC_D4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_D5 = new PieceTypeDef{Id = 3000405, Abbreviations = new List<string>{ "NPC_D5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_D6 = new PieceTypeDef{Id = 3000406, Abbreviations = new List<string>{ "NPC_D6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_D7 = new PieceTypeDef{Id = 3000407, Abbreviations = new List<string>{ "NPC_D7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_D8 = new PieceTypeDef{Id = 3000408, Abbreviations = new List<string>{ "NPC_D8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
     
    public static readonly PieceTypeDef NPC_E1 = new PieceTypeDef{Id = 3000501, Abbreviations = new List<string>{ "NPC_E1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_E2 = new PieceTypeDef{Id = 3000502, Abbreviations = new List<string>{ "NPC_E2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_E3 = new PieceTypeDef{Id = 3000503, Abbreviations = new List<string>{ "NPC_E3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_E4 = new PieceTypeDef{Id = 3000504, Abbreviations = new List<string>{ "NPC_E4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_E5 = new PieceTypeDef{Id = 3000505, Abbreviations = new List<string>{ "NPC_E5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_E6 = new PieceTypeDef{Id = 3000506, Abbreviations = new List<string>{ "NPC_E6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_E7 = new PieceTypeDef{Id = 3000507, Abbreviations = new List<string>{ "NPC_E7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_E8 = new PieceTypeDef{Id = 3000508, Abbreviations = new List<string>{ "NPC_E8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
 
    public static readonly PieceTypeDef NPC_F1 = new PieceTypeDef{Id = 3000601, Abbreviations = new List<string>{ "NPC_F1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_F2 = new PieceTypeDef{Id = 3000602, Abbreviations = new List<string>{ "NPC_F2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_F3 = new PieceTypeDef{Id = 3000603, Abbreviations = new List<string>{ "NPC_F3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_F4 = new PieceTypeDef{Id = 3000604, Abbreviations = new List<string>{ "NPC_F4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_F5 = new PieceTypeDef{Id = 3000605, Abbreviations = new List<string>{ "NPC_F5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_F6 = new PieceTypeDef{Id = 3000606, Abbreviations = new List<string>{ "NPC_F6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_F7 = new PieceTypeDef{Id = 3000607, Abbreviations = new List<string>{ "NPC_F7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_F8 = new PieceTypeDef{Id = 3000608, Abbreviations = new List<string>{ "NPC_F8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_G1 = new PieceTypeDef{Id = 3000701, Abbreviations = new List<string>{ "NPC_G1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_G2 = new PieceTypeDef{Id = 3000702, Abbreviations = new List<string>{ "NPC_G2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_G3 = new PieceTypeDef{Id = 3000703, Abbreviations = new List<string>{ "NPC_G3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_G4 = new PieceTypeDef{Id = 3000704, Abbreviations = new List<string>{ "NPC_G4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_G5 = new PieceTypeDef{Id = 3000705, Abbreviations = new List<string>{ "NPC_G5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_G6 = new PieceTypeDef{Id = 3000706, Abbreviations = new List<string>{ "NPC_G6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_G7 = new PieceTypeDef{Id = 3000707, Abbreviations = new List<string>{ "NPC_G7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_G8 = new PieceTypeDef{Id = 3000708, Abbreviations = new List<string>{ "NPC_G8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_H1 = new PieceTypeDef{Id = 3000801, Abbreviations = new List<string>{ "NPC_H1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_H2 = new PieceTypeDef{Id = 3000802, Abbreviations = new List<string>{ "NPC_H2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_H3 = new PieceTypeDef{Id = 3000803, Abbreviations = new List<string>{ "NPC_H3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_H4 = new PieceTypeDef{Id = 3000804, Abbreviations = new List<string>{ "NPC_H4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_H5 = new PieceTypeDef{Id = 3000805, Abbreviations = new List<string>{ "NPC_H5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_H6 = new PieceTypeDef{Id = 3000806, Abbreviations = new List<string>{ "NPC_H6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_H7 = new PieceTypeDef{Id = 3000807, Abbreviations = new List<string>{ "NPC_H7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_H8 = new PieceTypeDef{Id = 3000808, Abbreviations = new List<string>{ "NPC_H8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_I1 = new PieceTypeDef{Id = 3000901, Abbreviations = new List<string>{ "NPC_I1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_I2 = new PieceTypeDef{Id = 3000902, Abbreviations = new List<string>{ "NPC_I2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_I3 = new PieceTypeDef{Id = 3000903, Abbreviations = new List<string>{ "NPC_I3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_I4 = new PieceTypeDef{Id = 3000904, Abbreviations = new List<string>{ "NPC_I4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_I5 = new PieceTypeDef{Id = 3000905, Abbreviations = new List<string>{ "NPC_I5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_I6 = new PieceTypeDef{Id = 3000906, Abbreviations = new List<string>{ "NPC_I6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_I7 = new PieceTypeDef{Id = 3000907, Abbreviations = new List<string>{ "NPC_I7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_I8 = new PieceTypeDef{Id = 3000908, Abbreviations = new List<string>{ "NPC_I8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_J1 = new PieceTypeDef{Id = 3001001, Abbreviations = new List<string>{ "NPC_J1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_J2 = new PieceTypeDef{Id = 3001002, Abbreviations = new List<string>{ "NPC_J2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_J3 = new PieceTypeDef{Id = 3001003, Abbreviations = new List<string>{ "NPC_J3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_J4 = new PieceTypeDef{Id = 3001004, Abbreviations = new List<string>{ "NPC_J4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_J5 = new PieceTypeDef{Id = 3001005, Abbreviations = new List<string>{ "NPC_J5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_J6 = new PieceTypeDef{Id = 3001006, Abbreviations = new List<string>{ "NPC_J6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_J7 = new PieceTypeDef{Id = 3001007, Abbreviations = new List<string>{ "NPC_J7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_J8 = new PieceTypeDef{Id = 3001008, Abbreviations = new List<string>{ "NPC_J8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_K1 = new PieceTypeDef{Id = 3001101, Abbreviations = new List<string>{ "NPC_K1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_K2 = new PieceTypeDef{Id = 3001102, Abbreviations = new List<string>{ "NPC_K2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_K3 = new PieceTypeDef{Id = 3001103, Abbreviations = new List<string>{ "NPC_K3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_K4 = new PieceTypeDef{Id = 3001104, Abbreviations = new List<string>{ "NPC_K4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_K5 = new PieceTypeDef{Id = 3001105, Abbreviations = new List<string>{ "NPC_K5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_K6 = new PieceTypeDef{Id = 3001106, Abbreviations = new List<string>{ "NPC_K6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_K7 = new PieceTypeDef{Id = 3001107, Abbreviations = new List<string>{ "NPC_K7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_K8 = new PieceTypeDef{Id = 3001108, Abbreviations = new List<string>{ "NPC_K8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_L1 = new PieceTypeDef{Id = 3001201, Abbreviations = new List<string>{ "NPC_L1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_L2 = new PieceTypeDef{Id = 3001202, Abbreviations = new List<string>{ "NPC_L2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_L3 = new PieceTypeDef{Id = 3001203, Abbreviations = new List<string>{ "NPC_L3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_L4 = new PieceTypeDef{Id = 3001204, Abbreviations = new List<string>{ "NPC_L4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_L5 = new PieceTypeDef{Id = 3001205, Abbreviations = new List<string>{ "NPC_L5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_L6 = new PieceTypeDef{Id = 3001206, Abbreviations = new List<string>{ "NPC_L6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_L7 = new PieceTypeDef{Id = 3001207, Abbreviations = new List<string>{ "NPC_L7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_L8 = new PieceTypeDef{Id = 3001208, Abbreviations = new List<string>{ "NPC_L8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_M1 = new PieceTypeDef{Id = 3001301, Abbreviations = new List<string>{ "NPC_M1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_M2 = new PieceTypeDef{Id = 3001302, Abbreviations = new List<string>{ "NPC_M2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_M3 = new PieceTypeDef{Id = 3001303, Abbreviations = new List<string>{ "NPC_M3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_M4 = new PieceTypeDef{Id = 3001304, Abbreviations = new List<string>{ "NPC_M4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_M5 = new PieceTypeDef{Id = 3001305, Abbreviations = new List<string>{ "NPC_M5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_M6 = new PieceTypeDef{Id = 3001306, Abbreviations = new List<string>{ "NPC_M6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_M7 = new PieceTypeDef{Id = 3001307, Abbreviations = new List<string>{ "NPC_M7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_M8 = new PieceTypeDef{Id = 3001308, Abbreviations = new List<string>{ "NPC_M8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_N1 = new PieceTypeDef{Id = 3001401, Abbreviations = new List<string>{ "NPC_N1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_N2 = new PieceTypeDef{Id = 3001402, Abbreviations = new List<string>{ "NPC_N2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_N3 = new PieceTypeDef{Id = 3001403, Abbreviations = new List<string>{ "NPC_N3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_N4 = new PieceTypeDef{Id = 3001404, Abbreviations = new List<string>{ "NPC_N4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_N5 = new PieceTypeDef{Id = 3001405, Abbreviations = new List<string>{ "NPC_N5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_N6 = new PieceTypeDef{Id = 3001406, Abbreviations = new List<string>{ "NPC_N6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_N7 = new PieceTypeDef{Id = 3001407, Abbreviations = new List<string>{ "NPC_N7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_N8 = new PieceTypeDef{Id = 3001408, Abbreviations = new List<string>{ "NPC_N8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_O1 = new PieceTypeDef{Id = 3001501, Abbreviations = new List<string>{ "NPC_O1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_O2 = new PieceTypeDef{Id = 3001502, Abbreviations = new List<string>{ "NPC_O2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_O3 = new PieceTypeDef{Id = 3001503, Abbreviations = new List<string>{ "NPC_O3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_O4 = new PieceTypeDef{Id = 3001504, Abbreviations = new List<string>{ "NPC_O4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_O5 = new PieceTypeDef{Id = 3001505, Abbreviations = new List<string>{ "NPC_O5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_O6 = new PieceTypeDef{Id = 3001506, Abbreviations = new List<string>{ "NPC_O6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_O7 = new PieceTypeDef{Id = 3001507, Abbreviations = new List<string>{ "NPC_O7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_O8 = new PieceTypeDef{Id = 3001508, Abbreviations = new List<string>{ "NPC_O8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_P1 = new PieceTypeDef{Id = 3001601, Abbreviations = new List<string>{ "NPC_P1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_P2 = new PieceTypeDef{Id = 3001602, Abbreviations = new List<string>{ "NPC_P2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_P3 = new PieceTypeDef{Id = 3001603, Abbreviations = new List<string>{ "NPC_P3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_P4 = new PieceTypeDef{Id = 3001604, Abbreviations = new List<string>{ "NPC_P4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_P5 = new PieceTypeDef{Id = 3001605, Abbreviations = new List<string>{ "NPC_P5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_P6 = new PieceTypeDef{Id = 3001606, Abbreviations = new List<string>{ "NPC_P6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_P7 = new PieceTypeDef{Id = 3001607, Abbreviations = new List<string>{ "NPC_P7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_P8 = new PieceTypeDef{Id = 3001608, Abbreviations = new List<string>{ "NPC_P8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_Q1 = new PieceTypeDef{Id = 3001701, Abbreviations = new List<string>{ "NPC_Q1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_Q2 = new PieceTypeDef{Id = 3001702, Abbreviations = new List<string>{ "NPC_Q2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_Q3 = new PieceTypeDef{Id = 3001703, Abbreviations = new List<string>{ "NPC_Q3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_Q4 = new PieceTypeDef{Id = 3001704, Abbreviations = new List<string>{ "NPC_Q4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_Q5 = new PieceTypeDef{Id = 3001705, Abbreviations = new List<string>{ "NPC_Q5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_Q6 = new PieceTypeDef{Id = 3001706, Abbreviations = new List<string>{ "NPC_Q6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_Q7 = new PieceTypeDef{Id = 3001707, Abbreviations = new List<string>{ "NPC_Q7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_Q8 = new PieceTypeDef{Id = 3001708, Abbreviations = new List<string>{ "NPC_Q8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef NPC_R1 = new PieceTypeDef{Id = 3001801, Abbreviations = new List<string>{ "NPC_R1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_R2 = new PieceTypeDef{Id = 3001802, Abbreviations = new List<string>{ "NPC_R2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_R3 = new PieceTypeDef{Id = 3001803, Abbreviations = new List<string>{ "NPC_R3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_R4 = new PieceTypeDef{Id = 3001804, Abbreviations = new List<string>{ "NPC_R4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_R5 = new PieceTypeDef{Id = 3001805, Abbreviations = new List<string>{ "NPC_R5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_R6 = new PieceTypeDef{Id = 3001806, Abbreviations = new List<string>{ "NPC_R6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_R7 = new PieceTypeDef{Id = 3001807, Abbreviations = new List<string>{ "NPC_R7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef NPC_R8 = new PieceTypeDef{Id = 3001808, Abbreviations = new List<string>{ "NPC_R8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef NPC_Z8 = new PieceTypeDef{Id = 3002608, Abbreviations = new List<string>{ "NPC_Z8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Analytics};

#endregion
  
#region Chests
    
    public static readonly PieceTypeDef CH1_A   = new PieceTypeDef{Id = 5000101, Abbreviations = new List<string>{ "CH1_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_A   = new PieceTypeDef{Id = 5000102, Abbreviations = new List<string>{ "CH2_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_A   = new PieceTypeDef{Id = 5000103, Abbreviations = new List<string>{ "CH3_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_B   = new PieceTypeDef{Id = 5000201, Abbreviations = new List<string>{ "CH1_B" },   Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_B   = new PieceTypeDef{Id = 5000202, Abbreviations = new List<string>{ "CH2_B" },   Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_B   = new PieceTypeDef{Id = 5000203, Abbreviations = new List<string>{ "CH3_B" },   Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_C   = new PieceTypeDef{Id = 5000301, Abbreviations = new List<string>{ "CH1_C" },   Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_C   = new PieceTypeDef{Id = 5000302, Abbreviations = new List<string>{ "CH2_C" },   Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_C   = new PieceTypeDef{Id = 5000303, Abbreviations = new List<string>{ "CH3_C" },   Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_D   = new PieceTypeDef{Id = 5000401, Abbreviations = new List<string>{ "CH1_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_D   = new PieceTypeDef{Id = 5000402, Abbreviations = new List<string>{ "CH2_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_D   = new PieceTypeDef{Id = 5000403, Abbreviations = new List<string>{ "CH3_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_E   = new PieceTypeDef{Id = 5000501, Abbreviations = new List<string>{ "CH1_E" },   Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_E   = new PieceTypeDef{Id = 5000502, Abbreviations = new List<string>{ "CH2_E" },   Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_E   = new PieceTypeDef{Id = 5000503, Abbreviations = new List<string>{ "CH3_E" },   Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_F   = new PieceTypeDef{Id = 5000601, Abbreviations = new List<string>{ "CH1_F" },   Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_F   = new PieceTypeDef{Id = 5000602, Abbreviations = new List<string>{ "CH2_F" },   Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_F   = new PieceTypeDef{Id = 5000603, Abbreviations = new List<string>{ "CH3_F" },   Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_G   = new PieceTypeDef{Id = 5000701, Abbreviations = new List<string>{ "CH1_G" },   Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_G   = new PieceTypeDef{Id = 5000702, Abbreviations = new List<string>{ "CH2_G" },   Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_G   = new PieceTypeDef{Id = 5000703, Abbreviations = new List<string>{ "CH3_G" },   Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef CH1_H   = new PieceTypeDef{Id = 5000801, Abbreviations = new List<string>{ "CH1_H" },   Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_H   = new PieceTypeDef{Id = 5000802, Abbreviations = new List<string>{ "CH2_H" },   Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_H   = new PieceTypeDef{Id = 5000803, Abbreviations = new List<string>{ "CH3_H" },   Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
         
    public static readonly PieceTypeDef CH1_I   = new PieceTypeDef{Id = 5000901, Abbreviations = new List<string>{ "CH1_I" },   Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH2_I   = new PieceTypeDef{Id = 5000902, Abbreviations = new List<string>{ "CH2_I" },   Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH3_I   = new PieceTypeDef{Id = 5000903, Abbreviations = new List<string>{ "CH3_I" },   Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    
    public static readonly PieceTypeDef SK1_PR  = new PieceTypeDef{Id = 5100101, Abbreviations = new List<string>{ "SK1_PR" },  Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Bag};
    public static readonly PieceTypeDef SK2_PR  = new PieceTypeDef{Id = 5100102, Abbreviations = new List<string>{ "SK2_PR" },  Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Bag};
    public static readonly PieceTypeDef SK3_PR  = new PieceTypeDef{Id = 5100103, Abbreviations = new List<string>{ "SK3_PR" },  Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest | PieceTypeFilter.Bag};
    
    public static readonly PieceTypeDef CH_Free = new PieceTypeDef{Id = 5200001, Abbreviations = new List<string>{ "CH_Free" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};
    public static readonly PieceTypeDef CH_NPC  = new PieceTypeDef{Id = 5300001, Abbreviations = new List<string>{ "CH_NPC" },  Filter = PieceTypeFilter.Simple | PieceTypeFilter.Chest};

#endregion
   
#region Currencies
    
    public static readonly PieceTypeDef Mana1 = new PieceTypeDef{Id = 7000101, Abbreviations = new List<string>{ "Mana1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Mana2 = new PieceTypeDef{Id = 7000102, Abbreviations = new List<string>{ "Mana2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Mana3 = new PieceTypeDef{Id = 7000103, Abbreviations = new List<string>{ "Mana3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics}; 
    public static readonly PieceTypeDef Mana4 = new PieceTypeDef{Id = 7000104, Abbreviations = new List<string>{ "Mana4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Mana5 = new PieceTypeDef{Id = 7000105, Abbreviations = new List<string>{ "Mana5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Mana6 = new PieceTypeDef{Id = 7000106, Abbreviations = new List<string>{ "Mana6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    
    public static readonly PieceTypeDef Hard1 = new PieceTypeDef{Id = 7100101, Abbreviations = new List<string>{ "Hard1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Hard2 = new PieceTypeDef{Id = 7100102, Abbreviations = new List<string>{ "Hard2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Hard3 = new PieceTypeDef{Id = 7100103, Abbreviations = new List<string>{ "Hard3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics}; 
    public static readonly PieceTypeDef Hard4 = new PieceTypeDef{Id = 7100104, Abbreviations = new List<string>{ "Hard4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Hard5 = new PieceTypeDef{Id = 7100105, Abbreviations = new List<string>{ "Hard5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Hard6 = new PieceTypeDef{Id = 7100106, Abbreviations = new List<string>{ "Hard6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};

    public static readonly PieceTypeDef Soft1 = new PieceTypeDef{Id = 7200101, Abbreviations = new List<string>{ "Soft1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Soft2 = new PieceTypeDef{Id = 7200102, Abbreviations = new List<string>{ "Soft2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Soft3 = new PieceTypeDef{Id = 7200103, Abbreviations = new List<string>{ "Soft3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics}; 
    public static readonly PieceTypeDef Soft4 = new PieceTypeDef{Id = 7200104, Abbreviations = new List<string>{ "Soft4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Soft5 = new PieceTypeDef{Id = 7200105, Abbreviations = new List<string>{ "Soft5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Soft6 = new PieceTypeDef{Id = 7200106, Abbreviations = new List<string>{ "Soft6" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Soft7 = new PieceTypeDef{Id = 7200107, Abbreviations = new List<string>{ "Soft7" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};
    public static readonly PieceTypeDef Soft8 = new PieceTypeDef{Id = 7200108, Abbreviations = new List<string>{ "Soft8" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Analytics};    
     
    public static readonly PieceTypeDef Token1 = new PieceTypeDef{Id = 7500101, Abbreviations = new List<string>{ "Token1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Token2 = new PieceTypeDef{Id = 7500102, Abbreviations = new List<string>{ "Token2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    public static readonly PieceTypeDef Token3 = new PieceTypeDef{Id = 7500103, Abbreviations = new List<string>{ "Token3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource};
    
#endregion

#region Boosters
    
    public static readonly PieceTypeDef Boost_CR  = new PieceTypeDef{Id = 9000100, Abbreviations = new List<string>{ "Boost_CR"  }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Booster};
    
    public static readonly PieceTypeDef Boost_CR1 = new PieceTypeDef{Id = 9000101, Abbreviations = new List<string>{ "Boost_CR1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_CR2 = new PieceTypeDef{Id = 9000102, Abbreviations = new List<string>{ "Boost_CR2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_CR3 = new PieceTypeDef{Id = 9000103, Abbreviations = new List<string>{ "Boost_CR3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    
    public static readonly PieceTypeDef Boost_WR  = new PieceTypeDef{Id = 9000200, Abbreviations = new List<string>{ "Boost_WR" },  Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    
    public static readonly PieceTypeDef Boost_WR1 = new PieceTypeDef{Id = 9000201, Abbreviations = new List<string>{ "Boost_WR1" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_WR2 = new PieceTypeDef{Id = 9000202, Abbreviations = new List<string>{ "Boost_WR2" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_WR3 = new PieceTypeDef{Id = 9000203, Abbreviations = new List<string>{ "Boost_WR3" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef Boost_WR4 = new PieceTypeDef{Id = 9000204, Abbreviations = new List<string>{ "Boost_WR4" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    
#endregion

#region Orders
    
    public static readonly PieceTypeDef RC_A = new PieceTypeDef{Id = 11000101, Abbreviations = new List<string>{ "RC_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_B = new PieceTypeDef{Id = 11000201, Abbreviations = new List<string>{ "RC_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_C = new PieceTypeDef{Id = 11000301, Abbreviations = new List<string>{ "RC_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_D = new PieceTypeDef{Id = 11000401, Abbreviations = new List<string>{ "RC_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_E = new PieceTypeDef{Id = 11000501, Abbreviations = new List<string>{ "RC_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_F = new PieceTypeDef{Id = 11000601, Abbreviations = new List<string>{ "RC_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_G = new PieceTypeDef{Id = 11000701, Abbreviations = new List<string>{ "RC_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_H = new PieceTypeDef{Id = 11000801, Abbreviations = new List<string>{ "RC_H" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_I = new PieceTypeDef{Id = 11000901, Abbreviations = new List<string>{ "RC_I" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_J = new PieceTypeDef{Id = 11001001, Abbreviations = new List<string>{ "RC_J" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_K = new PieceTypeDef{Id = 11001101, Abbreviations = new List<string>{ "RC_K" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_L = new PieceTypeDef{Id = 11001201, Abbreviations = new List<string>{ "RC_L" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_M = new PieceTypeDef{Id = 11001301, Abbreviations = new List<string>{ "RC_M" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_N = new PieceTypeDef{Id = 11001401, Abbreviations = new List<string>{ "RC_N" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_O = new PieceTypeDef{Id = 11001501, Abbreviations = new List<string>{ "RC_O" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_P = new PieceTypeDef{Id = 11001601, Abbreviations = new List<string>{ "RC_P" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_Q = new PieceTypeDef{Id = 11001701, Abbreviations = new List<string>{ "RC_Q" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_R = new PieceTypeDef{Id = 11001801, Abbreviations = new List<string>{ "RC_R" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_S = new PieceTypeDef{Id = 11001901, Abbreviations = new List<string>{ "RC_S" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_T = new PieceTypeDef{Id = 11002001, Abbreviations = new List<string>{ "RC_T" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_U = new PieceTypeDef{Id = 11002101, Abbreviations = new List<string>{ "RC_U" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_V = new PieceTypeDef{Id = 11002201, Abbreviations = new List<string>{ "RC_V" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    public static readonly PieceTypeDef RC_W = new PieceTypeDef{Id = 11002301, Abbreviations = new List<string>{ "RC_W" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.OrderPiece};
    
#endregion

#region Reproduction Pieces
    
    #region PR_A
    
    public static readonly PieceTypeDef PR_A1 = new PieceTypeDef{Id = 13000101, Abbreviations = new List<string>{ "PR_A1" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A2 = new PieceTypeDef{Id = 13000102, Abbreviations = new List<string>{ "PR_A2" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A3 = new PieceTypeDef{Id = 13000103, Abbreviations = new List<string>{ "PR_A3" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_A4 = new PieceTypeDef{Id = 13000104, Abbreviations = new List<string>{ "PR_A4" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_A5 = new PieceTypeDef{Id = 13000105, Abbreviations = new List<string>{ "PR_A5" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_B
    
    public static readonly PieceTypeDef PR_B1 = new PieceTypeDef{Id = 13000201, Abbreviations = new List<string>{ "PR_B1" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B2 = new PieceTypeDef{Id = 13000202, Abbreviations = new List<string>{ "PR_B2" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B3 = new PieceTypeDef{Id = 13000203, Abbreviations = new List<string>{ "PR_B3" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_B4 = new PieceTypeDef{Id = 13000204, Abbreviations = new List<string>{ "PR_B4" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_B5 = new PieceTypeDef{Id = 13000205, Abbreviations = new List<string>{ "PR_B5" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_C
    
    public static readonly PieceTypeDef PR_C1 = new PieceTypeDef{Id = 13000301, Abbreviations = new List<string>{ "PR_C1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C2 = new PieceTypeDef{Id = 13000302, Abbreviations = new List<string>{ "PR_C2" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C3 = new PieceTypeDef{Id = 13000303, Abbreviations = new List<string>{ "PR_C3" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_C4 = new PieceTypeDef{Id = 13000304, Abbreviations = new List<string>{ "PR_C4" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_C5 = new PieceTypeDef{Id = 13000305, Abbreviations = new List<string>{ "PR_C5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_D
    
    public static readonly PieceTypeDef PR_D1 = new PieceTypeDef{Id = 13000401, Abbreviations = new List<string>{ "PR_D1" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D2 = new PieceTypeDef{Id = 13000402, Abbreviations = new List<string>{ "PR_D2" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D3 = new PieceTypeDef{Id = 13000403, Abbreviations = new List<string>{ "PR_D3" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_D4 = new PieceTypeDef{Id = 13000404, Abbreviations = new List<string>{ "PR_D4" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_D5 = new PieceTypeDef{Id = 13000405, Abbreviations = new List<string>{ "PR_D5" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_E
    
    public static readonly PieceTypeDef PR_E1 = new PieceTypeDef{Id = 13000501, Abbreviations = new List<string>{ "PR_E1" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E2 = new PieceTypeDef{Id = 13000502, Abbreviations = new List<string>{ "PR_E2" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E3 = new PieceTypeDef{Id = 13000503, Abbreviations = new List<string>{ "PR_E3" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_E4 = new PieceTypeDef{Id = 13000504, Abbreviations = new List<string>{ "PR_E4" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_E5 = new PieceTypeDef{Id = 13000505, Abbreviations = new List<string>{ "PR_E5" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_F
    
    public static readonly PieceTypeDef PR_F1 = new PieceTypeDef{Id = 13000601, Abbreviations = new List<string>{ "PR_F1" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F2 = new PieceTypeDef{Id = 13000602, Abbreviations = new List<string>{ "PR_F2" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F3 = new PieceTypeDef{Id = 13000603, Abbreviations = new List<string>{ "PR_F3" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_F4 = new PieceTypeDef{Id = 13000604, Abbreviations = new List<string>{ "PR_F4" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_F5 = new PieceTypeDef{Id = 13000605, Abbreviations = new List<string>{ "PR_F5" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    #region PR_G
    
    public static readonly PieceTypeDef PR_G1 = new PieceTypeDef{Id = 13000701, Abbreviations = new List<string>{ "PR_G1" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G2 = new PieceTypeDef{Id = 13000702, Abbreviations = new List<string>{ "PR_G2" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G3 = new PieceTypeDef{Id = 13000703, Abbreviations = new List<string>{ "PR_G3" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef PR_G4 = new PieceTypeDef{Id = 13000704, Abbreviations = new List<string>{ "PR_G4" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef PR_G5 = new PieceTypeDef{Id = 13000705, Abbreviations = new List<string>{ "PR_G5" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
    #endregion
    
    public static readonly PieceTypeDef PR_Z5 = new PieceTypeDef{Id = 13002605, Abbreviations = new List<string>{ "PR_Z5" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Resource | PieceTypeFilter.Ingredient};
    
#endregion
    
#region Obstacles
    
    public static readonly PieceTypeDef OB_PR_A = new PieceTypeDef{Id = 15000101, Abbreviations = new List<string>{ "OB_PR_A" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_B = new PieceTypeDef{Id = 15000201, Abbreviations = new List<string>{ "OB_PR_B" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_C = new PieceTypeDef{Id = 15000301, Abbreviations = new List<string>{ "OB_PR_C" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_D = new PieceTypeDef{Id = 15000401, Abbreviations = new List<string>{ "OB_PR_D" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_E = new PieceTypeDef{Id = 15000501, Abbreviations = new List<string>{ "OB_PR_E" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_F = new PieceTypeDef{Id = 15000601, Abbreviations = new List<string>{ "OB_PR_F" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    public static readonly PieceTypeDef OB_PR_G = new PieceTypeDef{Id = 15000701, Abbreviations = new List<string>{ "OB_PR_G" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.ProductionField};
    
    public static readonly PieceTypeDef OB1_TT  = new PieceTypeDef{Id = 17000001, Abbreviations = new List<string>{ "OB1_TT" },  Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_TT  = new PieceTypeDef{Id = 17000002, Abbreviations = new List<string>{ "OB2_TT" },  Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
     
    public static readonly PieceTypeDef OB1_A   = new PieceTypeDef{Id = 17000101, Abbreviations = new List<string>{ "OB1_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_A   = new PieceTypeDef{Id = 17000102, Abbreviations = new List<string>{ "OB2_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB3_A   = new PieceTypeDef{Id = 17000103, Abbreviations = new List<string>{ "OB3_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB4_A   = new PieceTypeDef{Id = 17000104, Abbreviations = new List<string>{ "OB4_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB5_A   = new PieceTypeDef{Id = 17000105, Abbreviations = new List<string>{ "OB5_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB6_A   = new PieceTypeDef{Id = 17000106, Abbreviations = new List<string>{ "OB6_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB7_A   = new PieceTypeDef{Id = 17000107, Abbreviations = new List<string>{ "OB7_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB8_A   = new PieceTypeDef{Id = 17000108, Abbreviations = new List<string>{ "OB8_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB9_A   = new PieceTypeDef{Id = 17000109, Abbreviations = new List<string>{ "OB9_A" },   Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    
    public static readonly PieceTypeDef OB1_D   = new PieceTypeDef{Id = 17000401, Abbreviations = new List<string>{ "OB1_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_D   = new PieceTypeDef{Id = 17000402, Abbreviations = new List<string>{ "OB2_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB3_D   = new PieceTypeDef{Id = 17000403, Abbreviations = new List<string>{ "OB3_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB4_D   = new PieceTypeDef{Id = 17000404, Abbreviations = new List<string>{ "OB4_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB5_D   = new PieceTypeDef{Id = 17000405, Abbreviations = new List<string>{ "OB5_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB6_D   = new PieceTypeDef{Id = 17000406, Abbreviations = new List<string>{ "OB6_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB7_D   = new PieceTypeDef{Id = 17000407, Abbreviations = new List<string>{ "OB7_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB8_D   = new PieceTypeDef{Id = 17000408, Abbreviations = new List<string>{ "OB8_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB9_D   = new PieceTypeDef{Id = 17000409, Abbreviations = new List<string>{ "OB9_D" },   Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    
    public static readonly PieceTypeDef OB1_G   = new PieceTypeDef{Id = 17000701, Abbreviations = new List<string>{ "OB1_G" },   Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB2_G   = new PieceTypeDef{Id = 17000702, Abbreviations = new List<string>{ "OB2_G" },   Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB3_G   = new PieceTypeDef{Id = 17000703, Abbreviations = new List<string>{ "OB3_G" },   Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB4_G   = new PieceTypeDef{Id = 17000704, Abbreviations = new List<string>{ "OB4_G" },   Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    public static readonly PieceTypeDef OB5_G   = new PieceTypeDef{Id = 17000705, Abbreviations = new List<string>{ "OB5_G" },   Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Obstacle | PieceTypeFilter.Tree};
    
#endregion
   
#region Mines

    public static readonly PieceTypeDef MN_A      = new PieceTypeDef{Id = 19000101, Abbreviations = new List<string>{ "MN_A1Fake", "MN_A" }, Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_A1     = new PieceTypeDef{Id = 19000102, Abbreviations = new List<string>{ "MN_A1" },             Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_A2Fake = new PieceTypeDef{Id = 19000103, Abbreviations = new List<string>{ "MN_A2Fake" },         Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_A2     = new PieceTypeDef{Id = 19000104, Abbreviations = new List<string>{ "MN_A2" },             Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_A3Fake = new PieceTypeDef{Id = 19000105, Abbreviations = new List<string>{ "MN_A3Fake" },         Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_A3     = new PieceTypeDef{Id = 19000106, Abbreviations = new List<string>{ "MN_A3" },             Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_A4Fake = new PieceTypeDef{Id = 19000107, Abbreviations = new List<string>{ "MN_A4Fake" },         Branch = PieceTypeBranch.A, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef MN_B      = new PieceTypeDef{Id = 19000201, Abbreviations = new List<string>{ "MN_B1Fake", "MN_B" }, Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_B1     = new PieceTypeDef{Id = 19000202, Abbreviations = new List<string>{ "MN_B1" },             Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_B2Fake = new PieceTypeDef{Id = 19000203, Abbreviations = new List<string>{ "MN_B2Fake" },         Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_B2     = new PieceTypeDef{Id = 19000204, Abbreviations = new List<string>{ "MN_B2" },             Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_B3Fake = new PieceTypeDef{Id = 19000205, Abbreviations = new List<string>{ "MN_B3Fake" },         Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_B3     = new PieceTypeDef{Id = 19000206, Abbreviations = new List<string>{ "MN_B3" },             Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_B4Fake = new PieceTypeDef{Id = 19000207, Abbreviations = new List<string>{ "MN_B4Fake" },         Branch = PieceTypeBranch.B, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef MN_C      = new PieceTypeDef{Id = 19000301, Abbreviations = new List<string>{ "MN_C1Fake", "MN_C" }, Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_C1     = new PieceTypeDef{Id = 19000302, Abbreviations = new List<string>{ "MN_C1" },             Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_C2Fake = new PieceTypeDef{Id = 19000303, Abbreviations = new List<string>{ "MN_C2Fake" },         Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_C2     = new PieceTypeDef{Id = 19000304, Abbreviations = new List<string>{ "MN_C2" },             Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_C3Fake = new PieceTypeDef{Id = 19000305, Abbreviations = new List<string>{ "MN_C3Fake" },         Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_C3     = new PieceTypeDef{Id = 19000306, Abbreviations = new List<string>{ "MN_C3" },             Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_C4Fake = new PieceTypeDef{Id = 19000307, Abbreviations = new List<string>{ "MN_C4Fake" },         Branch = PieceTypeBranch.C, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef MN_D      = new PieceTypeDef{Id = 19000401, Abbreviations = new List<string>{ "MN_D1Fake", "MN_D" }, Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_D1     = new PieceTypeDef{Id = 19000402, Abbreviations = new List<string>{ "MN_D1" },             Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_D2Fake = new PieceTypeDef{Id = 19000403, Abbreviations = new List<string>{ "MN_D2Fake" },         Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_D2     = new PieceTypeDef{Id = 19000404, Abbreviations = new List<string>{ "MN_D2" },             Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_D3Fake = new PieceTypeDef{Id = 19000405, Abbreviations = new List<string>{ "MN_D3Fake" },         Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_D3     = new PieceTypeDef{Id = 19000406, Abbreviations = new List<string>{ "MN_D3" },             Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_D4Fake = new PieceTypeDef{Id = 19000407, Abbreviations = new List<string>{ "MN_D4Fake" },         Branch = PieceTypeBranch.D, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef MN_E      = new PieceTypeDef{Id = 19000501, Abbreviations = new List<string>{ "MN_E1Fake", "MN_E" }, Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_E1     = new PieceTypeDef{Id = 19000502, Abbreviations = new List<string>{ "MN_E1" },             Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_E2Fake = new PieceTypeDef{Id = 19000503, Abbreviations = new List<string>{ "MN_E2Fake" },         Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_E2     = new PieceTypeDef{Id = 19000504, Abbreviations = new List<string>{ "MN_E2" },             Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_E3Fake = new PieceTypeDef{Id = 19000505, Abbreviations = new List<string>{ "MN_E3Fake" },         Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_E3     = new PieceTypeDef{Id = 19000506, Abbreviations = new List<string>{ "MN_E3" },             Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_E4Fake = new PieceTypeDef{Id = 19000507, Abbreviations = new List<string>{ "MN_E4Fake" },         Branch = PieceTypeBranch.E, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef MN_F      = new PieceTypeDef{Id = 19000601, Abbreviations = new List<string>{ "MN_F1Fake", "MN_F" }, Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_F1     = new PieceTypeDef{Id = 19000602, Abbreviations = new List<string>{ "MN_F1" },             Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_F2Fake = new PieceTypeDef{Id = 19000603, Abbreviations = new List<string>{ "MN_F2Fake" },         Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_F2     = new PieceTypeDef{Id = 19000604, Abbreviations = new List<string>{ "MN_F2" },             Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_F3Fake = new PieceTypeDef{Id = 19000605, Abbreviations = new List<string>{ "MN_F3Fake" },         Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_F3     = new PieceTypeDef{Id = 19000606, Abbreviations = new List<string>{ "MN_F3" },             Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_F4Fake = new PieceTypeDef{Id = 19000607, Abbreviations = new List<string>{ "MN_F4Fake" },         Branch = PieceTypeBranch.F, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef MN_G      = new PieceTypeDef{Id = 19000701, Abbreviations = new List<string>{ "MN_G1Fake", "MN_G" }, Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_G1     = new PieceTypeDef{Id = 19000702, Abbreviations = new List<string>{ "MN_G1" },             Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_G2Fake = new PieceTypeDef{Id = 19000703, Abbreviations = new List<string>{ "MN_G2Fake" },         Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_G2     = new PieceTypeDef{Id = 19000704, Abbreviations = new List<string>{ "MN_G2" },             Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_G3Fake = new PieceTypeDef{Id = 19000705, Abbreviations = new List<string>{ "MN_G3Fake" },         Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_G3     = new PieceTypeDef{Id = 19000706, Abbreviations = new List<string>{ "MN_G3" },             Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_G4Fake = new PieceTypeDef{Id = 19000707, Abbreviations = new List<string>{ "MN_G4Fake" },         Branch = PieceTypeBranch.G, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef MN_H      = new PieceTypeDef{Id = 19000801, Abbreviations = new List<string>{ "MN_H1Fake", "MN_H" }, Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_H1     = new PieceTypeDef{Id = 19000802, Abbreviations = new List<string>{ "MN_H1" },             Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_H2Fake = new PieceTypeDef{Id = 19000803, Abbreviations = new List<string>{ "MN_H2Fake" },         Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_H2     = new PieceTypeDef{Id = 19000804, Abbreviations = new List<string>{ "MN_H2" },             Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_H3Fake = new PieceTypeDef{Id = 19000805, Abbreviations = new List<string>{ "MN_H3Fake" },         Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_H3     = new PieceTypeDef{Id = 19000806, Abbreviations = new List<string>{ "MN_H3" },             Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_H4Fake = new PieceTypeDef{Id = 19000807, Abbreviations = new List<string>{ "MN_H4Fake" },         Branch = PieceTypeBranch.H, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef MN_I      = new PieceTypeDef{Id = 19000901, Abbreviations = new List<string>{ "MN_I1Fake", "MN_I" }, Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_I1     = new PieceTypeDef{Id = 19000902, Abbreviations = new List<string>{ "MN_I1" },             Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_I2Fake = new PieceTypeDef{Id = 19000903, Abbreviations = new List<string>{ "MN_I2Fake" },         Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_I2     = new PieceTypeDef{Id = 19000904, Abbreviations = new List<string>{ "MN_I2" },             Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_I3Fake = new PieceTypeDef{Id = 19000905, Abbreviations = new List<string>{ "MN_I3Fake" },         Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef MN_I3     = new PieceTypeDef{Id = 19000906, Abbreviations = new List<string>{ "MN_I3" },             Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine};
    public static readonly PieceTypeDef MN_I4Fake = new PieceTypeDef{Id = 19000907, Abbreviations = new List<string>{ "MN_I4Fake" },         Branch = PieceTypeBranch.I, Filter = PieceTypeFilter.Multicellular | PieceTypeFilter.Mine | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    
#endregion
    
#region Extended Pieces

    #region EXT_A
    
    public static readonly PieceTypeDef EXT_A1     = new PieceTypeDef{Id = 21000101, Abbreviations = new List<string>{ "EXT_A1" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_A2     = new PieceTypeDef{Id = 21000102, Abbreviations = new List<string>{ "EXT_A2" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_A3Fake = new PieceTypeDef{Id = 21000103, Abbreviations = new List<string>{ "EXT_A3Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_A3     = new PieceTypeDef{Id = 21000104, Abbreviations = new List<string>{ "EXT_A3" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_A4Fake = new PieceTypeDef{Id = 21000105, Abbreviations = new List<string>{ "EXT_A4Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_A4     = new PieceTypeDef{Id = 21000106, Abbreviations = new List<string>{ "EXT_A4" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_A5Fake = new PieceTypeDef{Id = 21000107, Abbreviations = new List<string>{ "EXT_A5Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_A5     = new PieceTypeDef{Id = 21000108, Abbreviations = new List<string>{ "EXT_A5" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_A6Fake = new PieceTypeDef{Id = 21000109, Abbreviations = new List<string>{ "EXT_A6Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_A6     = new PieceTypeDef{Id = 21000110, Abbreviations = new List<string>{ "EXT_A6" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_A7Fake = new PieceTypeDef{Id = 21000111, Abbreviations = new List<string>{ "EXT_A7Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_A7     = new PieceTypeDef{Id = 21000112, Abbreviations = new List<string>{ "EXT_A7" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_A8Fake = new PieceTypeDef{Id = 21000113, Abbreviations = new List<string>{ "EXT_A8Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_A8     = new PieceTypeDef{Id = 21000114, Abbreviations = new List<string>{ "EXT_A8" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef EXT_AMFake = new PieceTypeDef{Id = 21000190, Abbreviations = new List<string>{ "EXT_AMFake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_AM     = new PieceTypeDef{Id = 21000191, Abbreviations = new List<string>{ "EXT_AM" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular};
    
    #endregion

    #region EXT_B
    
    public static readonly PieceTypeDef EXT_B1     = new PieceTypeDef{Id = 21000201, Abbreviations = new List<string>{ "EXT_B1" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_B2     = new PieceTypeDef{Id = 21000202, Abbreviations = new List<string>{ "EXT_B2" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_B3Fake = new PieceTypeDef{Id = 21000203, Abbreviations = new List<string>{ "EXT_B3Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_B3     = new PieceTypeDef{Id = 21000204, Abbreviations = new List<string>{ "EXT_B3" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_B4Fake = new PieceTypeDef{Id = 21000205, Abbreviations = new List<string>{ "EXT_B4Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_B4     = new PieceTypeDef{Id = 21000206, Abbreviations = new List<string>{ "EXT_B4" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_B5Fake = new PieceTypeDef{Id = 21000207, Abbreviations = new List<string>{ "EXT_B5Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_B5     = new PieceTypeDef{Id = 21000208, Abbreviations = new List<string>{ "EXT_B5" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_B6Fake = new PieceTypeDef{Id = 21000209, Abbreviations = new List<string>{ "EXT_B6Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_B6     = new PieceTypeDef{Id = 21000210, Abbreviations = new List<string>{ "EXT_B6" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_B7Fake = new PieceTypeDef{Id = 21000211, Abbreviations = new List<string>{ "EXT_B7Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_B7     = new PieceTypeDef{Id = 21000212, Abbreviations = new List<string>{ "EXT_B7" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_B8Fake = new PieceTypeDef{Id = 21000213, Abbreviations = new List<string>{ "EXT_B8Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_B8     = new PieceTypeDef{Id = 21000214, Abbreviations = new List<string>{ "EXT_B8" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef EXT_BMFake = new PieceTypeDef{Id = 21000290, Abbreviations = new List<string>{ "EXT_BMFake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_BM     = new PieceTypeDef{Id = 21000291, Abbreviations = new List<string>{ "EXT_BM" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular};
    
    #endregion

    #region EXT_C
    
    public static readonly PieceTypeDef EXT_C1     = new PieceTypeDef{Id = 21000301, Abbreviations = new List<string>{ "EXT_C1" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_C2     = new PieceTypeDef{Id = 21000302, Abbreviations = new List<string>{ "EXT_C2" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_C3Fake = new PieceTypeDef{Id = 21000303, Abbreviations = new List<string>{ "EXT_C3Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_C3     = new PieceTypeDef{Id = 21000304, Abbreviations = new List<string>{ "EXT_C3" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_C4Fake = new PieceTypeDef{Id = 21000305, Abbreviations = new List<string>{ "EXT_C4Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_C4     = new PieceTypeDef{Id = 21000306, Abbreviations = new List<string>{ "EXT_C4" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_C5Fake = new PieceTypeDef{Id = 21000307, Abbreviations = new List<string>{ "EXT_C5Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_C5     = new PieceTypeDef{Id = 21000308, Abbreviations = new List<string>{ "EXT_C5" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_C6Fake = new PieceTypeDef{Id = 21000309, Abbreviations = new List<string>{ "EXT_C6Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_C6     = new PieceTypeDef{Id = 21000310, Abbreviations = new List<string>{ "EXT_C6" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_C7Fake = new PieceTypeDef{Id = 21000311, Abbreviations = new List<string>{ "EXT_C7Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_C7     = new PieceTypeDef{Id = 21000312, Abbreviations = new List<string>{ "EXT_C7" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_C8Fake = new PieceTypeDef{Id = 21000313, Abbreviations = new List<string>{ "EXT_C8Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_C8     = new PieceTypeDef{Id = 21000314, Abbreviations = new List<string>{ "EXT_C8" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef EXT_CMFake = new PieceTypeDef{Id = 21000390, Abbreviations = new List<string>{ "EXT_CMFake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_CM     = new PieceTypeDef{Id = 21000391, Abbreviations = new List<string>{ "EXT_CM" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular};
    
    #endregion

    #region EXT_D
    
    public static readonly PieceTypeDef EXT_D1     = new PieceTypeDef{Id = 21000401, Abbreviations = new List<string>{ "EXT_D1" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_D2     = new PieceTypeDef{Id = 21000402, Abbreviations = new List<string>{ "EXT_D2" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_D3Fake = new PieceTypeDef{Id = 21000403, Abbreviations = new List<string>{ "EXT_D3Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_D3     = new PieceTypeDef{Id = 21000404, Abbreviations = new List<string>{ "EXT_D3" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_D4Fake = new PieceTypeDef{Id = 21000405, Abbreviations = new List<string>{ "EXT_D4Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_D4     = new PieceTypeDef{Id = 21000406, Abbreviations = new List<string>{ "EXT_D4" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_D5Fake = new PieceTypeDef{Id = 21000407, Abbreviations = new List<string>{ "EXT_D5Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_D5     = new PieceTypeDef{Id = 21000408, Abbreviations = new List<string>{ "EXT_D5" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_D6Fake = new PieceTypeDef{Id = 21000409, Abbreviations = new List<string>{ "EXT_D6Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_D6     = new PieceTypeDef{Id = 21000410, Abbreviations = new List<string>{ "EXT_D6" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_D7Fake = new PieceTypeDef{Id = 21000411, Abbreviations = new List<string>{ "EXT_D7Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_D7     = new PieceTypeDef{Id = 21000412, Abbreviations = new List<string>{ "EXT_D7" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_D8Fake = new PieceTypeDef{Id = 21000413, Abbreviations = new List<string>{ "EXT_D8Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_D8     = new PieceTypeDef{Id = 21000414, Abbreviations = new List<string>{ "EXT_D8" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef EXT_DMFake = new PieceTypeDef{Id = 21000490, Abbreviations = new List<string>{ "EXT_DMFake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_DM     = new PieceTypeDef{Id = 21000491, Abbreviations = new List<string>{ "EXT_DM" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular};
    
    #endregion

    #region EXT_E
    
    public static readonly PieceTypeDef EXT_E1     = new PieceTypeDef{Id = 21000501, Abbreviations = new List<string>{ "EXT_E1" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_E2     = new PieceTypeDef{Id = 21000502, Abbreviations = new List<string>{ "EXT_E2" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Removable};
    public static readonly PieceTypeDef EXT_E3Fake = new PieceTypeDef{Id = 21000503, Abbreviations = new List<string>{ "EXT_E3Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_E3     = new PieceTypeDef{Id = 21000504, Abbreviations = new List<string>{ "EXT_E3" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_E4Fake = new PieceTypeDef{Id = 21000505, Abbreviations = new List<string>{ "EXT_E4Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_E4     = new PieceTypeDef{Id = 21000506, Abbreviations = new List<string>{ "EXT_E4" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_E5Fake = new PieceTypeDef{Id = 21000507, Abbreviations = new List<string>{ "EXT_E5Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_E5     = new PieceTypeDef{Id = 21000508, Abbreviations = new List<string>{ "EXT_E5" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_E6Fake = new PieceTypeDef{Id = 21000509, Abbreviations = new List<string>{ "EXT_E6Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_E6     = new PieceTypeDef{Id = 21000510, Abbreviations = new List<string>{ "EXT_E6" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_E7Fake = new PieceTypeDef{Id = 21000511, Abbreviations = new List<string>{ "EXT_E7Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_E7     = new PieceTypeDef{Id = 21000512, Abbreviations = new List<string>{ "EXT_E7" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple};
    public static readonly PieceTypeDef EXT_E8Fake = new PieceTypeDef{Id = 21000513, Abbreviations = new List<string>{ "EXT_E8Fake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_E8     = new PieceTypeDef{Id = 21000514, Abbreviations = new List<string>{ "EXT_E8" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Simple | PieceTypeFilter.Workplace};
    
    public static readonly PieceTypeDef EXT_EMFake = new PieceTypeDef{Id = 21000590, Abbreviations = new List<string>{ "EXT_EMFake" }, Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular | PieceTypeFilter.Fake | PieceTypeFilter.Workplace};
    public static readonly PieceTypeDef EXT_EM     = new PieceTypeDef{Id = 21000591, Abbreviations = new List<string>{ "EXT_EM" },     Filter = PieceTypeFilter.Analytics | PieceTypeFilter.Multicellular};
    
    #endregion
    
    public static readonly PieceTypeDef EXT_ZM     = new PieceTypeDef{Id = 21002691, Abbreviations = new List<string>{ "EXT_ZM" },     Filter = PieceTypeFilter.Multicellular};
    
#endregion

#region Special

    public static readonly PieceTypeDef Fog = new PieceTypeDef{Id = 98000001, Abbreviations = new List<string>{ "Fog" }, Filter = PieceTypeFilter.Multicellular};

    public static readonly PieceTypeDef LockedEmpty = new PieceTypeDef{Id = 99000101, Abbreviations = new List<string>{ "LockedEmpty" }, Filter = PieceTypeFilter.Simple};
    public static readonly PieceTypeDef Enemy1 = new PieceTypeDef{Id = 99009900, Abbreviations = new List<string>{ "Enemy1" }, Filter = PieceTypeFilter.Simple | PieceTypeFilter.Enemy};
    
#endregion
}