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
        
        RegisterType(M1);
        RegisterType(S1);
        
        RegisterType(H1);
        RegisterType(H2);
        
        RegisterType(Gbox1);
    }
    
    public readonly static PieceTypeDef A1 = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "A1", "PieceA1" }};
    public readonly static PieceTypeDef A2 = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "A2", "PieceA2" }};
    public readonly static PieceTypeDef A3 = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "A3", "PieceA3" }};
    public readonly static PieceTypeDef A4 = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "A4" }};
    public readonly static PieceTypeDef A5 = new PieceTypeDef{Id = 104, Abbreviations = new List<string>{ "A5" }};
    public readonly static PieceTypeDef A6 = new PieceTypeDef{Id = 105, Abbreviations = new List<string>{ "A6" }};
    
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
    
    public readonly static PieceTypeDef M1 = new PieceTypeDef{Id = 600, Abbreviations = new List<string>{ "M1", "mine" }};
    public readonly static PieceTypeDef S1 = new PieceTypeDef{Id = 700, Abbreviations = new List<string>{ "S1", "saw" }};
    
    public readonly static PieceTypeDef H1 = new PieceTypeDef{Id = 800, Abbreviations = new List<string>{ "H1" }};
    public readonly static PieceTypeDef H2 = new PieceTypeDef{Id = 801, Abbreviations = new List<string>{ "H2" }};
    
    public readonly static PieceTypeDef Gbox1 = new PieceTypeDef{Id = 1000, Abbreviations = new List<string>{ "GBOX1" }};
}