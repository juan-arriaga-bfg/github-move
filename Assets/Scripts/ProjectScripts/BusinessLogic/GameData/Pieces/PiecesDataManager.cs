using System.Collections.Generic;
using UnityEngine;

public class PiecesDataManager : IDataLoader<List<PieceDef>>
{
    public const int ReproductionDelay = 7;
    public const int ReproductionStepDelay = 5;
    public const int ReproductionChance = 25;
    
    public BoardPosition CastlePosition = BoardPosition.Default();
    public BoardPosition TavernPosition = BoardPosition.Default();
    public BoardPosition StoragePosition = BoardPosition.Default();
    public BoardPosition SawmillPosition = BoardPosition.Default();
    public BoardPosition MinePosition = BoardPosition.Default();
    public BoardPosition SheepfoldPosition = BoardPosition.Default();
    public BoardPosition KingPosition = BoardPosition.Default();
    
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
        var first = piece.Context.BoardLogic.MatchDefinition.GetFirst(piece.PieceType);
        
        if (first == PieceType.Market1.Id)
        {
            TavernPosition = position.Right;
            return;
        }
        
        if (first == PieceType.Storage1.Id)
        {
            StoragePosition = position.Right;
            return;
        }
        
        if (first == PieceType.Castle1.Id)
        {
            CastlePosition = position.Up.Right;
            return;
        }
        
        if (first == PieceType.Sawmill1.Id)
        {
            SawmillPosition = position.Right;
            return;
        }
        
        if (first == PieceType.Mine1.Id)
        {
            MinePosition = position;
            return;
        }
        
        if (first == PieceType.Sheepfold1.Id)
        {
            SheepfoldPosition = position;
            return;
        }
        
        if (first == PieceType.King.Id)
        {
            KingPosition = position;
            return;
        }
    }
}