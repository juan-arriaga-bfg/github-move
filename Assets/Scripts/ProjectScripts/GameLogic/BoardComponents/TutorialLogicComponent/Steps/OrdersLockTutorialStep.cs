﻿public class OrdersLockTutorialStep : BaseTutorialStep
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
        var position = positions[0];
        var customer = logic.GetPieceAt(position)?.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
        
        customer?.Cooldown.Complete();
        
        var centerPos = Context.Context.BoardDef.GetPiecePosition(position.X, position.Y);
        if (Context.Context.Manipulator.CameraManipulator.CameraMove.IsLocked == false) Context.Context.Manipulator.CameraManipulator.MoveTo(centerPos);
        
        base.Complete();
    }
}