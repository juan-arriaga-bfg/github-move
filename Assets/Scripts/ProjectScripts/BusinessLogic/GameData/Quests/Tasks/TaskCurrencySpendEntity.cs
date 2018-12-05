[TaskHighlight(typeof(HighlightTaskCurrencySpend))]
public class TaskCurrencySpendEntity : TaskCurrencyEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override void OnPurchasedEventHandler(IPurchaseableItem purchaseableItem, IShopItem shopItem)
    {
        if (!IsInProgress())
        {
            return;
        }

        foreach (var price in shopItem.CurrentPrices)
        {
            if (price.Currency == CurrencyName)
            {
                CurrentValue += price.TargetPriceAmount;
            }
        }
    }
}