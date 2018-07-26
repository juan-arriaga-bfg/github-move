using System.Collections.Generic;
using UnityEngine;

public class ChestsDataManager : IECSComponent, IDataManager, IDataLoader<List<ChestDef>>
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
    
    public Chest ActiveChest;
    public List<ChestDef> Chests;
    
    public Dictionary<BoardPosition, Chest> ChestsOnBoard = new Dictionary<BoardPosition, Chest>();
    
    public void Reload()
    {
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
                }

                var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
                
                if(save == null || save.Chests == null) return;

                foreach (var item in save.Chests)
                {
                    var chest = GetChest(item.Id);
                    
                    chest.State = item.State;
                    chest.SetStartTime(item.StartTime);
                    chest.Reward = item.Reward;
                    
                    ChestsOnBoard.Add(item.Position, chest);

                    if (chest.State == ChestState.InProgress) ActiveChest = chest;
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

        if (ChestsOnBoard.TryGetValue(position, out chest))
        {
            ChestsOnBoard.Remove(position);
            return chest;
        }
        
        return GetChest(pieceType);
    }
}