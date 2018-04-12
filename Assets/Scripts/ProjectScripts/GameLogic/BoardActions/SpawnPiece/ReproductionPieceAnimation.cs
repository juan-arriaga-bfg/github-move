using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ReproductionPieceAnimation : BoardAnimation 
{
    public ReproductionPieceAction Action;
    public List<Piece> Pieces;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = context.GetElementAt(Action.From);
        var startPosition = context.Context.BoardDef.GetPiecePosition(Action.From.X, Action.From.Y);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        sequence.Append(boardElement.CachedTransform.DOScale(new Vector3(1.2f, 0.7f, 1f), 0.1f));
        sequence.Append(boardElement.CachedTransform.DOScale(new Vector3(0.7f, 1.2f, 1f), 0.1f));
        sequence.Append(boardElement.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));

        for (var i = 0; i < Action.Positions.Count; i++)
        {
            var position = Action.Positions[i];
            var to = context.Context.BoardDef.GetPiecePosition(position.X, position.Y);
            var element = context.CreatePieceAt(Pieces[i], position);
            var delay = 0.1f + i * 0.02f;
            
            element.CachedTransform.localScale = Vector3.zero;
            element.CachedTransform.localPosition = startPosition;
            
            sequence.Insert(delay, element.CachedTransform.DOJump(new Vector3(to.x, to.y, element.CachedTransform.position.z), 1, 1, 0.4f).SetEase(Ease.InOutSine));
            sequence.Insert(delay, element.CachedTransform.DOScale(Vector3.one * 1.3f, 0.2f));
            sequence.Insert(delay + 0.2f, element.CachedTransform.DOScale(Vector3.one, 0.2f));
            
            sequence.Insert(delay + 0.4f, element.CachedTransform.DOScale(new Vector3(1f, 0.8f, 1f), 0.1f));
            sequence.Insert(delay + 0.4f, element.CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
            sequence.Insert(delay + 0.4f, element.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
        }

        sequence.OnComplete(() =>
        {
            CompleteAnimation(context);
        });
    }
}