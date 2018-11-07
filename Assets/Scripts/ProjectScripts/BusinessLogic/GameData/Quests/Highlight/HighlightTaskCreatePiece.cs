using System.Collections.Generic;
using UnityEngine;

public class HighlightTaskCreatePiece : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        var taskAboutPiece = task as IHavePieceId;
        if (taskAboutPiece == null)
        {
            Debug.LogError($"[HighlightTaskPiece] => Highlight: Can't highlight '{task.GetType()}' because it not implements IHavePieceId");
            return false;
        }

        var board = BoardService.Current.FirstBoard;
        
        var targetId = taskAboutPiece.PieceId;
        var piece    = board.BoardLogic.MatchDefinition.GetFirst(targetId);

        if (piece == PieceType.None.Id)
        {
            return false;
        }
        
        BoardPosition? position = null;

        var title = "";
        var image = "";

        var positions = new List<BoardPosition>();
        
        if(piece == PieceType.A1.Id)
        {
            title = "Need wooden pieces?";
            image = "wood_UI";
            positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceTypeFilter.Obstacle, 1);
        }
        else if(piece == PieceType.B1.Id)
        {
            title = "Need stone pieces?";
            image = "stone_UI";
            
            positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.MN_B.Id, 1);
        }
        else if(piece == PieceType.PR_A1.Id)
        {
            title = "Need sheep pieces?";
            image = "sheeps_UI";
        }
        else if(piece == PieceType.PR_B1.Id)
        {
            title = "Need apple pieces?";
            image = "apple_UI";
        }
        else
        {
            title = "[Debug]";
            image = $"codexQuestion"; 
        }
        
        if (positions.Count != 0) position = positions[0];
        
        UIMessageWindowController.CreateImageMessage(title, image, () =>
        {
            if (position == null || position.Value.X == 0 && position.Value.Y == 0)
            {
                UIService.Get.ShowWindow(UIWindowType.ChestsShopWindow);
                return;
            }
            
            board.HintCooldown.Step(position.Value);
        });

        return true;
    }
}