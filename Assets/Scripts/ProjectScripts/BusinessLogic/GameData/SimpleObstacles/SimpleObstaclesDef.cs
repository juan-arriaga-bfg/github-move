using System.Collections.Generic;

public class SimpleObstaclesDef
{ 
    private int pieceType = -1;
    
    public string Uid { get; set; }

    public List<int> PieceAmounts { get; set; }
    public Dictionary<int, List<ItemWeight>> PieceWeights { get; set; }
    public List<ItemWeight> ChestWeights { get; set; }
    
    public int Piece
    {
        get
        {
            if (pieceType == -1)
            {
                pieceType = PieceType.Parse(Uid);
            }
            
            return pieceType;
        }
    }
    
    public override string ToString()
    {
        var str = string.Format("Uid: {0}, Piece: {1}", Uid, Piece);

        foreach (var weight in ChestWeights)
        {
            str += string.Format("\n{0}", weight);
        }

        return str;
    }
}