using UnityEngine;

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
    
    public void Fly(int id, int x, int y)
    {
        if(GameDataService.Current.QuestsManager.IsNeedToFly(id) == false) return;
        
        FlyTo(id, x, y, PieceType.Parse(id));
    }
    
    public void FlyTo(int id, int x, int y, string target)
    {
        if(Locker.IsLocked) return;
        
        var currency = PieceType.Parse(id);
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