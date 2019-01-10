using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class MarketSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public string ResetMarketStartTime;
    
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
        var board = BoardService.Current.FirstBoard;
        var defs = GameDataService.Current.MarketManager.Defs;
        
        ResetMarketStartTime = board.MarketLogic.Timer.StartTimeLong.ToString();
        
        slots = new List<MarketSaveItem>();

        for (var i = 0; i < defs.Count; i++)
        {
            var def = defs[i];
            
            slots.Add(new MarketSaveItem
            {
                Index = i,
                ItemIndex = def.Index,
                IsPurchased = def.IsPurchased,
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