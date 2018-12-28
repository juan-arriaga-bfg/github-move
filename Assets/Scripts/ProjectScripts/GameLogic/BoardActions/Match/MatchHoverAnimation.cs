using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class MatchHoverAnimation : BoardAnimation
{
    public List<BoardPosition> PiecePositions;
    public BoardPosition HoverPosition;
    public BoardPosition From;
    public override void Animate(BoardRenderer context)
    {
        HoverPosition = HoverPosition.SetZ(BoardLayer.Piece.Layer);
        From = From.SetZ(BoardLayer.Piece.Layer);
        
        Debug.LogError($"MatchAnimation | HoverPosition:");
        DOTween.Kill(animationUid);
        var sequence = DOTween.Sequence();
        sequence.SetId(animationUid);
        var targetVector =
            context.Context.BoardDef.GetWorldPosition(HoverPosition.X, HoverPosition.Y);
        //targetVector += new Vector3(0, 0.5f);
        foreach (var piecePos in PiecePositions)
        {
            var pos = piecePos.SetZ(BoardLayer.Piece.Layer);
            var pieceView = context.GetElementAt(pos) as PieceBoardElementView;
            if(pos.Equals(HoverPosition))
                continue;
            if (pos.Equals(From))
            {
                HighlightMatchable(pieceView, true);
                sequence.onKill += () => HighlightMatchable(pieceView, false);
                continue;
            }
            
            var startPos = context.Context.BoardDef.GetWorldPosition(pieceView.Piece.CachedPosition.X, pieceView.Piece.CachedPosition.Y);
            var targetPos = Vector3.MoveTowards(startPos, targetVector, 0.35f);
            
            HighlightMatchable(pieceView, true);
            sequence.Insert(0, pieceView.transform.Find("View").DOMove(targetPos, 0.6f).OnKill(() =>
            {
                //pieceView.transform.Find("View").DOMove(startPos, 0.2f);
                pieceView.transform.Find("View").position = startPos;
                HighlightMatchable(pieceView, false);
            }));
        }

        sequence.OnComplete(() => Debug.LogError("Complete"));

        sequence.SetLoops(-1, LoopType.Yoyo);
    }

    public override void CompleteAnimation(BoardRenderer context)
    {
        Debug.LogError("CompleteExecute");
        base.CompleteAnimation(context);
        DOTween.Complete(animationUid);
    }

    private void HighlightMatchable(PieceBoardElementView view, bool state)
    {
        view.ToggleHighlight(state);
        view.ToggleSelection(state);
    }

    public override void StopAnimation(BoardRenderer context)
    {
        DOTween.Kill(animationUid);
    }
}