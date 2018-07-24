using System.Collections.Generic;
using UnityEngine;

public class PiecesDataManager : IECSComponent, IDataManager, IDataLoader<List<PieceDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid { get { return ComponentGuid; } }
	
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
    public BoardPosition MatketPosition = BoardPosition.Default();
    public BoardPosition StoragePosition = BoardPosition.Default();
    public BoardPosition SawmillPosition = BoardPosition.Default();
    public BoardPosition MinePosition = BoardPosition.Default();
    public BoardPosition SheepfoldPosition = BoardPosition.Default();
    public BoardPosition KingPosition = BoardPosition.Default();
    
    private Dictionary<int, PieceDef> pieces;
    
    public void Reload()
    {
        CastlePosition = BoardPosition.Default();
        MatketPosition = BoardPosition.Default();
        StoragePosition = BoardPosition.Default();
        SawmillPosition = BoardPosition.Default();
        MinePosition = BoardPosition.Default();
        SheepfoldPosition = BoardPosition.Default();
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
                    if (pieces.ContainsKey(def.Piece)) continue;
                
                    pieces.Add(def.Piece, def);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
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
            MatketPosition = position;
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