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
                Chests = new List<ChestDef> {data[0]};
                
                for (var i = 1; i < data.Count; i++)
                {
                    var def = data[i - 1];
                    var defNext = data[i];
                    
                    defNext.PieceWeights = ItemWeight.ReplaseWeights(def.PieceWeights, defNext.PieceWeights);
                    
                    Chests.Add(defNext);
                }

                var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
                
                if(save == null || save.Chests == null) return;

                foreach (var item in save.Chests)
                {
                    var chest = GetChest(item.Id);
                    
                    chest.State = item.State;
                    chest.SetStartTime(item.StartTime);
                    
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