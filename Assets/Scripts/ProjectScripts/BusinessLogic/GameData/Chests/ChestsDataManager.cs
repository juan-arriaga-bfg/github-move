using Debug = IW.Logger;
using System.Collections.Generic;

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
                data.Sort((a, b) => a.Piece.CompareTo(b.Piece));

                Chests = new List<ChestDef>();

                var dataManager = (GameDataManager) context;
                
                foreach (var next in data)
                {
                    var previousType = dataManager.MatchDefinition.GetPrevious(next.Piece);
                    
                    if (previousType != PieceType.None.Id)
                    {
                        var previous = data.Find(def => def.Piece == previousType);
                        
                        next.PieceWeights = ItemWeight.ReplaceWeights(previous.PieceWeights, next.PieceWeights);
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
    
    public ChestDef GetChest(int pieceType)
    {
        return Chests.Find(def => def.Piece == pieceType);
    }
}