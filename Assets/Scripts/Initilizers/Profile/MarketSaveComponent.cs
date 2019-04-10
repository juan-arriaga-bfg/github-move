using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public interface IMarketSaveComponent
{
    MarketSaveComponent MarketSave { get; }
}

public partial class UserProfile : IMarketSaveComponent
{
    protected MarketSaveComponent marketSaveComponent;

    [JsonIgnore]
    public MarketSaveComponent MarketSave => marketSaveComponent ?? (marketSaveComponent = GetComponent<MarketSaveComponent>(MarketSaveComponent.ComponentGuid));
}

[JsonObject(MemberSerialization.OptIn)]
public class MarketSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public string ResetMarketStartTime;
    [JsonProperty] public long FreeEnergyClaimTime;
    [JsonProperty] public string OfferTimerStartTime;
    [JsonProperty] public bool FirstEnergyClaimed;
    
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
        OfferTimerStartTime = board.MarketLogic.OfferTimer.IsExecuteable() ? board.MarketLogic.OfferTimer.StartTimeLong.ToString() : string.Empty;
        FreeEnergyClaimTime = UnixTimeHelper.DateTimeToUnixTimestamp(board.MarketLogic.FreeEnergyClaimTime);
        FirstEnergyClaimed = board.MarketLogic.FirstFreeEnergyClaimed;
            
        slots = new List<MarketSaveItem>();

        foreach (var def in defs)
        {
            if(def.Reward == null) continue;
            
            slots.Add(new MarketSaveItem
            {
                Index = def.Uid,
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