using System.Collections.Generic;

public struct PieceMatchDef
{
    public int Next;
    public int Previous;
    public int Count;
    public bool IsIgnore;
}

public class MatchDefinitionBuilder
{
    public Dictionary<int, PieceMatchDef> Build()
    {
        var dict = new Dictionary<int, PieceMatchDef>();

        dict = AddPiece(dict);
        dict = AddMulticellularPiece(dict);
        
        return dict;
    }

    private Dictionary<int, PieceMatchDef> AddPiece(Dictionary<int, PieceMatchDef> dict)
    {
        dict.Add(PieceType.A1.Id, new PieceMatchDef {Next = PieceType.A2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.A2.Id, new PieceMatchDef {Next = PieceType.A3.Id, Previous = PieceType.A1.Id, Count = 3});
        dict.Add(PieceType.A3.Id, new PieceMatchDef {Next = PieceType.A4.Id, Previous = PieceType.A2.Id, Count = 3});
        dict.Add(PieceType.A4.Id, new PieceMatchDef {Next = PieceType.A5.Id, Previous = PieceType.A3.Id, Count = 3});
        dict.Add(PieceType.A5.Id, new PieceMatchDef {Next = PieceType.A6.Id, Previous = PieceType.A4.Id, Count = 3});
        dict.Add(PieceType.A6.Id, new PieceMatchDef {Next = PieceType.A7.Id, Previous = PieceType.A5.Id, Count = 3});
        dict.Add(PieceType.A7.Id, new PieceMatchDef {Next = PieceType.A8.Id, Previous = PieceType.A6.Id, Count = 3});
        dict.Add(PieceType.A8.Id, new PieceMatchDef {Next = PieceType.A9.Id, Previous = PieceType.A7.Id, Count = 3});
        dict.Add(PieceType.A9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.A8.Id, Count = 3});

        dict.Add(PieceType.B1.Id, new PieceMatchDef {Next = PieceType.B2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.B2.Id, new PieceMatchDef {Next = PieceType.B3.Id, Previous = PieceType.B1.Id, Count = 3});
        dict.Add(PieceType.B3.Id, new PieceMatchDef {Next = PieceType.B4.Id, Previous = PieceType.B2.Id, Count = 3});
        dict.Add(PieceType.B4.Id, new PieceMatchDef {Next = PieceType.B5.Id, Previous = PieceType.B3.Id, Count = 3});
        dict.Add(PieceType.B5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.B4.Id, Count = 3});

        dict.Add(PieceType.C1.Id, new PieceMatchDef {Next = PieceType.C2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.C2.Id, new PieceMatchDef {Next = PieceType.C3.Id, Previous = PieceType.C1.Id, Count = 3});
        dict.Add(PieceType.C3.Id, new PieceMatchDef {Next = PieceType.C4.Id, Previous = PieceType.C2.Id, Count = 3});
        dict.Add(PieceType.C4.Id, new PieceMatchDef {Next = PieceType.C5.Id, Previous = PieceType.C3.Id, Count = 3});
        dict.Add(PieceType.C5.Id, new PieceMatchDef {Next = PieceType.C6.Id, Previous = PieceType.C4.Id, Count = 3});
        dict.Add(PieceType.C6.Id, new PieceMatchDef {Next = PieceType.C7.Id, Previous = PieceType.C5.Id, Count = 3});
        dict.Add(PieceType.C7.Id, new PieceMatchDef {Next = PieceType.C8.Id, Previous = PieceType.C6.Id, Count = 3});
        dict.Add(PieceType.C8.Id, new PieceMatchDef {Next = PieceType.C9.Id, Previous = PieceType.C7.Id, Count = 3});
        dict.Add(PieceType.C9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.C8.Id, Count = 3});

        dict.Add(PieceType.D1.Id, new PieceMatchDef {Next = PieceType.D2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.D2.Id, new PieceMatchDef {Next = PieceType.D3.Id, Previous = PieceType.D1.Id, Count = 3});
        dict.Add(PieceType.D3.Id, new PieceMatchDef {Next = PieceType.D4.Id, Previous = PieceType.D2.Id, Count = 3});
        dict.Add(PieceType.D4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.D3.Id, Count = 3});
//        dict.Add(PieceType.D5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.D4.Id, Count = 3});

        dict.Add(PieceType.E1.Id, new PieceMatchDef {Next = PieceType.E2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.E2.Id, new PieceMatchDef {Next = PieceType.E3.Id, Previous = PieceType.E1.Id, Count = 3});
        dict.Add(PieceType.E3.Id, new PieceMatchDef {Next = PieceType.E4.Id, Previous = PieceType.E2.Id, Count = 3});
        dict.Add(PieceType.E4.Id, new PieceMatchDef {Next = PieceType.E5.Id, Previous = PieceType.E3.Id, Count = 3});
        dict.Add(PieceType.E5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.E4.Id, Count = 3});
//        dict.Add(PieceType.E6.Id, new PieceMatchDef {Next = PieceType.Coin4.Id, Previous = PieceType.E5.Id, Count = 3}, IsIgnore = true});
        
        dict.Add(PieceType.F1.Id, new PieceMatchDef {Next = PieceType.F2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.F2.Id, new PieceMatchDef {Next = PieceType.F3.Id, Previous = PieceType.F1.Id, Count = 3});
        dict.Add(PieceType.F3.Id, new PieceMatchDef {Next = PieceType.F4.Id, Previous = PieceType.F2.Id, Count = 3});
        dict.Add(PieceType.F4.Id, new PieceMatchDef {Next = PieceType.F5.Id, Previous = PieceType.F3.Id, Count = 3});
        dict.Add(PieceType.F5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.F4.Id, Count = 3});

        dict.Add(PieceType.G1.Id, new PieceMatchDef {Next = PieceType.G2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.G2.Id, new PieceMatchDef {Next = PieceType.G3.Id, Previous = PieceType.G1.Id, Count = 3});
        dict.Add(PieceType.G3.Id, new PieceMatchDef {Next = PieceType.G4.Id, Previous = PieceType.G2.Id, Count = 3});
        dict.Add(PieceType.G4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.G3.Id, Count = 3});
        
        dict.Add(PieceType.H1.Id, new PieceMatchDef {Next = PieceType.H2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.H2.Id, new PieceMatchDef {Next = PieceType.H3.Id, Previous = PieceType.H1.Id, Count = 3});
        dict.Add(PieceType.H3.Id, new PieceMatchDef {Next = PieceType.H4.Id, Previous = PieceType.H2.Id, Count = 3});
        dict.Add(PieceType.H4.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.H3.Id, Count = 3});

        dict.Add(PieceType.I1.Id, new PieceMatchDef {Next = PieceType.I2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.I2.Id, new PieceMatchDef {Next = PieceType.I3.Id, Previous = PieceType.I1.Id, Count = 3});
        dict.Add(PieceType.I3.Id, new PieceMatchDef {Next = PieceType.I4.Id, Previous = PieceType.I2.Id, Count = 3});
        dict.Add(PieceType.I4.Id, new PieceMatchDef {Next = PieceType.I5.Id, Previous = PieceType.I3.Id, Count = 3});
        dict.Add(PieceType.I5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.I4.Id, Count = 3});
        
        dict.Add(PieceType.J1.Id, new PieceMatchDef {Next = PieceType.J2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.J2.Id, new PieceMatchDef {Next = PieceType.J3.Id, Previous = PieceType.J1.Id, Count = 3});
        dict.Add(PieceType.J3.Id, new PieceMatchDef {Next = PieceType.J4.Id, Previous = PieceType.J2.Id, Count = 3});
        dict.Add(PieceType.J4.Id, new PieceMatchDef {Next = PieceType.J5.Id, Previous = PieceType.J3.Id, Count = 3});
        dict.Add(PieceType.J5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.J4.Id, Count = 3});
        
        dict.Add(PieceType.X1.Id, new PieceMatchDef {Next = PieceType.X2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.X2.Id, new PieceMatchDef {Next = PieceType.X3.Id, Previous = PieceType.X1.Id, Count = 3});
        dict.Add(PieceType.X3.Id, new PieceMatchDef {Next = PieceType.X4.Id, Previous = PieceType.X2.Id, Count = 3});
        dict.Add(PieceType.X4.Id, new PieceMatchDef {Next = PieceType.X5.Id, Previous = PieceType.X3.Id, Count = 3});
        dict.Add(PieceType.X5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.X4.Id, Count = 3});
        
        dict.Add(PieceType.O1.Id, new PieceMatchDef {Next = PieceType.O2.Id, Previous = PieceType.None.Id, Count = -1});
        dict.Add(PieceType.O2.Id, new PieceMatchDef {Next = PieceType.O3.Id, Previous = PieceType.O1.Id, Count = -1});
        dict.Add(PieceType.O3.Id, new PieceMatchDef {Next = PieceType.O4.Id, Previous = PieceType.O2.Id, Count = -1});
        dict.Add(PieceType.O4.Id, new PieceMatchDef {Next = PieceType.O5.Id, Previous = PieceType.O3.Id, Count = -1});
        dict.Add(PieceType.O5.Id, new PieceMatchDef {Next = PieceType.O6.Id, Previous = PieceType.O4.Id, Count = -1});
        dict.Add(PieceType.O6.Id, new PieceMatchDef {Next = PieceType.O7.Id, Previous = PieceType.O5.Id, Count = -1});
        dict.Add(PieceType.O7.Id, new PieceMatchDef {Next = PieceType.O8.Id, Previous = PieceType.O6.Id, Count = -1});
        dict.Add(PieceType.O8.Id, new PieceMatchDef {Next = PieceType.O9.Id, Previous = PieceType.O7.Id, Count = -1});
        dict.Add(PieceType.O9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.O8.Id, Count = -1});

        dict.Add(PieceType.OX1.Id, new PieceMatchDef {Next = PieceType.OX2.Id, Previous = PieceType.None.Id, Count = -1});
        dict.Add(PieceType.OX2.Id, new PieceMatchDef {Next = PieceType.OX3.Id, Previous = PieceType.OX1.Id, Count = -1});
        dict.Add(PieceType.OX3.Id, new PieceMatchDef {Next = PieceType.OX4.Id, Previous = PieceType.OX2.Id, Count = -1});
        dict.Add(PieceType.OX4.Id, new PieceMatchDef {Next = PieceType.OX5.Id, Previous = PieceType.OX3.Id, Count = -1});
        dict.Add(PieceType.OX5.Id, new PieceMatchDef {Next = PieceType.OX6.Id, Previous = PieceType.OX4.Id, Count = -1});
        dict.Add(PieceType.OX6.Id, new PieceMatchDef {Next = PieceType.OX7.Id, Previous = PieceType.OX5.Id, Count = -1});
        dict.Add(PieceType.OX7.Id, new PieceMatchDef {Next = PieceType.OX8.Id, Previous = PieceType.OX6.Id, Count = -1});
        dict.Add(PieceType.OX8.Id, new PieceMatchDef {Next = PieceType.OX9.Id, Previous = PieceType.OX7.Id, Count = -1});
        dict.Add(PieceType.OX9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.OX8.Id, Count = -1});
        
        dict.Add(PieceType.ChestA1.Id, new PieceMatchDef {Next = PieceType.ChestA2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.ChestA2.Id, new PieceMatchDef {Next = PieceType.ChestA3.Id, Previous = PieceType.ChestA1.Id, Count = 3});
        dict.Add(PieceType.ChestA3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.ChestA2.Id, Count = 3});
        
        dict.Add(PieceType.ChestX1.Id, new PieceMatchDef {Next = PieceType.ChestX2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.ChestX2.Id, new PieceMatchDef {Next = PieceType.ChestX3.Id, Previous = PieceType.ChestX1.Id, Count = 3});
        dict.Add(PieceType.ChestX3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.ChestX2.Id, Count = 3});
        
        dict.Add(PieceType.ChestC1.Id, new PieceMatchDef {Next = PieceType.ChestC2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.ChestC2.Id, new PieceMatchDef {Next = PieceType.ChestC3.Id, Previous = PieceType.ChestC1.Id, Count = 3});
        dict.Add(PieceType.ChestC3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.ChestC2.Id, Count = 3});
        
        dict.Add(PieceType.Basket1.Id, new PieceMatchDef {Next = PieceType.Basket2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.Basket2.Id, new PieceMatchDef {Next = PieceType.Basket3.Id, Previous = PieceType.Basket1.Id, Count = 3});
        dict.Add(PieceType.Basket3.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Basket2.Id, Count = 3});
        
        dict.Add(PieceType.Chest1.Id, new PieceMatchDef {Next = PieceType.Chest2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.Chest2.Id, new PieceMatchDef {Next = PieceType.Chest3.Id, Previous = PieceType.Chest1.Id, Count = 3});
        dict.Add(PieceType.Chest3.Id, new PieceMatchDef {Next = PieceType.Chest4.Id, Previous = PieceType.Chest2.Id, Count = 3});
        dict.Add(PieceType.Chest4.Id, new PieceMatchDef {Next = PieceType.Chest5.Id, Previous = PieceType.Chest3.Id, Count = 3});
        dict.Add(PieceType.Chest5.Id, new PieceMatchDef {Next = PieceType.Chest6.Id, Previous = PieceType.Chest4.Id, Count = 3});
        dict.Add(PieceType.Chest6.Id, new PieceMatchDef {Next = PieceType.Chest7.Id, Previous = PieceType.Chest5.Id, Count = 3});
        dict.Add(PieceType.Chest7.Id, new PieceMatchDef {Next = PieceType.Chest8.Id, Previous = PieceType.Chest6.Id, Count = 3});
        dict.Add(PieceType.Chest8.Id, new PieceMatchDef {Next = PieceType.Chest9.Id, Previous = PieceType.Chest7.Id, Count = 3});
        dict.Add(PieceType.Chest9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Chest8.Id, Count = 3});
        
        dict.Add(PieceType.Coin1.Id, new PieceMatchDef {Next = PieceType.Coin2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.Coin2.Id, new PieceMatchDef {Next = PieceType.Coin3.Id, Previous = PieceType.Coin1.Id, Count = 3});
        dict.Add(PieceType.Coin3.Id, new PieceMatchDef {Next = PieceType.Coin4.Id, Previous = PieceType.Coin2.Id, Count = 3});
        dict.Add(PieceType.Coin4.Id, new PieceMatchDef {Next = PieceType.Coin5.Id, Previous = PieceType.Coin3.Id, Count = 3});
        dict.Add(PieceType.Coin5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Coin4.Id, Count = 3});
        
        return dict;
    }

    private Dictionary<int, PieceMatchDef> AddMulticellularPiece(Dictionary<int, PieceMatchDef> dict)
    {
        dict.Add(PieceType.Castle1.Id, new PieceMatchDef {Next = PieceType.Castle2.Id, Previous = PieceType.None.Id, Count = 1});
        dict.Add(PieceType.Castle2.Id, new PieceMatchDef {Next = PieceType.Castle3.Id, Previous = PieceType.Castle1.Id, Count = 1});
        dict.Add(PieceType.Castle3.Id, new PieceMatchDef {Next = PieceType.Castle4.Id, Previous = PieceType.Castle2.Id, Count = 1});
        dict.Add(PieceType.Castle4.Id, new PieceMatchDef {Next = PieceType.Castle5.Id, Previous = PieceType.Castle3.Id, Count = 1});
        dict.Add(PieceType.Castle5.Id, new PieceMatchDef {Next = PieceType.Castle6.Id, Previous = PieceType.Castle4.Id, Count = 1});
        dict.Add(PieceType.Castle6.Id, new PieceMatchDef {Next = PieceType.Castle7.Id, Previous = PieceType.Castle5.Id, Count = 1});
        dict.Add(PieceType.Castle7.Id, new PieceMatchDef {Next = PieceType.Castle8.Id, Previous = PieceType.Castle6.Id, Count = 1});
        dict.Add(PieceType.Castle8.Id, new PieceMatchDef {Next = PieceType.Castle9.Id, Previous = PieceType.Castle7.Id, Count = 1});
        dict.Add(PieceType.Castle9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Castle8.Id, Count = 1});
        
        return dict;
    }
}