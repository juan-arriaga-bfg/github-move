using System.Collections.Generic;

public partial class PieceType 
{
    public readonly static PieceTypeDef A1 = new PieceTypeDef{Id = 100, Abbreviations = new List<string>{ "A1" }};
    public readonly static PieceTypeDef A2 = new PieceTypeDef{Id = 101, Abbreviations = new List<string>{ "A2" }};
    public readonly static PieceTypeDef A3 = new PieceTypeDef{Id = 102, Abbreviations = new List<string>{ "A3" }};
    public readonly static PieceTypeDef A4 = new PieceTypeDef{Id = 103, Abbreviations = new List<string>{ "A4" }};
    
    public readonly static PieceTypeDef B1 = new PieceTypeDef{Id = 200, Abbreviations = new List<string>{ "B1" }};
    public readonly static PieceTypeDef B2 = new PieceTypeDef{Id = 201, Abbreviations = new List<string>{ "B2" }};
    public readonly static PieceTypeDef B3 = new PieceTypeDef{Id = 202, Abbreviations = new List<string>{ "B3" }};
    public readonly static PieceTypeDef B4 = new PieceTypeDef{Id = 203, Abbreviations = new List<string>{ "B4" }};
    
    public readonly static PieceTypeDef C1 = new PieceTypeDef{Id = 300, Abbreviations = new List<string>{ "C1", "coin" }};
    
    public readonly static PieceTypeDef E1 = new PieceTypeDef{Id = 400, Abbreviations = new List<string>{ "E1" }};
    public readonly static PieceTypeDef E2 = new PieceTypeDef{Id = 401, Abbreviations = new List<string>{ "E2" }};
    public readonly static PieceTypeDef E3 = new PieceTypeDef{Id = 402, Abbreviations = new List<string>{ "E3" }};
    
    public readonly static PieceTypeDef O1 = new PieceTypeDef{Id = 500, Abbreviations = new List<string>{ "O1" }};
    
    public readonly static PieceTypeDef M1 = new PieceTypeDef{Id = 600, Abbreviations = new List<string>{ "M1", "mine" }};
    public readonly static PieceTypeDef S1 = new PieceTypeDef{Id = 700, Abbreviations = new List<string>{ "S1", "saw" }};
}