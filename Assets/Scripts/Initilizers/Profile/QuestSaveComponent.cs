using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class QuestSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid
    {
        get { return ComponentGuid; }
    }

    private List<QuestSaveItem> castle;
    private List<QuestSaveItem> active;
    
    private List<TaskSaveItem> market;
    
    private List<int> completed;
    
    [JsonProperty]
    public List<QuestSaveItem> Castle
    {
        get { return castle; }
        set { castle = value; }
    }
    
    [JsonProperty]
    public List<QuestSaveItem> Active
    {
        get { return active; }
        set { active = value; }
    }
    
    [JsonProperty]
    public List<TaskSaveItem> Market
    {
        get { return market; }
        set { market = value; }
    }
    
    [JsonProperty]
    public List<int> Completed
    {
        get { return completed; }
        set { completed = value; }
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if(GameDataService.Current == null || BoardService.Current == null) return;
        
        SaveQuest();
        SaveCastle();
        SaveMarket();
    }
    
    private void SaveQuest()
    {
        var manager = GameDataService.Current.QuestsManager;
        
        completed = manager.SaveCompleted();
        completed.Sort((a, b) => a.CompareTo(b));
        
        active = new List<QuestSaveItem>();
        
        foreach (var quest in manager.ActiveQuests)
        {
            active.Add(new QuestSaveItem{Uid = quest.Def.Uid, Progress = GetSaveCount(quest.WantedPiece, quest.CurrentAmount)});
        }
        
        active.Sort((a, b) => a.Uid.CompareTo(b.Uid));
    }

    private void SaveCastle()
    {
        var board = BoardService.Current.GetBoardById(0);
        var piece = board.BoardLogic.GetPieceAt(GameDataService.Current.PiecesManager.CastlePosition);
        
        if(piece == null) return;

        var upgrade = piece.GetComponent<CastleUpgradeComponent>(CastleUpgradeComponent.ComponentGuid);
        
        if(upgrade == null) return;
        
        castle = new List<QuestSaveItem>();
        
        foreach (var price in upgrade.Prices)
        {
            castle.Add(new QuestSaveItem{Uid = price.Def.Uid, Progress = GetSaveCount(price.WantedPiece, price.CurrentAmount)});
        }
    }

    private void SaveMarket()
    {
        var manager = GameDataService.Current.TasksManager;
        
        market = new List<TaskSaveItem>();

        foreach (var task in manager.Tasks)
        {
            market.Add(new TaskSaveItem{Result = task.Result.Amount, Prices = task.Prices, Rewards = task.Rewards});
        }
    }

    private int GetSaveCount(int id, int pogress)
    {
        var board = BoardService.Current.GetBoardById(0);
        var cash = board.BoardLogic.PositionsCache;
        
        return cash == null ? 0 : Mathf.Max(0, pogress - cash.GetCountByType(id));
    }
}