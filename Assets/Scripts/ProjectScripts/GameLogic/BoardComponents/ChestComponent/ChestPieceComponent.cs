using System.Collections.Generic;
using System.Linq;

public class ChestPieceComponent : IECSComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public Chest Chest;

    private Piece contextPiece;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(Chest != null) return;
        
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        var item = save?.GetChestsSave(position);
        
        Chest = GameDataService.Current.ChestsManager.GetChest(contextPiece.PieceType);
        
        if(item == null) return;
        
        Chest.Reward = item.Reward;
        Chest.RewardCount = item.RewardAmount;
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }


    public void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }
    
    public void Open()
    {
        contextPiece.TouchReaction.Locker.Lock(this);
        
        var rewardPieces = Chest.GetRewardPieces();
		
        contextPiece.Context.ActionExecutor.AddAction(new ChestRewardAction
        {
            From = contextPiece.CachedPosition,
            Pieces = rewardPieces,
            OnErrorAction = () => { contextPiece.TouchReaction.Locker.Unlock(this); },
            OnCompleteAction = () =>
            {
                var remaind = rewardPieces.Sum(elem => elem.Value);
				
                if (remaind == 0)
                {
                    contextPiece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                    {
                        To = contextPiece.CachedPosition,
                        Positions = new List<BoardPosition> {contextPiece.CachedPosition}
                    });
				    
                    BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.OpenChest, contextPiece);
                    return;
                }
                
                contextPiece.ActorView.UpdateView();
                contextPiece.TouchReaction.Locker.Unlock(this);
            }
        });
    }
}