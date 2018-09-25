using Newtonsoft.Json;

public class TaskCurrencyCollectEntity : TaskCounterEntity, IConnectedToBoardEvent
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

        if (shopItem.ItemUid == Currency)
        {
            CurrentValue += shopItem.Amount;
        }
    }
    
    public override string ToString()
    {
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: {Currency} - {CurrentValue}/{TargetValue}";
        return ret;
    }
}