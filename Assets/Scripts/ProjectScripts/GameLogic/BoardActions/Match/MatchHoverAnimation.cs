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

    private bool isRevert;
    public override void Animate(BoardRenderer context)
    {
        HoverPosition = HoverPosition.SetZ(BoardLayer.Piece.Layer);
        From = From.SetZ(BoardLayer.Piece.Layer);
        
        DOTween.Kill(animationUid);
        var sequence = DOTween.Sequence();
        sequence.SetId(animationUid);
        var targetVector =
            context.Context.BoardDef.GetPiecePosition(HoverPosition.X, HoverPosition.Y);
        
        foreach (var piecePos in PiecePositions)
        {
            var pos = piecePos.SetZ(BoardLayer.Piece.Layer);
            var pieceView = context.GetElementAt(pos) as PieceBoardElementView;

            var startPos = context.Context.BoardDef.GetPiecePosition(pieceView.Piece.CachedPosition.X, pieceView.Piece.CachedPosition.Y);
            var alreadyHighlight = pieceView.IsHighlighted;
            if (pos.Equals(From))
            {
                pieceView.ToggleHighlight(true);
                sequence.onKill += () =>
                {
                    pieceView.ToggleHighlight(false);
                };
                continue;
            }
            
            if (pos.Equals(HoverPosition))
            {
                if(alreadyHighlight)
                    continue;
                HighlightMatchable(pieceView,true);
                sequence.onKill += () =>
                {
                    HighlightMatchable(pieceView,false);
                };
                continue;
            }
            
            
            var maxDistance = Mathf.Max(Math.Abs(pos.X - HoverPosition.X),Math.Abs(pos.Y - HoverPosition.Y));
            var distanceFactor = maxDistance <= 2 ? 1 : (maxDistance - 1);
            var targetPos = Vector3.MoveTowards(startPos, targetVector, 0.35f / distanceFactor);

            
            if( !alreadyHighlight )
                HighlightMatchable(pieceView, true);

            var view = pieceView.transform.Find("View");
            sequence.Insert(0, view.DOMove(targetPos, 0.4f).SetEase(Ease.InBack).OnKill(() =>
            {
                if (isRevert)
                    view.DOMove(startPos, 0.2f);
                else
                    view.transform.position = startPos;
                if(!alreadyHighlight)
                    HighlightMatchable(pieceView, false);
            }));
            sequence.Insert(0.4f, view.DOMove(startPos, 0.2f).SetEase(Ease.Linear));
            view.DOMove(startPos, 0.2f).SetEase(Ease.Linear);
        }

        sequence.SetLoops(-1, LoopType.Restart);
    }

    private void HighlightMatchable(PieceBoardElementView view, bool state)
    {
        view.ToggleHighlight(state);
        view.ToggleSelection(state);
    }

    public void RevertAnimation(BoardRenderer context)
    {
        isRevert = true;
        StopAnimation(context);
    }
    
    public override void StopAnimation(BoardRenderer context)
    {
        DOTween.Kill(animationUid);
    }
}