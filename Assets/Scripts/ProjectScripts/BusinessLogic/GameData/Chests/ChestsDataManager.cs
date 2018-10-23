using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : SequenceData, IDataLoader<List<ChestDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public Chest ActiveChest;
    public List<ChestDef> Chests;
    
    public Dictionary<BoardPosition, Chest> ChestsOnBoard = new Dictionary<BoardPosition, Chest>();
    
    public override void Reload()
    {
        base.Reload();
        
        ActiveChest = null;
        Chests = null;
        ChestsOnBoard = new Dictionary<BoardPosition, Chest>();
        
        LoadData(new ResourceConfigDataMapper<List<ChestDef>>("configs/chests.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<ChestDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                var matchDefinition = new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());
                data.Sort((a, b) => a.Piece.CompareTo(b.Piece));

                Chests = new List<ChestDef>();
                
                foreach (var next in data)
                {
                    var previousType = matchDefinition.GetPrevious(next.Piece);
                    
                    if (previousType != PieceType.None.Id)
                    {
                        var previous = data.Find(def => def.Piece == previousType);
                        
                        next.PieceWeights = ItemWeight.ReplaseWeights(previous.PieceWeights, next.PieceWeights);
                    }
                    
                    Chests.Add(next);
                    AddSequence(next.Uid, next.PieceWeights);
                }

                var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
                
                if(save?.Chests == null) return;

                foreach (var item in save.Chests)
                {
                    var chest = GetChest(item.Id);
                    
                    chest.Reward = item.Reward;
                    ChestsOnBoard.Add(item.Position, chest);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    private Chest GetChest(int pieceType)
    {
        var chestDef = Chests.Find(def => def.Piece == pieceType);
        
        return chestDef == null ? null : new Chest(chestDef);
    }
    
    public Chest GetFromBoard(BoardPosition position, int pieceType)
    {
        Chest chest;

        if (ChestsOnBoard.TryGetValue(position, out chest) == false) return GetChest(pieceType);
        
        ChestsOnBoard.Remove(position);
        return chest;
    }
}