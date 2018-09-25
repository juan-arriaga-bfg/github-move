using Newtonsoft.Json;

public class TaskCurrencySpendEntity : TaskCounterEntity, IConnectedToBoardEvent  
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public string Currency { get; protected set; }
    
    public void ConnectToBoard()
    {
        ShopService.Current.OnPurchasedEvent += OnPurchasedEventHandler;
    }
    
    public void DisconnectFromBoard()
    {
        ShopService.Current.OnPurchasedEvent -= OnPurchasedEventHandler;
    }

    private void OnPurchasedEventHandler(IPurchaseableItem purchaseableItem, IShopItem shopItem)
    {
        if (!IsInProgress())
        {
            return;
        }

        foreach (var price in shopItem.CurrentPrices)
        {
            if (price.Currency == Currency)
            {
                CurrentValue += price.TargetPriceAmount;
            }
        }
    }
    
    public override string ToString()
    {
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: {Currency} - {CurrentValue}/{TargetValue}";
        return ret;
    }
}