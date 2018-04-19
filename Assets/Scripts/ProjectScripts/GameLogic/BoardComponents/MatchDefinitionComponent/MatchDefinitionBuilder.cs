using System.Collections.Generic;

public struct PieceMatchDef
{
    public int Next;
    public int Previous;
    public int Count;
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
        dict.Add(PieceType.D4.Id, new PieceMatchDef {Next = PieceType.D5.Id, Previous = PieceType.D3.Id, Count = 3});
        dict.Add(PieceType.D5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.D4.Id, Count = 3});

        dict.Add(PieceType.E1.Id, new PieceMatchDef {Next = PieceType.E2.Id, Previous = PieceType.None.Id, Count = 3});
        dict.Add(PieceType.E2.Id, new PieceMatchDef {Next = PieceType.E3.Id, Previous = PieceType.E1.Id, Count = 3});
        dict.Add(PieceType.E3.Id, new PieceMatchDef {Next = PieceType.E4.Id, Previous = PieceType.E2.Id, Count = 3});
        dict.Add(PieceType.E4.Id, new PieceMatchDef {Next = PieceType.E5.Id, Previous = PieceType.E3.Id, Count = 3});
        dict.Add(PieceType.E5.Id, new PieceMatchDef {Next = PieceType.E6.Id, Previous = PieceType.E4.Id, Count = 3});
        dict.Add(PieceType.E6.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.E5.Id, Count = 3});

        dict.Add(PieceType.O1.Id, new PieceMatchDef {Next = PieceType.O2.Id, Previous = PieceType.None.Id, Count = -1});
        dict.Add(PieceType.O2.Id, new PieceMatchDef {Next = PieceType.O3.Id, Previous = PieceType.O1.Id, Count = -1});
        dict.Add(PieceType.O3.Id, new PieceMatchDef {Next = PieceType.O4.Id, Previous = PieceType.O2.Id, Count = -1});
        dict.Add(PieceType.O4.Id, new PieceMatchDef {Next = PieceType.O5.Id, Previous = PieceType.O3.Id, Count = -1});
        dict.Add(PieceType.O5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.O4.Id, Count = -1});

        dict.Add(PieceType.OX1.Id, new PieceMatchDef {Next = PieceType.OX2.Id, Previous = PieceType.None.Id, Count = -1});
        dict.Add(PieceType.OX2.Id, new PieceMatchDef {Next = PieceType.OX3.Id, Previous = PieceType.OX1.Id, Count = -1});
        dict.Add(PieceType.OX3.Id, new PieceMatchDef {Next = PieceType.OX4.Id, Previous = PieceType.OX2.Id, Count = -1});
        dict.Add(PieceType.OX4.Id, new PieceMatchDef {Next = PieceType.OX5.Id, Previous = PieceType.OX3.Id, Count = -1});
        dict.Add(PieceType.OX5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.OX4.Id, Count = -1});
        
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
        dict.Add(PieceType.Mine1.Id, new PieceMatchDef {Next = PieceType.Mine2.Id, Previous = PieceType.None.Id, Count = 1});
        dict.Add(PieceType.Mine2.Id, new PieceMatchDef {Next = PieceType.Mine3.Id, Previous = PieceType.Mine1.Id, Count = 1});
        dict.Add(PieceType.Mine3.Id, new PieceMatchDef {Next = PieceType.Mine4.Id, Previous = PieceType.Mine2.Id, Count = 1});
        dict.Add(PieceType.Mine4.Id, new PieceMatchDef {Next = PieceType.Mine5.Id, Previous = PieceType.Mine3.Id, Count = 1});
        dict.Add(PieceType.Mine5.Id, new PieceMatchDef {Next = PieceType.Mine6.Id, Previous = PieceType.Mine4.Id, Count = 1});
        dict.Add(PieceType.Mine6.Id, new PieceMatchDef {Next = PieceType.Mine7.Id, Previous = PieceType.Mine5.Id, Count = 1});
        dict.Add(PieceType.Mine7.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Mine6.Id, Count = 1});
        
        dict.Add(PieceType.Sawmill1.Id, new PieceMatchDef {Next = PieceType.Sawmill2.Id, Previous = PieceType.None.Id, Count = 1});
        dict.Add(PieceType.Sawmill2.Id, new PieceMatchDef {Next = PieceType.Sawmill3.Id, Previous = PieceType.Sawmill1.Id, Count = 1});
        dict.Add(PieceType.Sawmill3.Id, new PieceMatchDef {Next = PieceType.Sawmill4.Id, Previous = PieceType.Sawmill2.Id, Count = 1});
        dict.Add(PieceType.Sawmill4.Id, new PieceMatchDef {Next = PieceType.Sawmill5.Id, Previous = PieceType.Sawmill3.Id, Count = 1});
        dict.Add(PieceType.Sawmill5.Id, new PieceMatchDef {Next = PieceType.Sawmill6.Id, Previous = PieceType.Sawmill4.Id, Count = 1});
        dict.Add(PieceType.Sawmill6.Id, new PieceMatchDef {Next = PieceType.Sawmill7.Id, Previous = PieceType.Sawmill5.Id, Count = 1});
        dict.Add(PieceType.Sawmill7.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Sawmill6.Id, Count = 1});
        
        dict.Add(PieceType.Sheepfold1.Id, new PieceMatchDef {Next = PieceType.Sheepfold2.Id, Previous = PieceType.None.Id, Count = 1});
        dict.Add(PieceType.Sheepfold2.Id, new PieceMatchDef {Next = PieceType.Sheepfold3.Id, Previous = PieceType.Sheepfold1.Id, Count = 1});
        dict.Add(PieceType.Sheepfold3.Id, new PieceMatchDef {Next = PieceType.Sheepfold4.Id, Previous = PieceType.Sheepfold2.Id, Count = 1});
        dict.Add(PieceType.Sheepfold4.Id, new PieceMatchDef {Next = PieceType.Sheepfold5.Id, Previous = PieceType.Sheepfold3.Id, Count = 1});
        dict.Add(PieceType.Sheepfold5.Id, new PieceMatchDef {Next = PieceType.Sheepfold6.Id, Previous = PieceType.Sheepfold4.Id, Count = 1});
        dict.Add(PieceType.Sheepfold6.Id, new PieceMatchDef {Next = PieceType.Sheepfold7.Id, Previous = PieceType.Sheepfold5.Id, Count = 1});
        dict.Add(PieceType.Sheepfold7.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Sheepfold6.Id, Count = 1});
        
        dict.Add(PieceType.Castle1.Id, new PieceMatchDef {Next = PieceType.Castle2.Id, Previous = PieceType.None.Id, Count = 1});
        dict.Add(PieceType.Castle2.Id, new PieceMatchDef {Next = PieceType.Castle3.Id, Previous = PieceType.Castle1.Id, Count = 1});
        dict.Add(PieceType.Castle3.Id, new PieceMatchDef {Next = PieceType.Castle4.Id, Previous = PieceType.Castle2.Id, Count = 1});
        dict.Add(PieceType.Castle4.Id, new PieceMatchDef {Next = PieceType.Castle5.Id, Previous = PieceType.Castle3.Id, Count = 1});
        dict.Add(PieceType.Castle5.Id, new PieceMatchDef {Next = PieceType.Castle6.Id, Previous = PieceType.Castle4.Id, Count = 1});
        dict.Add(PieceType.Castle6.Id, new PieceMatchDef {Next = PieceType.Castle7.Id, Previous = PieceType.Castle5.Id, Count = 1});
        dict.Add(PieceType.Castle7.Id, new PieceMatchDef {Next = PieceType.Castle8.Id, Previous = PieceType.Castle6.Id, Count = 1});
        dict.Add(PieceType.Castle8.Id, new PieceMatchDef {Next = PieceType.Castle9.Id, Previous = PieceType.Castle7.Id, Count = 1});
        dict.Add(PieceType.Castle9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Castle8.Id, Count = 1});
        
        dict.Add(PieceType.Tavern1.Id, new PieceMatchDef {Next = PieceType.Tavern2.Id, Previous = PieceType.None.Id, Count = 1});
        dict.Add(PieceType.Tavern2.Id, new PieceMatchDef {Next = PieceType.Tavern3.Id, Previous = PieceType.Tavern1.Id, Count = 1});
        dict.Add(PieceType.Tavern3.Id, new PieceMatchDef {Next = PieceType.Tavern4.Id, Previous = PieceType.Tavern2.Id, Count = 1});
        dict.Add(PieceType.Tavern4.Id, new PieceMatchDef {Next = PieceType.Tavern5.Id, Previous = PieceType.Tavern3.Id, Count = 1});
        dict.Add(PieceType.Tavern5.Id, new PieceMatchDef {Next = PieceType.Tavern6.Id, Previous = PieceType.Tavern4.Id, Count = 1});
        dict.Add(PieceType.Tavern6.Id, new PieceMatchDef {Next = PieceType.Tavern7.Id, Previous = PieceType.Tavern5.Id, Count = 1});
        dict.Add(PieceType.Tavern7.Id, new PieceMatchDef {Next = PieceType.Tavern8.Id, Previous = PieceType.Tavern6.Id, Count = 1});
        dict.Add(PieceType.Tavern8.Id, new PieceMatchDef {Next = PieceType.Tavern9.Id, Previous = PieceType.Tavern7.Id, Count = 1});
        dict.Add(PieceType.Tavern9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Tavern8.Id, Count = 1});
        
        return dict;
    }
}