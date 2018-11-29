public class OrdersLockTutorialStep : BaseTutorialStep
{
    public override void OnRegisterEntity(ECSEntity entity)
    {
        GameDataService.Current.OrdersManager.Locker.Lock(this);
        base.OnRegisterEntity(entity);
    }

    protected override void Complete()
    {
        GameDataService.Current.OrdersManager.Locker.Unlock(this);
        
        var logic = BoardService.Current.FirstBoard.BoardLogic;
        var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Character);
        var customer = logic.GetPieceAt(positions[0])?.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
        
        customer?.Cooldown.Complete();
        
        base.Complete();
    }
}