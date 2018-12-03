[TaskHighlight(typeof(HighlightTaskCurrencyCollect))]
public class TaskCurrencyCollectEntity : TaskCurrencyEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

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