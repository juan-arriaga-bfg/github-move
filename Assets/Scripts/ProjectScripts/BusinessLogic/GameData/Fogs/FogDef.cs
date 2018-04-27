using System.Collections.Generic;

public class FogDef
{
    public int Uid { get; set; }
    public BoardPosition Position { get; set; }
    public HeroAbility Condition { get; set; }
    public BoardPosition Size { get; set; }
    public Dictionary<string, List<BoardPosition>> Pieces { get; set; }
    public List<ItemWeight> PieceWeights { get; set; }
}