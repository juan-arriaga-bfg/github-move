[TaskHighlight(typeof(HighlightTaskNotImplemented))]
public class TaskCurrencyCollectEntity : TaskCurrencyEntity, IHavePieceId
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public int PieceId => PieceType.Parse(Currency);

    protected override void OnPurchasedEventHandler(IPurchaseableItem purchaseableItem, IShopItem shopItem)
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
}