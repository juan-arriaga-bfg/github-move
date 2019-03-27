using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderPieceComponent : IECSComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public RewardsStoreComponent Rewards;

    private Piece contextPiece;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;

        Rewards = new RewardsStoreComponent
        {
            GetRewards = GetRewards,
            OnComplete = OnOpen,
            IsTargetReplace = true
        };
        
        contextPiece.RegisterComponent(Rewards);
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        Rewards.InitInSave(position);
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
    
    private Dictionary<int, int> GetRewards()
    {
        var reward = new Dictionary<int, int>();
        
        reward.Add(PieceType.Soft1.Id, 2);

        
        return reward;
    }

    private void OnOpen(bool isComplete)
    {
        // if(isComplete) BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.OpenChest, contextPiece);
    }
}