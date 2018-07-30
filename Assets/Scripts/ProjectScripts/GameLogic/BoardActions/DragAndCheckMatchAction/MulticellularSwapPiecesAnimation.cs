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
        Move(fromView, sequence, context, To, 0, 1);
        
        sequence.OnComplete(() => CompleteAnimation(context));
    }

    private void Move(BoardElementView view, Sequence sequence, BoardRenderer context, BoardPosition to, int index, int sumCount)
    {
        var pos = context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
        pos = new Vector3(pos.x, pos.y, 0f);
        var duration = 0.4f;
        var offset = index / (float)sumCount * duration;
        sequence.Insert(offset, view.CachedTransform.DOLocalMove(pos, duration).SetEase(Ease.InOutSine));
        sequence.InsertCallback(duration + offset, () => context.ResetBoardElement(view, to));
    }
}