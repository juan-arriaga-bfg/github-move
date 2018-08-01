using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MulticellularSwapPiecesAnimation : BoardAnimation
{
    public Piece MulticellularPiece;
    public List<BoardPosition> FromPositions;
    public List<Piece> AllPieces;
    public List<BoardPosition> AllSpace;

    public BoardPosition To;

    public override void Animate(BoardRenderer context)
    {
        var sequence = DOTween.Sequence().SetId(animationUid);

        var count = FromPositions.Count;

        if (FromPositions.Count != count)
        {
            sequence.OnComplete(() => CompleteAnimation(context));
            return;
        }

        for (int i = 0; i < AllPieces.Count; i++)
        {
            context.RemoveElement(AllPieces[i].ActorView, false);
        }

        var offset = FromPositions.Count;
        for (int i = offset; i < AllPieces.Count; i++)
        {
            var currentPiece = AllPieces[i];
            context.SetElementAt(currentPiece.CachedPosition, currentPiece.ActorView);
            Move(currentPiece.ActorView, sequence, context, AllSpace[i], i-offset, AllPieces.Count-offset);
        }
        
        var fromView = MulticellularPiece.ActorView;
        context.SetElementAt(MulticellularPiece.CachedPosition, fromView);
        MoveMulticellular(fromView, sequence, context, To);
        
        sequence.OnComplete(() => CompleteAnimation(context));
    }

    private void Move(BoardElementView view, Sequence sequence, BoardRenderer context, BoardPosition to, int index, int sumCount)
    {
        var pos = context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
        pos = new Vector3(pos.x, pos.y, 0f);
        var duration = 0.4f;
        var offset = index / (float)sumCount * duration;
        sequence.Insert(offset, view.CachedTransform.DOLocalJump(pos, 1, 1, duration));
        sequence.InsertCallback(duration + offset, () => context.ResetBoardElement(view, to));
    }

    private void MoveMulticellular(BoardElementView view, Sequence sequence, BoardRenderer context, BoardPosition to)
    {
        var pos = context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
        pos = new Vector3(pos.x, pos.y, 0f);
        var duration = 0.4f;
        sequence.Insert(0, view.CachedTransform.DOLocalMove(pos, duration));
        sequence.InsertCallback(duration, () => context.ResetBoardElement(view, to));
    }
}