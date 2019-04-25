[TaskHighlight(typeof(HighlightTaskPointToPiece))]
[TaskHighlight(typeof(HighlightTaskPointToRandomIngredient),          new[] {typeof(HighlightConditionPieceIdNotSpecified)})]
[TaskHighlight(typeof(HighlightTaskPointToRandomLiveProductionField), new[] {typeof(HighlightConditionPieceIdNotSpecified)})]
[TaskHighlight(typeof(HighlightTaskPointToRandomDeadProductionField), new[] {typeof(HighlightConditionPieceIdNotSpecified)})]
[TaskHighlight(typeof(HighlightTaskFindProductionFieldForPieceType))]
[TaskHighlight(typeof(HighlightTaskFindDeadProductionFieldForPieceType))]
[TaskHighlight(typeof(HighlightTaskPointToPredecessor))]
[TaskHighlight(typeof(HighlightTaskAnyBag))]
[TaskHighlight(typeof(HighlightTaskPointToRandomChestExcludingFreeAndNpc))]
[TaskHighlight(typeof(HighlightTaskPointToRandomBranchATree))]
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