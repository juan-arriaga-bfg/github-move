using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPieceBoardEvent : BoardEvent
{
    public int PieceType { get; set; }
	
    public BoardPosition Position { get; set; }

    public bool IsCombo { get; set; }

    public Piece PieceDef { get; set; }
}
