[TaskHighlight(typeof(HighlightTaskPointToPiece))]
[TaskHighlight(typeof(HighlightTaskFindProductionFieldForPieceType))]
[TaskHighlight(typeof(HighlightTaskPointToPredecessor))]
[TaskHighlight(typeof(HighlightTaskPointToRandomChest))]
[TaskHighlight(typeof(HighlightTaskFindObstacleForPieceType))]
[TaskHighlight(typeof(HighlightTaskFirstMineOfAnyType))]
[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskCollectIngredientEntity : TaskCurrencyCollectEntity
{
    public new static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    protected override void OnPurchasedEventHandler(IPurchaseableItem purchaseableItem, IShopItem shopItem)
    {
        if (!IsInProgress())
        {
            return;
        }

        var pieceType = PieceType.Parse(shopItem.ItemUid);
        if (pieceType == PieceType.None.Id)
        {
            return;
        }
        
        var pieceDef = PieceType.GetDefById(pieceType);
        if (!pieceDef.Filter.Has(PieceTypeFilter.Resource))
        {
            return;
        }
        
        
        if (shopItem.ItemUid == CurrencyName || string.IsNullOrEmpty(CurrencyName))
        {
            CurrentValue += shopItem.Amount;
        }
    }
}