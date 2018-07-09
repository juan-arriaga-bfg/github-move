using System.Collections.Generic;
using UnityEngine;

public class FogDef
{
    public int Uid { get; set; }
    public BoardPosition Position { get; set; }
    public CurrencyPair Condition { get; set; }
    public BoardPosition Size { get; set; }
    public Dictionary<string, List<BoardPosition>> Pieces { get; set; }
    public List<ItemWeight> PieceWeights { get; set; }

    public Vector3 GetCenter(BoardController board)
    {
        var max = new BoardPosition(Position.RightAtDistance(Size.X - 1).X, Position.UpAtDistance(Size.Y - 1).Y);
		
        var minPos = board.BoardDef.GetSectorCenterWorldPosition(Position.X, Position.Y, 0);
        var maxPos = board.BoardDef.GetSectorCenterWorldPosition(max.X, max.Y, 0);

        return (maxPos + minPos) / 2;
    }
}