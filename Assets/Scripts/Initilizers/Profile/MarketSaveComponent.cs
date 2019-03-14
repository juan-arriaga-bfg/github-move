using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class MarketSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public string ResetMarketStartTime;
    [JsonProperty] public string ResetEnergyStartTime;
    [JsonProperty] public string OfferTimerStartTime;
    
    private List<MarketSaveItem> slots;

    [JsonProperty]
    public List<MarketSaveItem> Slots
    {
        get { return slots; }
        set { slots = value; }
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (BoardService.Current == null) return;
        
        var board = BoardService.Current.FirstBoard;
        var defs = GameDataService.Current.MarketManager.Defs;
        
        ResetMarketStartTime = board.MarketLogic.ResetMarketTimer.StartTimeLong.ToString();

        if (board.MarketLogic.OfferTimer.IsExecuteable()) OfferTimerStartTime = board.MarketLogic.OfferTimer.StartTimeLong.ToString();
        if (board.MarketLogic.ResetEnergyTimer.IsExecuteable()) ResetEnergyStartTime = board.MarketLogic.ResetEnergyTimer.StartTimeLong.ToString();
        
        slots = new List<MarketSaveItem>();

        for (var i = 0; i < defs.Count; i++)
        {
            var def = defs[i];
            
            if(def.Reward == null) continue;
            
            slots.Add(new MarketSaveItem
            {
                Index = i,
                ItemIndex = def.Index,
                State = def.State,
                Piece = PieceType.Parse(def.Reward.Currency),
                Amount = def.Reward.Amount
            });
        }
    }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
    }
}