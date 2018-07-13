using System.Collections.Generic;

public class MineLifeComponent : StorageLifeComponent
{
    private MineDef def;
    
    public override CurrencyPair Energy
    {
        get
        {
            return def.Conditions.Find(pair => pair.Currency == Currency.Energy.Name);
        }
    }
    
    public override CurrencyPair Worker
    {
        get
        {
            return  def.Conditions.Find(pair => pair.Currency == Currency.Worker.Name);
        }
    }

    public override List<CurrencyPair> Conditions
    {
        get { return def.Conditions; }
    }

    public override string Key
    {
        get { return string.Format("{0}_{1}", thisContext.PieceType, def.Position); }
    }

    public override void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        base.OnAddToBoard(position, context);
        
        var key = new BoardPosition(position.X, position.Y);
        def = GameDataService.Current.MinesManager.GetDef(key);
        
        if(def == null) return;
        
        var timer = thisContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        timer.Delay = def.Delay;
        storage.SpawnPiece = PieceType.Parse(def.Reward.Currency);
        storage.Capacity = storage.Amount = def.Reward.Amount;
        
        HP = def.Size;
        current = 0;//GameDataService.Current.ObstaclesManager.GetSaveStep(position);
    }

    protected override void OnComplete()
    {
        var position = thisContext.CachedPosition;
                
        storage.OnScatter = () =>
        {
            storage.OnScatter = null;
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
        var multi = thisContext.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
        
        if(multi == null) return;

        var mask = multi.Mask;
        
        foreach (var maskPoint in mask)
        {
            var point = multi.GetPointInMask(thisContext.CachedPosition, maskPoint);
            
            thisContext.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
            {
                IsCheckMatch = false,
                At = point,
                PieceTypeId = PieceType.OX1.Id
            });
        }
    }
}