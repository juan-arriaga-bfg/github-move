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
            
            {PieceType.Sawmill1.Id, new PieceMatchDef {Next = PieceType.Sawmill2.Id, Previous = PieceType.None.Id, Count = 1}},
            {PieceType.Sawmill2.Id, new PieceMatchDef {Next = PieceType.Sawmill3.Id, Previous = PieceType.Sawmill1.Id, Count = 1}},
            {PieceType.Sawmill3.Id, new PieceMatchDef {Next = PieceType.Sawmill4.Id, Previous = PieceType.Sawmill2.Id, Count = 1}},
            {PieceType.Sawmill4.Id, new PieceMatchDef {Next = PieceType.Sawmill5.Id, Previous = PieceType.Sawmill3.Id, Count = 1}},
            {PieceType.Sawmill5.Id, new PieceMatchDef {Next = PieceType.Sawmill6.Id, Previous = PieceType.Sawmill4.Id, Count = 1}},
            {PieceType.Sawmill6.Id, new PieceMatchDef {Next = PieceType.Sawmill7.Id, Previous = PieceType.Sawmill5.Id, Count = 1}},
            {PieceType.Sawmill7.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Sawmill6.Id, Count = 1}},
            
            {PieceType.Castle1.Id, new PieceMatchDef {Next = PieceType.Castle2.Id, Previous = PieceType.None.Id, Count = 1}},
            {PieceType.Castle2.Id, new PieceMatchDef {Next = PieceType.Castle3.Id, Previous = PieceType.Castle1.Id, Count = 1}},
            {PieceType.Castle3.Id, new PieceMatchDef {Next = PieceType.Castle4.Id, Previous = PieceType.Castle2.Id, Count = 1}},
            {PieceType.Castle4.Id, new PieceMatchDef {Next = PieceType.Castle5.Id, Previous = PieceType.Castle3.Id, Count = 1}},
            {PieceType.Castle5.Id, new PieceMatchDef {Next = PieceType.Castle6.Id, Previous = PieceType.Castle4.Id, Count = 1}},
            {PieceType.Castle6.Id, new PieceMatchDef {Next = PieceType.Castle7.Id, Previous = PieceType.Castle5.Id, Count = 1}},
            {PieceType.Castle7.Id, new PieceMatchDef {Next = PieceType.Castle8.Id, Previous = PieceType.Castle6.Id, Count = 1}},
            {PieceType.Castle8.Id, new PieceMatchDef {Next = PieceType.Castle9.Id, Previous = PieceType.Castle7.Id, Count = 1}},
            {PieceType.Castle9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Castle8.Id, Count = 1}},
            
            {PieceType.Tavern1.Id, new PieceMatchDef {Next = PieceType.Tavern2.Id, Previous = PieceType.None.Id, Count = 1}},
            {PieceType.Tavern2.Id, new PieceMatchDef {Next = PieceType.Tavern3.Id, Previous = PieceType.Tavern1.Id, Count = 1}},
            {PieceType.Tavern3.Id, new PieceMatchDef {Next = PieceType.Tavern4.Id, Previous = PieceType.Tavern2.Id, Count = 1}},
            {PieceType.Tavern4.Id, new PieceMatchDef {Next = PieceType.Tavern5.Id, Previous = PieceType.Tavern3.Id, Count = 1}},
            {PieceType.Tavern5.Id, new PieceMatchDef {Next = PieceType.Tavern6.Id, Previous = PieceType.Tavern4.Id, Count = 1}},
            {PieceType.Tavern6.Id, new PieceMatchDef {Next = PieceType.Tavern7.Id, Previous = PieceType.Tavern5.Id, Count = 1}},
            {PieceType.Tavern7.Id, new PieceMatchDef {Next = PieceType.Tavern8.Id, Previous = PieceType.Tavern6.Id, Count = 1}},
            {PieceType.Tavern8.Id, new PieceMatchDef {Next = PieceType.Tavern9.Id, Previous = PieceType.Tavern7.Id, Count = 1}},
            {PieceType.Tavern9.Id, new PieceMatchDef {Next = PieceType.None.Id, Previous = PieceType.Tavern8.Id, Count = 1}},
        };
        
        return dict;
    }
}