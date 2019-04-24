using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ClearBoardAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public bool PerformAction(BoardController gameBoardController)
    {
        var targetPieces = gameBoardController.BoardLogic.EmptyCellsFinder.FindAllWithCondition(position =>
        {
            bool isEmpty = gameBoardController.BoardLogic.IsEmpty(position);

            if (isEmpty) return false;
            
            var piece = gameBoardController.BoardLogic.GetPieceAt(position);

            if (piece != null && piece.PieceType == PieceType.Empty.Id) return false;

            if (piece != null && piece.PieceType == PieceType.Fog.Id)
            {
                var fogObserver = piece.GetComponent<FogObserver>(FogObserver.ComponentGuid);
                piece.Context.ActionExecutor.AddAction(new CollapseFogToAction
                {
                    To = piece.CachedPosition,
                    Positions = new List<BoardPosition> {piece.CachedPosition},
                    FogObserver = fogObserver,
                    IsIgnoreSpawn = true,
                    OnComplete = () =>
                    {
                        FogSectorsView.Rebuild(piece.Context.RendererContext);
                    }
                });
                
                gameBoardController.ActionExecutor.AddAction(this, BoardActionMode.SingleMode, 100);
                return false;
            }

            return true;

        }, BoardLayer.Piece.Layer);

        var collapsePieceAction = new CollapsePieceToAction {Positions = targetPieces, To = BoardPosition.Default()};
        gameBoardController.ActionExecutor.PerformAction(collapsePieceAction);
        
        return true;
    }
}
