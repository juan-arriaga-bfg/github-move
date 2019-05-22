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

    public Action<Dictionary<BoardPosition, Piece>> LogicCallback { get; set; }
    
    public bool PerformAction(BoardController gameBoardController)
    {
        var pieces = new Dictionary<BoardPosition, Piece>();

        var positionsForLock = new List<BoardPosition>();
        
        gameBoardController.BoardLogic.PieceFlyer.Locker.Lock(this);
		
        foreach (var pair in Pieces)
        {
            var piece = gameBoardController.CreatePieceFromType(pair.Value);
            var pos = pair.Key;
            
            var def = PieceType.GetDefById(pair.Value);

            if (def.Filter.Has(PieceTypeFilter.Fake)
                && def.Filter.Has(PieceTypeFilter.Mine) == false
                && piece.PieceState != null)
            {
                piece.PieceState.StartState = BuildingState.Warning;
            }
            
            if (gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false) continue;
            
            pieces.Add(pos, piece);
            positionsForLock.Add(pos);
        }
        
        gameBoardController.BoardLogic.PieceFlyer.Locker.Unlock(this);
        
        LogicCallback?.Invoke(pieces);

        foreach (var pair in pieces)
        {
            gameBoardController.BoardLogic.PieceFlyer.FlyToQuest(pair.Value);
            gameBoardController.BoardLogic.PieceFlyer.FlyToCodex(pair.Value, pair.Key.X, pair.Key.Y, Currency.Codex.Name);
        }
        
        if (pieces.Count == 0)
        {
            OnSuccessEvent?.Invoke();
            return true;
        }
            
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
                    OnSuccessEvent?.Invoke();
                    
                };
            }
            gameBoardController.RendererContext.AddAnimationToQueue(animation);
        }
        
        return true;
    }
}
