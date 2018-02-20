using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPieceBuilder
{
    Piece Build(int pieceType, BoardController context);
}
