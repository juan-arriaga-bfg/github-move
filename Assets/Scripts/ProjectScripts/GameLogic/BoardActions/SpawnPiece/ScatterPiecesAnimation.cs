using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScatterPiecesAnimation : BoardAnimation
{
    public BoardPosition From;
    public Piece Replace;
    public Dictionary<BoardPosition, Piece> Pieces;
    
    public override void Animate(BoardRenderer context)
    {
        var target = Replace == null ? context.GetElementAt(From) : context.RemoveElementAt(From, false);
        var next = Replace == null ? null : context.CreatePieceAt(Replace, From);
        var startPosition = context.Context.BoardDef.GetPiecePosition(From.X, From.Y);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        if (Pieces.Count > 0)
        {
            sequence.Append(target.CachedTransform.DOScale(new Vector3(1.2f, 0.7f, 1f), 0.1f));
            sequence.Append(target.CachedTransform.DOScale(new Vector3(0.7f, 1.2f, 1f), 0.1f));
            sequence.Append(target.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
            
            var index = 0;
        
            foreach (var pair in Pieces)
            {
                var position = pair.Key;
                var to = context.Context.BoardDef.GetPiecePosition(position.X, position.Y);
                var element = context.CreatePieceAt(pair.Value, position);
                
                var delay = 0.1f + index * 0.02f;
            
                element.CachedTransform.localScale = Vector3.zero;
                element.CachedTransform.localPosition = startPosition;
            
                element.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
            
                sequence.Insert(delay, element.CachedTransform.DOJump(new Vector3(to.x, to.y, element.CachedTransform.position.z), 1, 1, 0.4f).SetEase(Ease.InOutSine));
                sequence.InsertCallback(0.4f, () => context.ResetBoardElement(element, position));
            
                sequence.Insert(delay, element.CachedTransform.DOScale(Vector3.one * 1.3f, 0.2f));
                sequence.Insert(delay + 0.2f, element.CachedTransform.DOScale(Vector3.one, 0.2f));
            
                sequence.Insert(delay + 0.4f, element.CachedTransform.DOScale(new Vector3(1f, 0.8f, 1f), 0.1f));
                sequence.Insert(delay + 0.5f, element.CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
                sequence.Insert(delay + 0.6f, element.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
            
                index++;
            }
        }
        
        if (next != null)
        {
            next.CachedTransform.localScale = Vector3.zero;
            
            sequence.Append(target.CachedTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack));
            sequence.Append(next.CachedTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
        }
        
        sequence.OnComplete(() =>
        {
            if (next != null) context.DestroyElement(target);
            CompleteAnimation(context);
        });
    }
}