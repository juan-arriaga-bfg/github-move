using System.Collections.Generic;
using System.Reflection;

public static partial class PieceType 
{
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
        }
    }
    
    public static readonly PieceTypeDef A1 = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "A1", "PieceA1" }};
    public static readonly PieceTypeDef A2 = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "A2", "PieceA2" }};
    public static readonly PieceTypeDef A3 = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "A3", "PieceA3" }};
    public static readonly PieceTypeDef A4 = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "A4", "PieceA4" }};
    public static readonly PieceTypeDef A5 = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "A5", "PieceA5" }};
    public static readonly PieceTypeDef A6 = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "A6", "PieceA6" }};
    public static readonly PieceTypeDef A7 = new PieceTypeDef{Id = 106, Abbreviations = new List<string>{ "A7", "PieceA7" }};
    public static readonly PieceTypeDef A8 = new PieceTypeDef{Id = 107, Abbreviations = new List<string>{ "A8", "PieceA8" }};
    public static readonly PieceTypeDef A9 = new PieceTypeDef{Id = 108, Abbreviations = new List<string>{ "A9", "PieceA9" }};
    
    public static readonly PieceTypeDef B1 = new PieceTypeDef{Id = 200, Abbreviations = new List<string>{ "B1", "PieceB1" }};
    public static readonly PieceTypeDef B2 = new PieceTypeDef{Id = 201, Abbreviations = new List<string>{ "B2", "PieceB2" }};
    public static readonly PieceTypeDef B3 = new PieceTypeDef{Id = 202, Abbreviations = new List<string>{ "B3", "PieceB3" }};
    public static readonly PieceTypeDef B4 = new PieceTypeDef{Id = 203, Abbreviations = new List<string>{ "B4", "PieceB4" }};
    public static readonly PieceTypeDef B5 = new PieceTypeDef{Id = 204, Abbreviations = new List<string>{ "B5", "PieceB5" }};
    
    public static readonly PieceTypeDef C1 = new PieceTypeDef{Id = 300, Abbreviations = new List<string>{ "C1", "PieceC1" }};
    public static readonly PieceTypeDef C2 = new PieceTypeDef{Id = 301, Abbreviations = new List<string>{ "C2", "PieceC2" }};
    public static readonly PieceTypeDef C3 = new PieceTypeDef{Id = 302, Abbreviations = new List<string>{ "C3", "PieceC3" }};
    public static readonly PieceTypeDef C4 = new PieceTypeDef{Id = 303, Abbreviations = new List<string>{ "C4", "PieceC4" }};
    public static readonly PieceTypeDef C5 = new PieceTypeDef{Id = 304, Abbreviations = new List<string>{ "C5", "PieceC5" }};
    public static readonly PieceTypeDef C6 = new PieceTypeDef{Id = 305, Abbreviations = new List<string>{ "C6", "PieceC6" }};
    public static readonly PieceTypeDef C7 = new PieceTypeDef{Id = 306, Abbreviations = new List<string>{ "C7", "PieceC7" }};
    public static readonly PieceTypeDef C8 = new PieceTypeDef{Id = 307, Abbreviations = new List<string>{ "C8", "PieceC8" }};
    public static readonly PieceTypeDef C9 = new PieceTypeDef{Id = 308, Abbreviations = new List<string>{ "C9", "PieceC9" }};
    
    public static readonly PieceTypeDef D1 = new PieceTypeDef{Id = 400, Abbreviations = new List<string>{ "D1", "PieceD1" }};
    public static readonly PieceTypeDef D2 = new PieceTypeDef{Id = 401, Abbreviations = new List<string>{ "D2", "PieceD2" }};
    public static readonly PieceTypeDef D3 = new PieceTypeDef{Id = 402, Abbreviations = new List<string>{ "D3", "PieceD3" }};
    public static readonly PieceTypeDef D4 = new PieceTypeDef{Id = 403, Abbreviations = new List<string>{ "D4", "PieceD4" }};
    public static readonly PieceTypeDef D5 = new PieceTypeDef{Id = 404, Abbreviations = new List<string>{ "D5", "PieceD5" }};
    
    public static readonly PieceTypeDef E1 = new PieceTypeDef{Id = 500, Abbreviations = new List<string>{ "E1", "PieceE1" }};
    public static readonly PieceTypeDef E2 = new PieceTypeDef{Id = 501, Abbreviations = new List<string>{ "E2", "PieceE2" }};
    public static readonly PieceTypeDef E3 = new PieceTypeDef{Id = 502, Abbreviations = new List<string>{ "E3", "PieceE3" }};
    public static readonly PieceTypeDef E4 = new PieceTypeDef{Id = 503, Abbreviations = new List<string>{ "E4", "PieceE4" }};
    public static readonly PieceTypeDef E5 = new PieceTypeDef{Id = 504, Abbreviations = new List<string>{ "E5", "PieceE5" }};
    public static readonly PieceTypeDef E6 = new PieceTypeDef{Id = 505, Abbreviations = new List<string>{ "E6", "PieceE6" }};
    
    public static readonly PieceTypeDef F1 = new PieceTypeDef{Id = 600, Abbreviations = new List<string>{ "F1", "PieceF1" }};
    public static readonly PieceTypeDef F2 = new PieceTypeDef{Id = 601, Abbreviations = new List<string>{ "F2", "PieceF2" }};
    public static readonly PieceTypeDef F3 = new PieceTypeDef{Id = 602, Abbreviations = new List<string>{ "F3", "PieceF3" }};
    public static readonly PieceTypeDef F4 = new PieceTypeDef{Id = 603, Abbreviations = new List<string>{ "F4", "PieceF4" }};
    public static readonly PieceTypeDef F5 = new PieceTypeDef{Id = 604, Abbreviations = new List<string>{ "F5", "PieceF5" }};
    
    public static readonly PieceTypeDef G1 = new PieceTypeDef{Id = 700, Abbreviations = new List<string>{ "G1", "PieceG1" }};
    public static readonly PieceTypeDef G2 = new PieceTypeDef{Id = 701, Abbreviations = new List<string>{ "G2", "PieceG2" }};
    public static readonly PieceTypeDef G3 = new PieceTypeDef{Id = 702, Abbreviations = new List<string>{ "G3", "PieceG3" }};
    public static readonly PieceTypeDef G4 = new PieceTypeDef{Id = 703, Abbreviations = new List<string>{ "G4", "PieceG4" }};
    
    public static readonly PieceTypeDef H1 = new PieceTypeDef{Id = 800, Abbreviations = new List<string>{ "H1", "PieceH1" }};
    public static readonly PieceTypeDef H2 = new PieceTypeDef{Id = 801, Abbreviations = new List<string>{ "H2", "PieceH2" }};
    public static readonly PieceTypeDef H3 = new PieceTypeDef{Id = 802, Abbreviations = new List<string>{ "H3", "PieceH3" }};
    public static readonly PieceTypeDef H4 = new PieceTypeDef{Id = 803, Abbreviations = new List<string>{ "H4", "PieceH4" }};
    
    public static readonly PieceTypeDef I1 = new PieceTypeDef{Id = 900, Abbreviations = new List<string>{ "I1", "PieceI1" }};
    public static readonly PieceTypeDef I2 = new PieceTypeDef{Id = 901, Abbreviations = new List<string>{ "I2", "PieceI2" }};
    public static readonly PieceTypeDef I3 = new PieceTypeDef{Id = 902, Abbreviations = new List<string>{ "I3", "PieceI3" }};
    public static readonly PieceTypeDef I4 = new PieceTypeDef{Id = 903, Abbreviations = new List<string>{ "I4", "PieceI4" }};
    public static readonly PieceTypeDef I5 = new PieceTypeDef{Id = 904, Abbreviations = new List<string>{ "I5", "PieceI5" }};
    
    public static readonly PieceTypeDef O1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "O1" }};
    public static readonly PieceTypeDef O2 = new PieceTypeDef{Id = 1001, Abbreviations = new List<string>{ "O2" }};
    public static readonly PieceTypeDef O3 = new PieceTypeDef{Id = 1002, Abbreviations = new List<string>{ "O3" }};
    public static readonly PieceTypeDef O4 = new PieceTypeDef{Id = 1003, Abbreviations = new List<string>{ "O4" }};
    public static readonly PieceTypeDef O5 = new PieceTypeDef{Id = 1004, Abbreviations = new List<string>{ "O5" }};
    
    public static readonly PieceTypeDef OX1 = new PieceTypeDef{Id = 1100, Abbreviations = new List<string>{ "OX1" }};
    public static readonly PieceTypeDef OX2 = new PieceTypeDef{Id = 1101, Abbreviations = new List<string>{ "OX2" }};
    public static readonly PieceTypeDef OX3 = new PieceTypeDef{Id = 1102, Abbreviations = new List<string>{ "OX3" }};
    public static readonly PieceTypeDef OX4 = new PieceTypeDef{Id = 1103, Abbreviations = new List<string>{ "OX4" }};
    public static readonly PieceTypeDef OX5 = new PieceTypeDef{Id = 1104, Abbreviations = new List<string>{ "OX5" }};
    
    public static readonly PieceTypeDef Fog = new PieceTypeDef{Id = 1201, Abbreviations = new List<string>{ "Fog" }};
    
    public static readonly PieceTypeDef King = new PieceTypeDef{Id = 1301, Abbreviations = new List<string>{ "King" }};
    
    public static readonly PieceTypeDef Coin1 = new PieceTypeDef{Id = 1400, Abbreviations = new List<string>{ "Coin1" }};
    public static readonly PieceTypeDef Coin2 = new PieceTypeDef{Id = 1401, Abbreviations = new List<string>{ "Coin2" }};
    public static readonly PieceTypeDef Coin3 = new PieceTypeDef{Id = 1402, Abbreviations = new List<string>{ "Coin3" }}; 
    public static readonly PieceTypeDef Coin4 = new PieceTypeDef{Id = 1403, Abbreviations = new List<string>{ "Coin4" }};
    public static readonly PieceTypeDef Coin5 = new PieceTypeDef{Id = 1404, Abbreviations = new List<string>{ "Coin5" }};
    
    public static readonly PieceTypeDef Mine1 = new PieceTypeDef{Id = 10000, Abbreviations = new List<string>{ "Mine1" }};
    public static readonly PieceTypeDef Mine2 = new PieceTypeDef{Id = 10001, Abbreviations = new List<string>{ "Mine2" }};
    public static readonly PieceTypeDef Mine3 = new PieceTypeDef{Id = 10002, Abbreviations = new List<string>{ "Mine3" }};
    public static readonly PieceTypeDef Mine4 = new PieceTypeDef{Id = 10003, Abbreviations = new List<string>{ "Mine4" }};
    public static readonly PieceTypeDef Mine5 = new PieceTypeDef{Id = 10004, Abbreviations = new List<string>{ "Mine5" }};
    public static readonly PieceTypeDef Mine6 = new PieceTypeDef{Id = 10005, Abbreviations = new List<string>{ "Mine6" }};
    public static readonly PieceTypeDef Mine7 = new PieceTypeDef{Id = 10006, Abbreviations = new List<string>{ "Mine7" }};
    
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
    
    public static readonly PieceTypeDef Chest1 = new PieceTypeDef{Id = 13001, Abbreviations = new List<string>{ "Chest1" }};
    public static readonly PieceTypeDef Chest2 = new PieceTypeDef{Id = 13002, Abbreviations = new List<string>{ "Chest2" }};
    public static readonly PieceTypeDef Chest3 = new PieceTypeDef{Id = 13003, Abbreviations = new List<string>{ "Chest3" }};
    public static readonly PieceTypeDef Chest4 = new PieceTypeDef{Id = 13004, Abbreviations = new List<string>{ "Chest4" }};
    public static readonly PieceTypeDef Chest5 = new PieceTypeDef{Id = 13005, Abbreviations = new List<string>{ "Chest5" }};
    public static readonly PieceTypeDef Chest6 = new PieceTypeDef{Id = 13006, Abbreviations = new List<string>{ "Chest6" }};
    public static readonly PieceTypeDef Chest7 = new PieceTypeDef{Id = 13007, Abbreviations = new List<string>{ "Chest7" }};
    public static readonly PieceTypeDef Chest8 = new PieceTypeDef{Id = 13008, Abbreviations = new List<string>{ "Chest8" }};
    public static readonly PieceTypeDef Chest9 = new PieceTypeDef{Id = 13009, Abbreviations = new List<string>{ "Chest9" }};
    
    public static readonly PieceTypeDef Castle1 = new PieceTypeDef{Id = 14000, Abbreviations = new List<string>{ "Castle1" }};
    public static readonly PieceTypeDef Castle2 = new PieceTypeDef{Id = 14001, Abbreviations = new List<string>{ "Castle2" }};
    public static readonly PieceTypeDef Castle3 = new PieceTypeDef{Id = 14002, Abbreviations = new List<string>{ "Castle3" }};
    public static readonly PieceTypeDef Castle4 = new PieceTypeDef{Id = 14003, Abbreviations = new List<string>{ "Castle4" }};
    public static readonly PieceTypeDef Castle5 = new PieceTypeDef{Id = 14004, Abbreviations = new List<string>{ "Castle5" }};
    public static readonly PieceTypeDef Castle6 = new PieceTypeDef{Id = 14005, Abbreviations = new List<string>{ "Castle6" }};
    public static readonly PieceTypeDef Castle7 = new PieceTypeDef{Id = 14006, Abbreviations = new List<string>{ "Castle7" }};
    public static readonly PieceTypeDef Castle8 = new PieceTypeDef{Id = 14007, Abbreviations = new List<string>{ "Castle8" }};
    public static readonly PieceTypeDef Castle9 = new PieceTypeDef{Id = 14008, Abbreviations = new List<string>{ "Castle9" }};
    
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
}