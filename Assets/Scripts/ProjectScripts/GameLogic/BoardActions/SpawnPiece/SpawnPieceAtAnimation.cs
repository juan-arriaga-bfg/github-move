using UnityEngine;
using DG.Tweening;

public class SpawnPieceAtAnimation : BoardAnimation 
{
    public Piece CreatedPiece;
    public BoardPosition At;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = context.CreatePieceAt(CreatedPiece, At);

        boardElement.CachedTransform.localScale = Vector3.zero;
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        sequence.Append(boardElement.CachedTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

        sequence.OnComplete(() =>
        {
            CompleteAnimation(context);
        });
    }
}

public class MatchSpawnPieceAtAnimation : BoardAnimation 
{
    public Piece CreatedPiece;
    public BoardPosition At;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = context.CreatePieceAt(CreatedPiece, At);

        boardElement.CachedTransform.localScale = Vector3.zero;
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        ParticleView.Show(R.OutMergeParticleSystem, new BoardPosition(At.X, At.Y, 0));
        sequence.Insert(0.1f, boardElement.CachedTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InFlash));
        
        sequence.OnComplete(() =>
        {
            CompleteAnimation(context);
        });
    }
}