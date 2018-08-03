﻿using UnityEngine;
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
        //sequence.timeScale = 0.5f;
        ParticleView.Show(R.OutMergeParticleSystem, new BoardPosition(At.X, At.Y, 4));
        boardElement.SyncRendererLayers(new BoardPosition(At.X, At.Y, 5));
        sequence.Insert(0.0f, boardElement.CachedTransform.DOScale(Vector3.one * 1.3f, 0.3f));
        sequence.Insert(0.3f, boardElement.CachedTransform.DOScale(Vector3.one, 0.1f));
        
        sequence.OnComplete(() =>
        {
            boardElement.SyncRendererLayers(new BoardPosition(At.X, At.Y, At.Z));
            CompleteAnimation(context);
        });
    }
}