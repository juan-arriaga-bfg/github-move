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
        var dict = new Dictionary<int, PieceMatchDef>
        {
            {PieceType.A1.Id, new PieceMatchDef {Next = PieceType.A2.Id, Previous = PieceType.None.Id, Count = 3}},
            {PieceType.A2.Id, new PieceMatchDef {Next = PieceType.A3.Id, Previous = PieceType.A1.Id, Count = 3}},
            {PieceType.A3.Id, new PieceMatchDef {Next = PieceType.A4.Id, Previous = PieceType.A2.Id, Count = 3}},
            {PieceType.A4.Id, new PieceMatchDef {Next = PieceType.A5.Id, Previous = PieceType.A3.Id, Count = 3}},
            {PieceType.A5.Id, new PieceMatchDef {Next = PieceType.A6.Id, Previous = PieceType.A4.Id, Count = 3}},
            {PieceType.A6.Id, new PieceMatchDef {Next = PieceType.A7.Id, Previous = PieceType.A5.Id, Count = 3}},
            {PieceType.A7.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.A6.Id, Count = 3}},

            {PieceType.B1.Id, new PieceMatchDef {Next = PieceType.B2.Id, Previous = PieceType.None.Id, Count = 3}},
            {PieceType.B2.Id, new PieceMatchDef {Next = PieceType.B3.Id, Previous = PieceType.B1.Id, Count = 3}},
            {PieceType.B3.Id, new PieceMatchDef {Next = PieceType.B4.Id, Previous = PieceType.B2.Id, Count = 3}},
            {PieceType.B4.Id, new PieceMatchDef {Next = PieceType.B5.Id, Previous = PieceType.B3.Id, Count = 3}},
            {PieceType.B5.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.B4.Id, Count = 3}},
            
            {PieceType.S1.Id, new PieceMatchDef {Next = PieceType.S2.Id, Previous = PieceType.None.Id, Count = 1}},
            {PieceType.S2.Id, new PieceMatchDef {Next = PieceType.S3.Id, Previous = PieceType.S1.Id, Count = 1}},
            {PieceType.S3.Id, new PieceMatchDef {Next = PieceType.S4.Id, Previous = PieceType.S2.Id, Count = 1}},
            {PieceType.S4.Id, new PieceMatchDef {Next = PieceType.S5.Id, Previous = PieceType.S3.Id, Count = 1}},
            {PieceType.S5.Id, new PieceMatchDef {Next = PieceType.S6.Id, Previous = PieceType.S4.Id, Count = 1}},
            {PieceType.S6.Id, new PieceMatchDef {Next = PieceType.S7.Id, Previous = PieceType.S5.Id, Count = 1}},
            {PieceType.S7.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.S6.Id, Count = 1}},
        };
        
        return dict;
    }
}