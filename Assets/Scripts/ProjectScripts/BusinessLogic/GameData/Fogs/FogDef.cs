using System.Collections.Generic;

public class FogDef
{
    public int Uid { get; set; }
    public BoardPosition Position { get; set; }
    public HeroAbility Condition { get; set; }
    public Dictionary<string, BoardPosition> Pieces { get; set; }

    public override string ToString()
    {
        var str = string.Format("Uid: {0}, Position: {1}\nPieces: ", Uid, Position);

        foreach (var piece in Pieces)
        {
            str += string.Format("id: {0}, Position: {1}\n", piece.Key, piece.Value);
        }
        
        return str;
    }
}