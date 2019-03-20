using System.Collections.Generic;

public class WorkerTutorialStep1 : BaseTutorialStep
{
    private List<BoardPosition> positions = new List<BoardPosition>();
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceType.NPC_Gnome.Id);

        if (positions.Count == 0 && ProfileService.Current.GetStorageItem(Currency.Worker.Name).Amount == 0)
        {
            CurrencyHelper.PurchaseAsync(Currency.Worker.Name, 2).Complete();
        }
        
        base.Perform();
    }

    protected override void Complete()
    {
        foreach (var position in positions)
        {
            var from = Context.Context.BoardDef.GetPiecePosition(position.X, position.Y);
            var fly = Context.Context.BoardDef.ViewCamera.WorldToScreenPoint(from);

            CurrencyHelper.PurchaseAsync(Currency.Worker.Name, 1, null, fly).Complete();
        }
        
        base.Complete();
    }
}
