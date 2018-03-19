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

            {PieceType.S1.Id, R.S1Piece},
            {PieceType.S2.Id, R.S2Piece},
            {PieceType.S3.Id, R.S3Piece},
            {PieceType.S4.Id, R.S4Piece},
            {PieceType.S5.Id, R.S5Piece},
            {PieceType.S6.Id, R.S6Piece},
            {PieceType.S7.Id, R.S7Piece},

            {PieceType.H1.Id, R.H1Piece},
            {PieceType.H2.Id, R.H2Piece},

            {PieceType.E1.Id, R.E1Piece},
            {PieceType.E2.Id, R.E2Piece},
            {PieceType.E3.Id, R.E3Piece},

            {PieceType.O1.Id, R.O1Piece},
            {PieceType.O2.Id, R.O2Piece},

            {PieceType.C1.Id, R.C1Piece},

            {PieceType.A1.Id, R.A1Piece},
            {PieceType.A2.Id, R.A2Piece},
            {PieceType.A3.Id, R.A3Piece},
            {PieceType.A4.Id, R.A4Piece},
            {PieceType.A5.Id, R.A5Piece},
            {PieceType.A6.Id, R.A6Piece},
            {PieceType.A7.Id, R.A7Piece},

            {PieceType.B1.Id, R.B1Piece},
            {PieceType.B2.Id, R.B2Piece},
            {PieceType.B3.Id, R.B3Piece},
            {PieceType.B4.Id, R.B4Piece},
            {PieceType.B5.Id, R.B5Piece},

            {PieceType.Gbox1.Id, R.GBox1Piece},

            {-1000, R.HitboxDamageView}
        };
        
        return dict;
    }
}