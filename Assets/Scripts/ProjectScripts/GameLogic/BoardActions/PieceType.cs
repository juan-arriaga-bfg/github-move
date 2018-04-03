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
    
    public readonly static PieceTypeDef A1 = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "A1", "PieceA1" }};
    public readonly static PieceTypeDef A2 = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "A2", "PieceA2" }};
    public readonly static PieceTypeDef A3 = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "A3", "PieceA3" }};
    public readonly static PieceTypeDef A4 = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "A4", "PieceA4" }};
    public readonly static PieceTypeDef A5 = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "A5", "PieceA5" }};
    public readonly static PieceTypeDef A6 = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "A6", "PieceA6" }};
    public readonly static PieceTypeDef A7 = new PieceTypeDef{Id = 106, Abbreviations = new List<string>{ "A7", "PieceA7" }};
    public readonly static PieceTypeDef A8 = new PieceTypeDef{Id = 107, Abbreviations = new List<string>{ "A8", "PieceA8" }};
    
    public readonly static PieceTypeDef B1 = new PieceTypeDef{Id = 200, Abbreviations = new List<string>{ "B1", "PieceB1" }};
    public readonly static PieceTypeDef B2 = new PieceTypeDef{Id = 201, Abbreviations = new List<string>{ "B2", "PieceB2" }};
    public readonly static PieceTypeDef B3 = new PieceTypeDef{Id = 202, Abbreviations = new List<string>{ "B3", "PieceB3" }};
    public readonly static PieceTypeDef B4 = new PieceTypeDef{Id = 203, Abbreviations = new List<string>{ "B4", "PieceB4" }};
    public readonly static PieceTypeDef B5 = new PieceTypeDef{Id = 204, Abbreviations = new List<string>{ "B5", "PieceB5" }};
    
    public readonly static PieceTypeDef C1 = new PieceTypeDef{Id = 300, Abbreviations = new List<string>{ "C1", "PieceC1" }};
    public readonly static PieceTypeDef C2 = new PieceTypeDef{Id = 301, Abbreviations = new List<string>{ "C2", "PieceC2" }};
    public readonly static PieceTypeDef C3 = new PieceTypeDef{Id = 302, Abbreviations = new List<string>{ "C3", "PieceC3" }};
    public readonly static PieceTypeDef C4 = new PieceTypeDef{Id = 303, Abbreviations = new List<string>{ "C4", "PieceC4" }};
    public readonly static PieceTypeDef C5 = new PieceTypeDef{Id = 304, Abbreviations = new List<string>{ "C5", "PieceC5" }};
    
    public readonly static PieceTypeDef E1 = new PieceTypeDef{Id = 400, Abbreviations = new List<string>{ "E1" }};
    public readonly static PieceTypeDef E2 = new PieceTypeDef{Id = 401, Abbreviations = new List<string>{ "E2" }};
    public readonly static PieceTypeDef E3 = new PieceTypeDef{Id = 402, Abbreviations = new List<string>{ "E3" }};
    
    public readonly static PieceTypeDef O1 = new PieceTypeDef{Id = 500, Abbreviations = new List<string>{ "O1" }};
    public readonly static PieceTypeDef O2 = new PieceTypeDef{Id = 501, Abbreviations = new List<string>{ "O2" }};
    public readonly static PieceTypeDef O3 = new PieceTypeDef{Id = 502, Abbreviations = new List<string>{ "O3" }};
    public readonly static PieceTypeDef O4 = new PieceTypeDef{Id = 503, Abbreviations = new List<string>{ "O4" }};
    
    public readonly static PieceTypeDef Fog = new PieceTypeDef{Id = 505, Abbreviations = new List<string>{ "Fog" }};
    
    public readonly static PieceTypeDef Mine1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "Mine1" }};
    public readonly static PieceTypeDef Mine2 = new PieceTypeDef{Id = 1001, Abbreviations = new List<string>{ "Mine2" }};
    public readonly static PieceTypeDef Mine3 = new PieceTypeDef{Id = 1002, Abbreviations = new List<string>{ "Mine3" }};
    public readonly static PieceTypeDef Mine4 = new PieceTypeDef{Id = 1003, Abbreviations = new List<string>{ "Mine4" }};
    public readonly static PieceTypeDef Mine5 = new PieceTypeDef{Id = 1004, Abbreviations = new List<string>{ "Mine5" }};
    public readonly static PieceTypeDef Mine6 = new PieceTypeDef{Id = 1005, Abbreviations = new List<string>{ "Mine6" }};
    public readonly static PieceTypeDef Mine7 = new PieceTypeDef{Id = 1006, Abbreviations = new List<string>{ "Mine7" }};
    
    public readonly static PieceTypeDef Sawmill1 = new PieceTypeDef{Id = 1100, Abbreviations = new List<string>{ "Sawmill1" }};
    public readonly static PieceTypeDef Sawmill2 = new PieceTypeDef{Id = 1101, Abbreviations = new List<string>{ "Sawmill2" }};
    public readonly static PieceTypeDef Sawmill3 = new PieceTypeDef{Id = 1102, Abbreviations = new List<string>{ "Sawmill3" }};
    public readonly static PieceTypeDef Sawmill4 = new PieceTypeDef{Id = 1103, Abbreviations = new List<string>{ "Sawmill4" }};
    public readonly static PieceTypeDef Sawmill5 = new PieceTypeDef{Id = 1104, Abbreviations = new List<string>{ "Sawmill5" }};
    public readonly static PieceTypeDef Sawmill6 = new PieceTypeDef{Id = 1105, Abbreviations = new List<string>{ "Sawmill6" }};
    public readonly static PieceTypeDef Sawmill7 = new PieceTypeDef{Id = 1106, Abbreviations = new List<string>{ "Sawmill7" }};
    
    public readonly static PieceTypeDef Sheepfold1 = new PieceTypeDef{Id = 1200, Abbreviations = new List<string>{ "Sheepfold1" }};
    public readonly static PieceTypeDef Sheepfold2 = new PieceTypeDef{Id = 1201, Abbreviations = new List<string>{ "Sheepfold2" }};
    public readonly static PieceTypeDef Sheepfold3 = new PieceTypeDef{Id = 1202, Abbreviations = new List<string>{ "Sheepfold3" }};
    public readonly static PieceTypeDef Sheepfold4 = new PieceTypeDef{Id = 1203, Abbreviations = new List<string>{ "Sheepfold4" }};
    public readonly static PieceTypeDef Sheepfold5 = new PieceTypeDef{Id = 1204, Abbreviations = new List<string>{ "Sheepfold5" }};
    public readonly static PieceTypeDef Sheepfold6 = new PieceTypeDef{Id = 1205, Abbreviations = new List<string>{ "Sheepfold6" }};
    public readonly static PieceTypeDef Sheepfold7 = new PieceTypeDef{Id = 1206, Abbreviations = new List<string>{ "Sheepfold7" }};
    
    public readonly static PieceTypeDef Gbox1 = new PieceTypeDef{Id = 1300, Abbreviations = new List<string>{ "GBOX1" }};
    
    public readonly static PieceTypeDef Chest1 = new PieceTypeDef{Id = 1301, Abbreviations = new List<string>{ "ChestCommon", "Chest1", "Common" }};
    public readonly static PieceTypeDef Chest2 = new PieceTypeDef{Id = 1302, Abbreviations = new List<string>{ "ChestRare", "Chest2", "Rare" }};
    public readonly static PieceTypeDef Chest3 = new PieceTypeDef{Id = 1303, Abbreviations = new List<string>{ "ChestEpic", "Chest3", "Epic" }};
    
    public readonly static PieceTypeDef Castle1 = new PieceTypeDef{Id = 1400, Abbreviations = new List<string>{ "Castle1" }};
    public readonly static PieceTypeDef Castle2 = new PieceTypeDef{Id = 1401, Abbreviations = new List<string>{ "Castle2" }};
    public readonly static PieceTypeDef Castle3 = new PieceTypeDef{Id = 1402, Abbreviations = new List<string>{ "Castle3" }};
    public readonly static PieceTypeDef Castle4 = new PieceTypeDef{Id = 1403, Abbreviations = new List<string>{ "Castle4" }};
    public readonly static PieceTypeDef Castle5 = new PieceTypeDef{Id = 1404, Abbreviations = new List<string>{ "Castle5" }};
    public readonly static PieceTypeDef Castle6 = new PieceTypeDef{Id = 1405, Abbreviations = new List<string>{ "Castle6" }};
    public readonly static PieceTypeDef Castle7 = new PieceTypeDef{Id = 1406, Abbreviations = new List<string>{ "Castle7" }};
    public readonly static PieceTypeDef Castle8 = new PieceTypeDef{Id = 1407, Abbreviations = new List<string>{ "Castle8" }};
    public readonly static PieceTypeDef Castle9 = new PieceTypeDef{Id = 1408, Abbreviations = new List<string>{ "Castle9" }};
    
    public readonly static PieceTypeDef Tavern1 = new PieceTypeDef{Id = 1500, Abbreviations = new List<string>{ "Tavern1" }};
    public readonly static PieceTypeDef Tavern2 = new PieceTypeDef{Id = 1501, Abbreviations = new List<string>{ "Tavern2" }};
    public readonly static PieceTypeDef Tavern3 = new PieceTypeDef{Id = 1502, Abbreviations = new List<string>{ "Tavern3" }};
    public readonly static PieceTypeDef Tavern4 = new PieceTypeDef{Id = 1503, Abbreviations = new List<string>{ "Tavern4" }};
    public readonly static PieceTypeDef Tavern5 = new PieceTypeDef{Id = 1504, Abbreviations = new List<string>{ "Tavern5" }};
    public readonly static PieceTypeDef Tavern6 = new PieceTypeDef{Id = 1505, Abbreviations = new List<string>{ "Tavern6" }};
    public readonly static PieceTypeDef Tavern7 = new PieceTypeDef{Id = 1506, Abbreviations = new List<string>{ "Tavern7" }};
    public readonly static PieceTypeDef Tavern8 = new PieceTypeDef{Id = 1507, Abbreviations = new List<string>{ "Tavern8" }};
    public readonly static PieceTypeDef Tavern9 = new PieceTypeDef{Id = 1508, Abbreviations = new List<string>{ "Tavern9" }};
}