using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Base task for currencies 
/// </summary>
public abstract class TaskCurrencyEntity : TaskCounterEntity, IHavePieceId
{
    public string CurrencyName { get; protected set; }
    
    public int PieceId => CurrencyName != null ? PieceType.Parse(CurrencyName) : PieceType.None.Id;
    
#region Serialization

    // Use PieceUid name to unify names across the tasks to simplify Google Drive export
    [JsonProperty] private string PieceUid;
    
    public bool ShouldSerializePieceId()
    {
        return false;
    }    
    
    public bool ShouldSerializePieceUid()
    {
        return false;
    }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        if (string.IsNullOrEmpty(PieceUid))
        {
            return;
        }
        
        CurrencyName = PieceUid;
    }
    
    public bool ShouldSerializeCurrency()
    {
        return false;
    }
    
#endregion
    
    public override void ConnectToBoard()
    {
        ShopService.Current.OnPurchasedEvent += OnPurchasedEventHandler;
    }

    public override void DisconnectFromBoard()
    {
        ShopService.Current.OnPurchasedEvent -= OnPurchasedEventHandler;
    }

    protected abstract void OnPurchasedEventHandler(IPurchaseableItem purchaseableItem, IShopItem shopItem);

    public override string ToString()
    {
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: {CurrencyName} - {CurrentValue}/{TargetValue}";
        return ret;
    }
}