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

            if (piece != null && piece.PieceType == PieceType.Fog.Id) return false;

            return true;

        }, gameBoardController.BoardDef.PieceLayer);

        var collapsePieceAction = new CollapsePieceToAction {Positions = targetPieces, To = BoardPosition.Default()};
        gameBoardController.ActionExecutor.PerformAction(collapsePieceAction);
        
        return true;
    }
}
