﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnPieceAtAnimation : BoardAnimation 
{
    public SpawnPieceAtAction Action { get; set; }
    

    public override void Animate(BoardRenderer context)
    {
        var boardElement = context.CreateElementAt(Action.PieceTypeId, Action.At);

        boardElement.CachedTransform.localScale = Vector3.zero;
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        sequence.Append(boardElement.CachedTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack));

        sequence.OnComplete(() =>
        {
            CompleteAnimation(context);
        });
    }
}
