using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestDef
{
    private int pieceType = -1;
    private int hardPieceAmount = -1;
    
    private Dictionary<int, int> hardPieces;
    
    public string Uid { get; set; }
    public int Time { get; set; }
    public CurrencyPair Price { get; set; }
    public int PieceAmount { get; set; }

    public int HardPieceAmount => hardPieceAmount != -1 ? hardPieceAmount : (hardPieceAmount = GetHardPieces().Values.Sum());

    public List<ItemWeight> PieceWeights;
    public List<string> RandomPieces;
    public List<CurrencyPair> HardPieces { get; set; }

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

    public int GetRandomPiece()
    {
        if (RandomPieces == null || RandomPieces.Count == 0) return PieceType.None.Id;

        var id = RandomPieces[Random.Range(0, RandomPieces.Count)];

        return PieceType.Parse(id);
    }

    public Dictionary<int, int> GetHardPieces()
    {
        if (hardPieces != null) return hardPieces;
        
        hardPieces = new Dictionary<int, int>();
        
        if(HardPieces == null) return hardPieces;

        foreach (var hard in HardPieces)
        {
            hardPieces.Add(PieceType.Parse(hard.Currency), hard.Amount);
        }
        
        return hardPieces;
    }
}