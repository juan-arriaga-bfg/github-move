using System.Collections.Generic;

public class ReproductionLifeComponent : StorageLifeComponent
{
    private PieceDef def;
    private string childName;
    
    public override string Message => $"Harvest {childName}";

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        def = GameDataService.Current.PiecesManager.GetPieceDef(thisContext.PieceType);
        
        var child = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(def.Reproduction.Currency));
        childName = child?.Name;
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = def.Delay;
        timer.Price = def.FastPrice;
        
        storage.SpawnPiece = PieceType.Parse(def.Reproduction.Currency);
        storage.Capacity = storage.Amount = def.Reproduction.Amount;
        
        HP = def.Limit;
    }
    
    protected override void OnStep()
    {
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            OnSpawnRewards();
        };
    }

    protected override void OnComplete()
    {
        var position = thisContext.CachedPosition;
                
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
            OnSpawnRewards();
            thisContext.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = position,
                Positions = new List<BoardPosition> {position},
                OnComplete = OnRemove
            });
        };
    }

    private void OnRemove()
    {
        thisContext.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
        {
            IsCheckMatch = false,
            At = thisContext.CachedPosition,
            PieceTypeId = PieceType.OX1.Id
        });
    }

    protected override void OnSpawnRewards()
    {
        AddResourceView.Show(StartPosition(), def.StepReward);
    }
}