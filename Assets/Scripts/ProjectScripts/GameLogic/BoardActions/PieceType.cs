using System.Collections.Generic;

public static partial class PieceType 
{
    static PieceType()
    {
        RegisterType(Empty);
        
        RegisterType(A1);
        RegisterType(A2);
        RegisterType(A3);
        RegisterType(A4);
        RegisterType(A5);
        RegisterType(A6);
        RegisterType(A7);
        RegisterType(A8);
        RegisterType(A9);
        
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
        RegisterType(C6);
        RegisterType(C7);
        RegisterType(C8);
        RegisterType(C9);
        
        RegisterType(D1);
        RegisterType(D2);
        RegisterType(D3);
        RegisterType(D4);
        RegisterType(D5);
        
        RegisterType(E1);
        RegisterType(E2);
        RegisterType(E3);
        RegisterType(E4);
        RegisterType(E5);
        RegisterType(E6);
        
        RegisterType(O1);
        RegisterType(O2);
        RegisterType(O3);
        RegisterType(O4);
        RegisterType(O5);
        
        RegisterType(OX1);
        RegisterType(OX2);
        RegisterType(OX3);
        RegisterType(OX4);
        RegisterType(OX5);
        
        RegisterType(King);
        
        RegisterType(Fog);
        
        RegisterType(Coin1);
        RegisterType(Coin2);
        RegisterType(Coin3);
        RegisterType(Coin4);
        RegisterType(Coin5);
        
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
        
        RegisterType(Chest1);
        RegisterType(Chest2);
        RegisterType(Chest3);
        RegisterType(Chest4);
        RegisterType(Chest5);
        RegisterType(Chest6);
        RegisterType(Chest7);
        RegisterType(Chest8);
        RegisterType(Chest9);
        
        RegisterType(Castle1);
        RegisterType(Castle2);
        RegisterType(Castle3);
        RegisterType(Castle4);
        RegisterType(Castle5);
        RegisterType(Castle6);
        RegisterType(Castle7);
        RegisterType(Castle8);
        RegisterType(Castle9);
        
        RegisterType(Market1);
        RegisterType(Market2);
        RegisterType(Market3);
        RegisterType(Market4);
        RegisterType(Market5);
        RegisterType(Market6);
        RegisterType(Market7);
        RegisterType(Market8);
        RegisterType(Market9);
        
        RegisterType(Storage1);
        RegisterType(Storage2);
        RegisterType(Storage3);
        RegisterType(Storage4);
        RegisterType(Storage5);
        RegisterType(Storage6);
        RegisterType(Storage7);
        RegisterType(Storage8);
        RegisterType(Storage9);
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
    
    public static readonly PieceTypeDef O1 = new PieceTypeDef{Id = 600, Abbreviations = new List<string>{ "O1" }};
    public static readonly PieceTypeDef O2 = new PieceTypeDef{Id = 601, Abbreviations = new List<string>{ "O2" }};
    public static readonly PieceTypeDef O3 = new PieceTypeDef{Id = 602, Abbreviations = new List<string>{ "O3" }};
    public static readonly PieceTypeDef O4 = new PieceTypeDef{Id = 603, Abbreviations = new List<string>{ "O4" }};
    public static readonly PieceTypeDef O5 = new PieceTypeDef{Id = 604, Abbreviations = new List<string>{ "O5" }};
    
    public static readonly PieceTypeDef OX1 = new PieceTypeDef{Id = 700, Abbreviations = new List<string>{ "OX1" }};
    public static readonly PieceTypeDef OX2 = new PieceTypeDef{Id = 701, Abbreviations = new List<string>{ "OX2" }};
    public static readonly PieceTypeDef OX3 = new PieceTypeDef{Id = 702, Abbreviations = new List<string>{ "OX3" }};
    public static readonly PieceTypeDef OX4 = new PieceTypeDef{Id = 703, Abbreviations = new List<string>{ "OX4" }};
    public static readonly PieceTypeDef OX5 = new PieceTypeDef{Id = 704, Abbreviations = new List<string>{ "OX5" }};
    
    public static readonly PieceTypeDef Fog = new PieceTypeDef{Id = 801, Abbreviations = new List<string>{ "Fog" }};
    
    public static readonly PieceTypeDef King = new PieceTypeDef{Id = 901, Abbreviations = new List<string>{ "King" }};
    
    public static readonly PieceTypeDef Coin1 = new PieceTypeDef{Id = 1100, Abbreviations = new List<string>{ "Coin1" }};
    public static readonly PieceTypeDef Coin2 = new PieceTypeDef{Id = 1101, Abbreviations = new List<string>{ "Coin2" }};
    public static readonly PieceTypeDef Coin3 = new PieceTypeDef{Id = 1102, Abbreviations = new List<string>{ "Coin3" }}; 
    public static readonly PieceTypeDef Coin4 = new PieceTypeDef{Id = 1103, Abbreviations = new List<string>{ "Coin4" }};
    public static readonly PieceTypeDef Coin5 = new PieceTypeDef{Id = 1104, Abbreviations = new List<string>{ "Coin5" }};
    
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
}