using System;
using System.Collections.Generic;

public class ObstacleLifeComponent : LifeComponent, IPieceBoardObserver
{
    public CurrencyPair Price
    {
        get
        {
            return GameDataService.Current.SimpleObstaclesManager.PriceForPiece(context.PieceType, current);
        }
    }

    public float GetProgressNext
    {
        get { return 1 - (current+1)/(float)HP; }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        HP = context.Context.BoardLogic.MatchDefinition.GetIndexInChain(context.PieceType);
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        current = GameDataService.Current.SimpleObstaclesManager.GetSaveStep(position);
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }

    public bool Damage(Action onComplete)
    {
        if (current == HP) return false;

        var isSuccess = false;
        
        CurrencyHellper.Purchase(Currency.Obstacle.Name, 1, Price, success =>
        {
            if(success == false) return;

            isSuccess = true;

            var position = context.CachedPosition;
            
            Damage(1);

            if (current != HP)
            {
                context.Context.ActionExecutor.AddAction(new EjectionPieceAction
                {
                    From = position,
                    OnComplete = onComplete,
                    Pieces = GameDataService.Current.SimpleObstaclesManager.RewardForPiece(context.PieceType, current)
                });
                
                return;
            }

            var chest = GameDataService.Current.SimpleObstaclesManager.ChestForPiece(context.PieceType);
                
            if(chest == PieceType.None.Id) return;
                
            context.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = position,
                Positions = new List<BoardPosition>{position},
                OnComplete = () =>
                {
                    context.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
                    {
                        IsCheckMatch = false,
                        At = position,
                        PieceTypeId = chest
                    });
                }
            });
        });
        
        return isSuccess;
    }
}