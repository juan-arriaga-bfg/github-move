using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovePieceFromToAnimation : BoardAnimation 
{
    public MovePieceFromToAction Action { get; set; }

    public override void Animate(BoardRenderer context)
    {
        var pieceFromView = context.GetElementAt(Action.From);

        context.MoveElement(Action.From, Action.To);
        
        var pos = context.Context.BoardDef.GetPiecePosition(Action.To.X, Action.To.Y);
        pos = new Vector3(pos.x, pos.y, 0f);

        var sequence = DOTween.Sequence().SetId(pieceFromView.AnimationUid);
        sequence.Append(pieceFromView.CachedTransform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutSine));
        sequence.OnComplete(() =>
        {
            context.ResetBoardElement(pieceFromView, Action.To);
            
            CompleteAnimation(context);
        });
   
    }
}
