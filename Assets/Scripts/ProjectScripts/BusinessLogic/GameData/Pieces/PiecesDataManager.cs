using System.Collections.Generic;
using UnityEngine;

public class PiecesDataManager : IDataLoader<List<PieceDef>>
{
    public BoardPosition CastlePosition { get; set; }
    public BoardPosition TavernPosition { get; set; }
    public BoardPosition SawmillPosition { get; set; }
    
    private Dictionary<int, PieceDef> pieces;
    
    public void LoadData(IDataMapper<List<PieceDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                pieces = new Dictionary<int, PieceDef>();
                
                foreach (var def in data)
                {
                    if (pieces.ContainsKey(def.Piece)) continue;
                
                    pieces.Add(def.Piece, def);
                }
            }
            else
            {
                Debug.LogWarning("[PiecesDataManager]: pieces config not loaded");
            }
        });
    }
    
    public PieceDef GetPieceDef(int id)
    {
        PieceDef def;

        return pieces.TryGetValue(id, out def) ? def : null;
    }

    public PieceDef GetPieceDefOrDefault(int id)
    {
        PieceDef def;
        
        return pieces.TryGetValue(id, out def) ? def : PieceDef.Default();
    }

    public void CachedPosition(Piece piece, BoardPosition position)
    {
        if (piece.PieceType >= PieceType.Tavern1.Id && piece.PieceType <= PieceType.Tavern9.Id)
        {
            TavernPosition = position.Right;
            return;
        }
        
        if (piece.PieceType >= PieceType.Castle1.Id && piece.PieceType <= PieceType.Castle9.Id)
        {
            CastlePosition = position.Up.Right;
            return;
        }
        
        if (piece.PieceType >= PieceType.Sawmill1.Id && piece.PieceType <= PieceType.Sawmill7.Id)
        {
            SawmillPosition = position;
            return;
        }
    }
}