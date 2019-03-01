using System.Collections.Generic;
using UnityEngine;

public class FogDef
{
    public string Uid { get; set; }
    public int Level { get; set; }
    
    public string Hero { get; set; }

    private int heroId = PieceType.None.Id;
    public int HeroId
    {
        get
        {
            if (string.IsNullOrEmpty(Hero) == false && heroId == PieceType.None.Id) heroId = PieceType.Parse(Hero);
            
            return heroId;
        }
    }

    public bool IsActive { get; set; }

    public List<BoardPosition> Positions { get; set; }
    public CurrencyPair Condition { get; set; }
    public CurrencyPair Reward { get; set; }
    public Dictionary<string, List<BoardPosition>> Pieces { get; set; }
    
    public BoardPosition GetCenter()
    {
        return BoardPosition.GetCenter(Positions);
    }
    
    public Vector3 GetCenter(BoardController board)
    {
        var center = BoardPosition.GetCenter(Positions);
        return board.BoardDef.GetSectorCenterWorldPosition(center.X, center.Y, 0);
    }
}