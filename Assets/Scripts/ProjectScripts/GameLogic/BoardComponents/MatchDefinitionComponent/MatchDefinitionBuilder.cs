using System.Collections.Generic;

public class MatchDefinitionBuilder
{
    public Dictionary<int, int> Build()
    {
        var dict = new Dictionary<int, int>
        {
            {PieceType.A1.Id, PieceType.A2.Id},
            {PieceType.A2.Id, PieceType.A3.Id},
            {PieceType.A3.Id, PieceType.A4.Id},
            {PieceType.A4.Id, PieceType.A5.Id},
            {PieceType.A5.Id, PieceType.A6.Id},
            {PieceType.A6.Id, PieceType.A7.Id},
            {PieceType.A7.Id, PieceType.A8.Id},
            {PieceType.A8.Id, PieceType.A9.Id},
            
            {PieceType.B1.Id, PieceType.B2.Id},
            {PieceType.B2.Id, PieceType.B3.Id},
            {PieceType.B3.Id, PieceType.B4.Id},
            {PieceType.B4.Id, PieceType.B5.Id},
            {PieceType.B5.Id, PieceType.B6.Id},
            {PieceType.B6.Id, PieceType.B7.Id},
            {PieceType.B7.Id, PieceType.B8.Id},
            {PieceType.B8.Id, PieceType.B9.Id},
        };
        
        return dict;
    }
}