using DG.Tweening;
using UnityEngine;

public class BouncePieceAnimation : BoardAnimation
{
    public BoardPosition From;
    public BoardElementView BoardElement;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = BoardElement != null ? BoardElement : context.GetElementAt(From);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        sequence.Append(boardElement.CachedTransform.DOScale(new Vector3(1.2f, 0.7f, 1f), 0.1f));
        sequence.Append(boardElement.CachedTransform.DOScale(new Vector3(0.7f, 1.2f, 1f), 0.1f));
        sequence.Append(boardElement.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
    }
}