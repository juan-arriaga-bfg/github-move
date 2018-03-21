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
        
        RegisterType(B1);
        RegisterType(B2);
        RegisterType(B3);
        RegisterType(B4);
        RegisterType(B5);
        
        RegisterType(C1);
        
        RegisterType(E1);
        RegisterType(E2);
        RegisterType(E3);
        
        RegisterType(O1);
        RegisterType(O2);
        
        RegisterType(M1);
        RegisterType(S1);
        
        RegisterType(H1);
        RegisterType(H2);
        
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
    public readonly static PieceTypeDef A4 = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "A4" }};
    public readonly static PieceTypeDef A5 = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "A5" }};
    public readonly static PieceTypeDef A6 = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "A6" }};
    public readonly static PieceTypeDef A7 = new PieceTypeDef{Id = 106, Abbreviations = new List<string>{ "A7" }};
    
    public readonly static PieceTypeDef B1 = new PieceTypeDef{Id = 200, Abbreviations = new List<string>{ "B1", "PieceB1" }};
    public readonly static PieceTypeDef B2 = new PieceTypeDef{Id = 201, Abbreviations = new List<string>{ "B2", "PieceB2" }};
    public readonly static PieceTypeDef B3 = new PieceTypeDef{Id = 202, Abbreviations = new List<string>{ "B3", "PieceB3" }};
    public readonly static PieceTypeDef B4 = new PieceTypeDef{Id = 203, Abbreviations = new List<string>{ "B4" }};
    public readonly static PieceTypeDef B5 = new PieceTypeDef{Id = 204, Abbreviations = new List<string>{ "B5" }};
    
    public readonly static PieceTypeDef C1 = new PieceTypeDef{Id = 300, Abbreviations = new List<string>{ "C1", "coin", "Coins" }};
    
    public readonly static PieceTypeDef E1 = new PieceTypeDef{Id = 400, Abbreviations = new List<string>{ "E1" }};
    public readonly static PieceTypeDef E2 = new PieceTypeDef{Id = 401, Abbreviations = new List<string>{ "E2" }};
    public readonly static PieceTypeDef E3 = new PieceTypeDef{Id = 402, Abbreviations = new List<string>{ "E3" }};
    
    public readonly static PieceTypeDef O1 = new PieceTypeDef{Id = 500, Abbreviations = new List<string>{ "O1" }};
    public readonly static PieceTypeDef O2 = new PieceTypeDef{Id = 501, Abbreviations = new List<string>{ "O2" }};
    
    public readonly static PieceTypeDef M1 = new PieceTypeDef{Id = 600, Abbreviations = new List<string>{ "M1", "mine" }};
    
    public readonly static PieceTypeDef S1 = new PieceTypeDef{Id = 700, Abbreviations = new List<string>{ "S1" }};
    public readonly static PieceTypeDef S2 = new PieceTypeDef{Id = 701, Abbreviations = new List<string>{ "S2" }};
    public readonly static PieceTypeDef S3 = new PieceTypeDef{Id = 702, Abbreviations = new List<string>{ "S3" }};
    public readonly static PieceTypeDef S4 = new PieceTypeDef{Id = 703, Abbreviations = new List<string>{ "S4" }};
    public readonly static PieceTypeDef S5 = new PieceTypeDef{Id = 704, Abbreviations = new List<string>{ "S5" }};
    public readonly static PieceTypeDef S6 = new PieceTypeDef{Id = 705, Abbreviations = new List<string>{ "S6" }};
    public readonly static PieceTypeDef S7 = new PieceTypeDef{Id = 706, Abbreviations = new List<string>{ "S7" }};
    
    public readonly static PieceTypeDef H1 = new PieceTypeDef{Id = 800, Abbreviations = new List<string>{ "H1" }};
    public readonly static PieceTypeDef H2 = new PieceTypeDef{Id = 801, Abbreviations = new List<string>{ "H2" }};
    
    public readonly static PieceTypeDef Gbox1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "GBOX1" }};
    
    public readonly static PieceTypeDef Chest1 = new PieceTypeDef{Id = 1001, Abbreviations = new List<string>{ "ChestCommon", "Chest1", "Common" }};
    public readonly static PieceTypeDef Chest2 = new PieceTypeDef{Id = 1002, Abbreviations = new List<string>{ "ChestRare", "Chest2", "Rare" }};
    public readonly static PieceTypeDef Chest3 = new PieceTypeDef{Id = 1003, Abbreviations = new List<string>{ "ChestEpic", "Chest3", "Epic" }};
    
    public readonly static PieceTypeDef Castle1 = new PieceTypeDef{Id = 1100, Abbreviations = new List<string>{ "Castle1" }};
    public readonly static PieceTypeDef Castle2 = new PieceTypeDef{Id = 1101, Abbreviations = new List<string>{ "Castle2" }};
    public readonly static PieceTypeDef Castle3 = new PieceTypeDef{Id = 1102, Abbreviations = new List<string>{ "Castle3" }};
    public readonly static PieceTypeDef Castle4 = new PieceTypeDef{Id = 1103, Abbreviations = new List<string>{ "Castle4" }};
    public readonly static PieceTypeDef Castle5 = new PieceTypeDef{Id = 1104, Abbreviations = new List<string>{ "Castle5" }};
    public readonly static PieceTypeDef Castle6 = new PieceTypeDef{Id = 1105, Abbreviations = new List<string>{ "Castle6" }};
    public readonly static PieceTypeDef Castle7 = new PieceTypeDef{Id = 1106, Abbreviations = new List<string>{ "Castle7" }};
    public readonly static PieceTypeDef Castle8 = new PieceTypeDef{Id = 1107, Abbreviations = new List<string>{ "Castle8" }};
    public readonly static PieceTypeDef Castle9 = new PieceTypeDef{Id = 1108, Abbreviations = new List<string>{ "Castle9" }};
    
    public readonly static PieceTypeDef Tavern1 = new PieceTypeDef{Id = 1200, Abbreviations = new List<string>{ "Tavern1" }};
    public readonly static PieceTypeDef Tavern2 = new PieceTypeDef{Id = 1201, Abbreviations = new List<string>{ "Tavern2" }};
    public readonly static PieceTypeDef Tavern3 = new PieceTypeDef{Id = 1202, Abbreviations = new List<string>{ "Tavern3" }};
    public readonly static PieceTypeDef Tavern4 = new PieceTypeDef{Id = 1203, Abbreviations = new List<string>{ "Tavern4" }};
    public readonly static PieceTypeDef Tavern5 = new PieceTypeDef{Id = 1204, Abbreviations = new List<string>{ "Tavern5" }};
    public readonly static PieceTypeDef Tavern6 = new PieceTypeDef{Id = 1205, Abbreviations = new List<string>{ "Tavern6" }};
    public readonly static PieceTypeDef Tavern7 = new PieceTypeDef{Id = 1206, Abbreviations = new List<string>{ "Tavern7" }};
    public readonly static PieceTypeDef Tavern8 = new PieceTypeDef{Id = 1207, Abbreviations = new List<string>{ "Tavern8" }};
    public readonly static PieceTypeDef Tavern9 = new PieceTypeDef{Id = 1208, Abbreviations = new List<string>{ "Tavern9" }};
    
}