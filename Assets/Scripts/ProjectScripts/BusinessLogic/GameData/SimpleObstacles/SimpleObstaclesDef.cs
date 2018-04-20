using System.Collections.Generic;

public class SimpleObstaclesDef
{ 
    private int pieceType = -1;
    
    public string Uid { get; set; }

    public List<ItemWeight> Weights;
    
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

        foreach (var weight in Weights)
        {
            str += string.Format("\n{0}", weight);
        }

        return str;
    }
}