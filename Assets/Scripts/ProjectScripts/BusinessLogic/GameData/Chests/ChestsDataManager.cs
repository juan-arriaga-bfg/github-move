using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : SequenceData, IDataLoader<List<ChestDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public List<ChestDef> Chests;
    
    public override void Reload()
    {
        base.Reload();
        
        Chests = null;
        
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
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public Chest GetChest(int pieceType)
    {
        var chestDef = Chests.Find(def => def.Piece == pieceType);
        
        return chestDef == null ? null : new Chest {Def = chestDef};
    }
}