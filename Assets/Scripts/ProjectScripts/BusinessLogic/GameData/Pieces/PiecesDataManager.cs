using System.Collections.Generic;
using UnityEngine;

public class PiecesDataManager : IECSComponent, IDataManager, IDataLoader<List<PieceDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public const int ReproductionDelay = 20;
    public const int ReproductionStepDelay = 5;
    public const int ReproductionChance = 50;
    
    public BoardPosition CastlePosition = BoardPosition.Default();
    public BoardPosition KingPosition = BoardPosition.Default();
    
    private Dictionary<int, PieceDef> pieces;
    
    public void Reload()
    {
        CastlePosition = BoardPosition.Default();
        KingPosition = BoardPosition.Default();

        pieces = null;
        
        LoadData(new ResourceConfigDataMapper<List<PieceDef>>("configs/pieces.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadData(IDataMapper<List<PieceDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                pieces = new Dictionary<int, PieceDef>();
                
                foreach (var def in data)
                {
                    if (pieces.ContainsKey(def.Piece))
                    {
                        continue;
                    }

                    AssignFilters(def);
                    
                    pieces.Add(def.Piece, def);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    private void AssignFilters(PieceDef pieceDef)
    {
        PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceDef.Piece);

        if (pieceDef.Reproduction != null)
        {
            pieceTypeDef.Filter = pieceTypeDef.Filter.Add(PieceTypeFilter.Reproduction);
            // Debug.Log($"Add Reproduction filter to {pieceTypeDef.Abbreviations[0]}");
        }
        
        if (pieceDef.SpawnResources != null && pieceDef.SpawnResources.Currency == Currency.Energy.Name)
        {
            pieceTypeDef.Filter = pieceTypeDef.Filter.Add(PieceTypeFilter.Energy);
            // Debug.Log($"Add Energy filter to {pieceTypeDef.Abbreviations[0]}");
        }
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
        
        if (first == PieceType.Castle1.Id)
        {
            CastlePosition = position;
            return;
        }
        
        if (first == PieceType.King.Id)
        {
            KingPosition = position;
            return;
        }
    }
}