using System;
using System.Collections.Generic;

public class ObstacleLifeComponent : LifeComponent, IPieceBoardObserver
{
    public CurrencyPair Price
    {
        get
        {
            return GameDataService.Current.ObstaclesManager.PriceForPiece(thisContext.PieceType, current);
        }
    }

    public float GetProgressNext
    {
        get { return 1 - (current+1)/(float)HP; }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        HP = thisContext.Context.BoardLogic.MatchDefinition.GetIndexInChain(thisContext.PieceType);
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        current = GameDataService.Current.ObstaclesManager.GetSaveStep(position);
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
        
        CurrencyHellper.Purchase(Currency.Damage.Name, 1, Price, success =>
        {
            if(success == false) return;

            isSuccess = true;

            var position = thisContext.CachedPosition;
            
            Damage(1);

            if (current != HP)
            {
                thisContext.Context.ActionExecutor.AddAction(new EjectionPieceAction
                {
                    From = position,
                    OnComplete = onComplete,
                    Pieces = GameDataService.Current.ObstaclesManager.RewardForPiece(thisContext.PieceType, current)
                });
                
                return;
            }

            var chest = GameDataService.Current.ObstaclesManager.ChestForPiece(thisContext.PieceType);
                
            if(chest == PieceType.None.Id) return;
                
            thisContext.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = position,
                Positions = new List<BoardPosition>{position},
                OnComplete = () =>
                {
                    thisContext.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
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