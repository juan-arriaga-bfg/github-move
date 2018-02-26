using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawmillPieceObserver : IPieceBoardObserver
{
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        throw new System.NotImplementedException();
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        throw new System.NotImplementedException();
    }
}