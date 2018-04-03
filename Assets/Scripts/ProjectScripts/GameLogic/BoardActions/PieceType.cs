using System.Collections.Generic;

public static partial class PieceType 
{
    static PieceType()
    {
        RegisterType(A1);
        RegisterType(A2);
        RegisterType(A3);
        RegisterType(A4);
        RegisterType(A5);
        RegisterType(A6);
        RegisterType(A7);
        RegisterType(A8);
        
        RegisterType(B1);
        RegisterType(B2);
        RegisterType(B3);
        RegisterType(B4);
        RegisterType(B5);
        
        RegisterType(C1);
        RegisterType(C2);
        RegisterType(C3);
        RegisterType(C4);
        RegisterType(C5);
        
        RegisterType(E1);
        RegisterType(E2);
        RegisterType(E3);
        
        RegisterType(O1);
        RegisterType(O2);
        RegisterType(O3);
        RegisterType(O4);
        RegisterType(Fog);
        
        RegisterType(Mine1);
        RegisterType(Mine2);
        RegisterType(Mine3);
        RegisterType(Mine4);
        RegisterType(Mine5);
        RegisterType(Mine6);
        RegisterType(Mine7);
        
        RegisterType(Sawmill1);
        RegisterType(Sawmill2);
        RegisterType(Sawmill3);
        RegisterType(Sawmill4);
        RegisterType(Sawmill5);
        RegisterType(Sawmill6);
        RegisterType(Sawmill7);
        
        RegisterType(Sheepfold1);
        RegisterType(Sheepfold2);
        RegisterType(Sheepfold3);
        RegisterType(Sheepfold4);
        RegisterType(Sheepfold5);
        RegisterType(Sheepfold6);
        RegisterType(Sheepfold7);
        
        RegisterType(Gbox1);
        
        RegisterType(Chest1);
        RegisterType(Chest2);
        RegisterType(Chest3);
        
        RegisterType(Castle1);
        RegisterType(Castle2);
        RegisterType(Castle3);
        RegisterType(Castle4);
        RegisterType(Castle5);
        RegisterType(Castle6);
        RegisterType(Castle7);
        RegisterType(Castle8);
        RegisterType(Castle9);
        
        RegisterType(Tavern1);
        RegisterType(Tavern2);
        RegisterType(Tavern3);
        RegisterType(Tavern4);
        RegisterType(Tavern5);
        RegisterType(Tavern6);
        RegisterType(Tavern7);
        RegisterType(Tavern8);
        RegisterType(Tavern9);
    }
    
    public static readonly PieceTypeDef A1 = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "A1", "PieceA1" }};
    public static readonly PieceTypeDef A2 = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "A2", "PieceA2" }};
    public static readonly PieceTypeDef A3 = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "A3", "PieceA3" }};
    public static readonly PieceTypeDef A4 = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "A4", "PieceA4" }};
    public static readonly PieceTypeDef A5 = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "A5", "PieceA5" }};
    public static readonly PieceTypeDef A6 = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "A6", "PieceA6" }};
    public static readonly PieceTypeDef A7 = new PieceTypeDef{Id = 106, Abbreviations = new List<string>{ "A7", "PieceA7" }};
    public static readonly PieceTypeDef A8 = new PieceTypeDef{Id = 107, Abbreviations = new List<string>{ "A8", "PieceA8" }};
    
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
    
    public static readonly PieceTypeDef E1 = new PieceTypeDef{Id = 400, Abbreviations = new List<string>{ "E1" }};
    public static readonly PieceTypeDef E2 = new PieceTypeDef{Id = 401, Abbreviations = new List<string>{ "E2" }};
    public static readonly PieceTypeDef E3 = new PieceTypeDef{Id = 402, Abbreviations = new List<string>{ "E3" }};
    
    public static readonly PieceTypeDef O1 = new PieceTypeDef{Id = 500, Abbreviations = new List<string>{ "O1" }};
    public static readonly PieceTypeDef O2 = new PieceTypeDef{Id = 501, Abbreviations = new List<string>{ "O2" }};
    public static readonly PieceTypeDef O3 = new PieceTypeDef{Id = 502, Abbreviations = new List<string>{ "O3" }};
    public static readonly PieceTypeDef O4 = new PieceTypeDef{Id = 503, Abbreviations = new List<string>{ "O4" }};
    
    public static readonly PieceTypeDef Fog = new PieceTypeDef{Id = 505, Abbreviations = new List<string>{ "Fog" }};
    
    public static readonly PieceTypeDef Mine1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "Mine1" }};
    public static readonly PieceTypeDef Mine2 = new PieceTypeDef{Id = 1001, Abbreviations = new List<string>{ "Mine2" }};
    public static readonly PieceTypeDef Mine3 = new PieceTypeDef{Id = 1002, Abbreviations = new List<string>{ "Mine3" }};
    public static readonly PieceTypeDef Mine4 = new PieceTypeDef{Id = 1003, Abbreviations = new List<string>{ "Mine4" }};
    public static readonly PieceTypeDef Mine5 = new PieceTypeDef{Id = 1004, Abbreviations = new List<string>{ "Mine5" }};
    public static readonly PieceTypeDef Mine6 = new PieceTypeDef{Id = 1005, Abbreviations = new List<string>{ "Mine6" }};
    public static readonly PieceTypeDef Mine7 = new PieceTypeDef{Id = 1006, Abbreviations = new List<string>{ "Mine7" }};
    
    public static readonly PieceTypeDef Sawmill1 = new PieceTypeDef{Id = 1100, Abbreviations = new List<string>{ "Sawmill1" }};
    public static readonly PieceTypeDef Sawmill2 = new PieceTypeDef{Id = 1101, Abbreviations = new List<string>{ "Sawmill2" }};
    public static readonly PieceTypeDef Sawmill3 = new PieceTypeDef{Id = 1102, Abbreviations = new List<string>{ "Sawmill3" }};
    public static readonly PieceTypeDef Sawmill4 = new PieceTypeDef{Id = 1103, Abbreviations = new List<string>{ "Sawmill4" }};
    public static readonly PieceTypeDef Sawmill5 = new PieceTypeDef{Id = 1104, Abbreviations = new List<string>{ "Sawmill5" }};
    public static readonly PieceTypeDef Sawmill6 = new PieceTypeDef{Id = 1105, Abbreviations = new List<string>{ "Sawmill6" }};
    public static readonly PieceTypeDef Sawmill7 = new PieceTypeDef{Id = 1106, Abbreviations = new List<string>{ "Sawmill7" }};
    
    public static readonly PieceTypeDef Sheepfold1 = new PieceTypeDef{Id = 1200, Abbreviations = new List<string>{ "Sheepfold1" }};
    public static readonly PieceTypeDef Sheepfold2 = new PieceTypeDef{Id = 1201, Abbreviations = new List<string>{ "Sheepfold2" }};
    public static readonly PieceTypeDef Sheepfold3 = new PieceTypeDef{Id = 1202, Abbreviations = new List<string>{ "Sheepfold3" }};
    public static readonly PieceTypeDef Sheepfold4 = new PieceTypeDef{Id = 1203, Abbreviations = new List<string>{ "Sheepfold4" }};
    public static readonly PieceTypeDef Sheepfold5 = new PieceTypeDef{Id = 1204, Abbreviations = new List<string>{ "Sheepfold5" }};
    public static readonly PieceTypeDef Sheepfold6 = new PieceTypeDef{Id = 1205, Abbreviations = new List<string>{ "Sheepfold6" }};
    public static readonly PieceTypeDef Sheepfold7 = new PieceTypeDef{Id = 1206, Abbreviations = new List<string>{ "Sheepfold7" }};
    
    public static readonly PieceTypeDef Gbox1 = new PieceTypeDef{Id = 1300, Abbreviations = new List<string>{ "GBOX1" }};
    
    public static readonly PieceTypeDef Chest1 = new PieceTypeDef{Id = 1301, Abbreviations = new List<string>{ "ChestCommon", "Chest1", "Common" }};
    public static readonly PieceTypeDef Chest2 = new PieceTypeDef{Id = 1302, Abbreviations = new List<string>{ "ChestRare", "Chest2", "Rare" }};
    public static readonly PieceTypeDef Chest3 = new PieceTypeDef{Id = 1303, Abbreviations = new List<string>{ "ChestEpic", "Chest3", "Epic" }};
    
    public static readonly PieceTypeDef Castle1 = new PieceTypeDef{Id = 1400, Abbreviations = new List<string>{ "Castle1" }};
    public static readonly PieceTypeDef Castle2 = new PieceTypeDef{Id = 1401, Abbreviations = new List<string>{ "Castle2" }};
    public static readonly PieceTypeDef Castle3 = new PieceTypeDef{Id = 1402, Abbreviations = new List<string>{ "Castle3" }};
    public static readonly PieceTypeDef Castle4 = new PieceTypeDef{Id = 1403, Abbreviations = new List<string>{ "Castle4" }};
    public static readonly PieceTypeDef Castle5 = new PieceTypeDef{Id = 1404, Abbreviations = new List<string>{ "Castle5" }};
    public static readonly PieceTypeDef Castle6 = new PieceTypeDef{Id = 1405, Abbreviations = new List<string>{ "Castle6" }};
    public static readonly PieceTypeDef Castle7 = new PieceTypeDef{Id = 1406, Abbreviations = new List<string>{ "Castle7" }};
    public static readonly PieceTypeDef Castle8 = new PieceTypeDef{Id = 1407, Abbreviations = new List<string>{ "Castle8" }};
    public static readonly PieceTypeDef Castle9 = new PieceTypeDef{Id = 1408, Abbreviations = new List<string>{ "Castle9" }};
    
    public static readonly PieceTypeDef Tavern1 = new PieceTypeDef{Id = 1500, Abbreviations = new List<string>{ "Tavern1" }};
    public static readonly PieceTypeDef Tavern2 = new PieceTypeDef{Id = 1501, Abbreviations = new List<string>{ "Tavern2" }};
    public static readonly PieceTypeDef Tavern3 = new PieceTypeDef{Id = 1502, Abbreviations = new List<string>{ "Tavern3" }};
    public static readonly PieceTypeDef Tavern4 = new PieceTypeDef{Id = 1503, Abbreviations = new List<string>{ "Tavern4" }};
    public static readonly PieceTypeDef Tavern5 = new PieceTypeDef{Id = 1504, Abbreviations = new List<string>{ "Tavern5" }};
    public static readonly PieceTypeDef Tavern6 = new PieceTypeDef{Id = 1505, Abbreviations = new List<string>{ "Tavern6" }};
    public static readonly PieceTypeDef Tavern7 = new PieceTypeDef{Id = 1506, Abbreviations = new List<string>{ "Tavern7" }};
    public static readonly PieceTypeDef Tavern8 = new PieceTypeDef{Id = 1507, Abbreviations = new List<string>{ "Tavern8" }};
    public static readonly PieceTypeDef Tavern9 = new PieceTypeDef{Id = 1508, Abbreviations = new List<string>{ "Tavern9" }};
}