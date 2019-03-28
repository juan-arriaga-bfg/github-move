using System.Collections.Generic;
using Newtonsoft.Json;
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

    /// <summary>
    /// WARNING! Do not use directly, always use GetCenter() instead
    /// This filed purpose is serialization only
    /// </summary>
    [JsonProperty]
    private BoardPosition Center = BoardPosition.Default();
    
    public BoardPosition GetCenter()
    {
        if (!IsActive && Center.Equals(BoardPosition.Default()))
        {
            return BoardPosition.GetCenter(Positions); 
        }

        return Center;
    }
    
    public Vector3 GetCenter(BoardController board)
    {
        var center = GetCenter();
        return board.BoardDef.GetSectorCenterWorldPosition(center.X, center.Y, 0);
    }
}