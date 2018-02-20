using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieceFromToAnimation : BoardAnimation 
{
    public MovePieceFromToAction Action { get; set; }

    public override void Animate(BoardRenderer context)
    {
        var pieceFromView = context.GetElementAt(Action.From);

        context.MoveElement(Action.From, Action.To);
        
        context.ResetBoardElement(pieceFromView, Action.To);
        
        
        CompleteAnimation(context);

    }
}
