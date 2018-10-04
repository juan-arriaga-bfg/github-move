using System.Collections.Generic;
using UnityEngine;

public class HighlightTaskPiece : ITaskHighlight
{
    public void Highlight(TaskEntity task)
    {
        var taskAboutPiece = task as IHavePieceId;
        if (taskAboutPiece == null)
        {
            Debug.LogError($"[HighlightTaskPiece] => Highlight: Can't highlight '{task.GetType()}' because it not implements IHavePieceId");
            return;
        }

        var board = BoardService.Current.FirstBoard;
        
        var targetId = taskAboutPiece.PieceId;
        var piece    = board.BoardLogic.MatchDefinition.GetFirst(targetId);
        
        if(piece == PieceType.None.Id) return;
        
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
            title = "Need wheat pieces?";
            image = "hay_UI";
        }
        else if(piece == PieceType.C1.Id)
        {
            title = "Need stone pieces?";
            image = "stone_UI";
            
            positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.MineC.Id, 1);
        }
        else if(piece == PieceType.D1.Id)
        {
            title = "Need sheep pieces?";
            image = "sheeps_UI";
        }
        else if(piece == PieceType.E1.Id)
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
    }
}