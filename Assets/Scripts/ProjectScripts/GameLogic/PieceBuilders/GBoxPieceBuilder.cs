using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBoxPieceBuilder : GenericPieceBuilder 
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
        
		
		return piece;
	}
}