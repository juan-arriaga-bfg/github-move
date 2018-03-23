using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsResourcesBuilder
{
    public Dictionary<int, string> Build()
    {
        var dict = new Dictionary<int, string>
        {
            {PieceType.Generic.Id, R.GenericPiece},

            {PieceType.M1.Id, R.M1Piece},

            {PieceType.Sawmill1.Id, R.Sawmill1Piece},
            {PieceType.Sawmill2.Id, R.Sawmill2Piece},
            {PieceType.Sawmill3.Id, R.Sawmill3Piece},
            {PieceType.Sawmill4.Id, R.Sawmill4Piece},
            {PieceType.Sawmill5.Id, R.Sawmill5Piece},
            {PieceType.Sawmill6.Id, R.Sawmill6Piece},
            {PieceType.Sawmill7.Id, R.Sawmill7Piece},
            
            {PieceType.E1.Id, R.E1Piece},
            {PieceType.E2.Id, R.E2Piece},
            {PieceType.E3.Id, R.E3Piece},

            {PieceType.O1.Id, R.O1Piece},
            {PieceType.O2.Id, R.O2Piece},
            {PieceType.O3.Id, R.O3Piece},
            {PieceType.O4.Id, R.O4Piece},
            
            {PieceType.A1.Id, R.A1Piece},
            {PieceType.A2.Id, R.A2Piece},
            {PieceType.A3.Id, R.A3Piece},
            {PieceType.A4.Id, R.A4Piece},
            {PieceType.A5.Id, R.A5Piece},
            {PieceType.A6.Id, R.A6Piece},
            {PieceType.A7.Id, R.A7Piece},
            {PieceType.A8.Id, R.A8Piece},

            {PieceType.B1.Id, R.B1Piece},
            {PieceType.B2.Id, R.B2Piece},
            {PieceType.B3.Id, R.B3Piece},
            {PieceType.B4.Id, R.B4Piece},
            {PieceType.B5.Id, R.B5Piece},

            {PieceType.Gbox1.Id, R.GBox1Piece},
            
            {PieceType.Castle1.Id, R.Castle1Piece},
            {PieceType.Castle2.Id, R.Castle2Piece},
            {PieceType.Castle3.Id, R.Castle3Piece},
            {PieceType.Castle4.Id, R.Castle4Piece},
            {PieceType.Castle5.Id, R.Castle5Piece},
            {PieceType.Castle6.Id, R.Castle6Piece},
            {PieceType.Castle7.Id, R.Castle7Piece},
            {PieceType.Castle8.Id, R.Castle8Piece},
            {PieceType.Castle9.Id, R.Castle9Piece},

            {PieceType.Tavern1.Id, R.Tavern1Piece},
            {PieceType.Tavern2.Id, R.Tavern2Piece},
            {PieceType.Tavern3.Id, R.Tavern3Piece},
            {PieceType.Tavern4.Id, R.Tavern4Piece},
            {PieceType.Tavern5.Id, R.Tavern5Piece},
            {PieceType.Tavern6.Id, R.Tavern6Piece},
            {PieceType.Tavern7.Id, R.Tavern7Piece},
            {PieceType.Tavern8.Id, R.Tavern8Piece},
            {PieceType.Tavern9.Id, R.Tavern9Piece},
            
            {PieceType.Chest1.Id, R.Chest1Piece},
            {PieceType.Chest2.Id, R.Chest2Piece},
            {PieceType.Chest3.Id, R.Chest3Piece},

            {-1000, R.HitboxDamageView},
            {-1100, R.AddResourceView},
            {-1200, R.HintArrow},
            {-1300, R.AddCards}
        };
        
        return dict;
    }
}