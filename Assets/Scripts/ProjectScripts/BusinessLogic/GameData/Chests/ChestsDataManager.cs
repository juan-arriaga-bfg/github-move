using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : ECSEntity, IDataManager, IDataLoader<List<ChestDef>>, ISequenceData
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
    
    public void Reload()
    {
        ActiveChest = null;
        Chests = null;
        ChestsOnBoard = new Dictionary<BoardPosition, Chest>();

        ReloadSequences();
        
        LoadData(new ResourceConfigDataMapper<List<ChestDef>>("configs/chests.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public List<RandomSaveItem> GetSaveSequences()
    {
        var save = new List<RandomSaveItem>();

        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);

        foreach (SequenceComponent component in collection.Components)
        {
            save.Add(component.Save());
        }
        
        return save;
    }

    public void ReloadSequences()
    {
        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);

        if (collection?.Components == null) return;
        
        var components = new List<IECSComponent>(collection.Components);
            
        foreach (var component in components)
        {
            UnRegisterComponent(component);
        }
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
                    
                    var sequenceData = new SequenceComponent{Key = next.Uid};
                    
                    sequenceData.Init(next.PieceWeights);
                    RegisterComponent(sequenceData, true);
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
    
    public Chest GetChest(int pieceType)
    {
        var chestDef = Chests.Find(def => def.Piece == pieceType);
        
        return chestDef == null ? null : new Chest(chestDef);
    }

    public SequenceComponent GetSequence(string uid)
    {
        var collection = GetComponent<ECSComponentCollection>(SequenceComponent.ComponentGuid);
        return (SequenceComponent) collection.Components.Find(component => (component as SequenceComponent).Key == uid);
    }
    
    public bool AddToBoard(BoardPosition position, int pieceType, bool isOpen = false)
    {
        if (ChestsOnBoard.ContainsKey(position)) return false;

        var chest = GetChest(pieceType);

        if (isOpen) chest.State = ChestState.Open;
        
        ChestsOnBoard.Add(position, chest);
        
        return true;
    }
    
    public Chest GetFromBoard(BoardPosition position, int pieceType)
    {
        Chest chest;

        if (ChestsOnBoard.TryGetValue(position, out chest) == false) return GetChest(pieceType);
        
        ChestsOnBoard.Remove(position);
        return chest;
    }
}