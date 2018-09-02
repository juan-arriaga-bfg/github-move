public class PieceFlyerComponent : ECSEntity, ILockerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private BoardLogicComponent context;
    
    private LockerComponent locker;
    public virtual LockerComponent Locker => locker ?? (locker = GetComponent<LockerComponent>(LockerComponent.ComponentGuid));
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardLogicComponent;
    }
    
    public void FlyToQuest(Piece piece)
    {
        if (piece.PieceState != null && piece.PieceState.State != BuildingState.Complete
            || Locker.IsLocked
            || GameDataService.Current.QuestsManager.IsNeedToFly(piece.PieceType) == false) return;
        
        var flay = ResourcesViewManager.Instance.GetFirstViewById(PieceType.Parse(piece.PieceType));
        flay?.UpdateResource(1);
    }
    
    public void FlyTo(Piece piece, int x, int y, string target)
    {
        if(piece.PieceState != null && piece.PieceState.State != BuildingState.Complete || Locker.IsLocked) return;
        
        var currency = PieceType.Parse(piece.PieceType);
        var flay = ResourcesViewManager.Instance.GetFirstViewById(target);
        
        if (flay == null) return;
        
        var from = context.Context.BoardDef.GetPiecePosition(x, y);
        
        var carriers = ResourcesViewManager.DeliverResource<ResourceCarrier>
        (
            target,
            1,
            flay.GetAnchorRect(),
            context.Context.BoardDef.ViewCamera.WorldToScreenPoint(from),
            R.ResourceCarrier
        );

        if (carriers.Count != 0) carriers[0].RefreshIcon(currency);
    }
}