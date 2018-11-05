using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateGroupPieces:IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public virtual int Guid => ComponentGuid;

    public Dictionary<BoardPosition, int> Pieces { get; set; }
    public Action OnSuccessEvent { get; set; }

    public Action LogicCallback { get; set; }
    
    public bool PerformAction(BoardController gameBoardController)
    {
        Debug.LogError("GroupSpawnExecute");
        var pieces = new Dictionary<BoardPosition, Piece>();

        var positionsForLock = new List<BoardPosition>();
		
        foreach (var pair in Pieces)
        {
            var piece = gameBoardController.CreatePieceFromType(pair.Value);
            var pos = pair.Key;
            if (gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false)
            {
                continue;
            }
			
            pieces.Add(pos, piece);
            positionsForLock.Add(pos);
        }
        
        LogicCallback?.Invoke();
        
        gameBoardController.BoardLogic.LockCells(positionsForLock, this);

        var lastPiece = pieces.Last();
        foreach (var piece in pieces)
        {
            var animation = new SpawnPieceAtAnimation()
            {
                CreatedPiece = piece.Value,
                At = piece.Key
            };
            if (piece.Key.Equals(lastPiece.Key))
            {
                animation.OnCompleteEvent += boardAnimation =>
                {
                    gameBoardController.BoardLogic.UnlockCells(positionsForLock, this);
                    //gameBoardController.PathfindLocker.OnAddComplete();
                    OnSuccessEvent?.Invoke();
                    
                };
            }
            gameBoardController.RendererContext.AddAnimationToQueue(animation);
        }
        
        return true;
    }
}
