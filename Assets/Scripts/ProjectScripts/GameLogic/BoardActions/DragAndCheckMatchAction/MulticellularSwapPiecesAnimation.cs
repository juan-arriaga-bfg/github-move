using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using DG.Tweening;
using UnityEngine;

public class MulticellularSwapPiecesAnimation : BoardAnimation
{
    public List<Piece> FromPieces;
    public List<BoardPosition> FromPositions;
    public List<Piece> TargetPieces;
    public List<BoardPosition> TargetPositions;

    public BoardPosition To;
    public BoardPosition From;

    public override void Animate(BoardRenderer context)
    {
        var sequence = DOTween.Sequence().SetId(animationUid);

        var count = FromPositions.Count;

        if (FromPositions.Count != count || TargetPieces.Count != count || FromPieces.Count != count)
        {
            sequence.OnComplete(() => CompleteAnimation(context));
            return;
        }

        var counter = 0;
        var sum = TargetPieces.Count(elem => elem != null);

//        var width = Math.Abs(FromPositions[0].X - FromPositions[count].X) + 1;
//        var height = Math.Abs(FromPositions[0].Y - FromPositions[count].Y) + 1;
//        var size = new Vector2Int(width, height);
//        
//        var offsetX = To.X - From.X;
//        var offsetY = To.Y - From.Y;
//
//
//        if (offsetX < 0)
//        {
//            FromPositions.FlipX(size);
//            TargetPositions.FlipX(size);
//            FromPieces.FlipX(size);
//            TargetPieces.FlipX(size);
//        }
        
        for (int i = 0; i < count; i++)
        {
            var from = FromPositions[i];
            var target = TargetPositions[i];
            
            var targetView = TargetPieces[i] == null ? null : TargetPieces[i].ActorView;
            
            if (targetView != null)
            {
                Move(targetView, sequence, context, from, counter++, sum);
                if (!context.SwapElements(from, target))
                    context.MoveElement(target, from);
            }
            else
            {
                context.MoveElement(from, target);
            }
            
            
        }
        var fromView = FromPieces[0] == null ? null : FromPieces[0].ActorView;
        MoveMulticellular(fromView, sequence, context, To);
        
        sequence.OnComplete(() => CompleteAnimation(context));
    }

    private void Move(BoardElementView view, Sequence sequence, BoardRenderer context, BoardPosition to, int index, int sumCount)
    {
        var pos = context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
        pos = new Vector3(pos.x, pos.y, 0f);
        var duration = 0.4f;
        var offset = index / (float)sumCount * duration;
        //sequence.Insert(offset, view.CachedTransform.DOLocalMove(pos, duration).SetEase(Ease.InOutSine));
        sequence.Insert(offset, view.CachedTransform.DOLocalJump(pos, 1, 1, duration));
        sequence.InsertCallback(duration + offset, () => context.ResetBoardElement(view, to));
    }

    private void MoveMulticellular(BoardElementView view, Sequence sequence, BoardRenderer context, BoardPosition to)
    {
        var pos = context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
        pos = new Vector3(pos.x, pos.y, 0f);
        var duration = 0.4f;
        sequence.Insert(0, view.CachedTransform.DOLocalMove(pos, duration));
        //sequence.Insert(offset, view.CachedTransform.DOLocalJump(pos, 1, 1, duration).SetEase(Ease.InOutSine));
        sequence.InsertCallback(duration, () => context.ResetBoardElement(view, to));
    }
}