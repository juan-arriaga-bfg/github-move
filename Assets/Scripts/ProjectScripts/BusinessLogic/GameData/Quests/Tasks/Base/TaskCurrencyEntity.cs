using Newtonsoft.Json;

public abstract class TaskCurrencyEntity : TaskCounterEntity
{
    [JsonProperty] public string Currency { get; protected set; }
    
#region Serialization

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
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: {Currency} - {CurrentValue}/{TargetValue}";
        return ret;
    }
}