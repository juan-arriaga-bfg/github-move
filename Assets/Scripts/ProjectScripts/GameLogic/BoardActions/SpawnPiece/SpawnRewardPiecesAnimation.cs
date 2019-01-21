using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnRewardPiecesAnimation : BoardAnimation
{
    public Dictionary<BoardPosition, Piece> Pieces;

    public bool EnabledTopHighlight = false;
    public bool EnabledBottomHighlight = false;

    public Func<int, string> AnimationResourceSearch;
    
    public override void Animate(BoardRenderer context)
    {
        var sequence = DOTween.Sequence().SetId(animationUid);

        int index = 0;
        foreach (var pair in Pieces)
        {
            
            var position = pair.Key;
            var element = context.CreatePieceAt(pair.Value, position);
        
            element.CachedTransform.localScale = Vector3.zero;
            element.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
            
            var AnimationResource = AnimationResourceSearch?.Invoke(element.Piece.PieceType);
            if (string.IsNullOrEmpty(AnimationResource) == false)
            {
                sequence.InsertCallback(index*0.08f, () =>
                {
                    
                    var animView = context.CreateBoardElementAt<AnimationView>(AnimationResource, position);
                    animView.Play(element);
                    sequence.InsertCallback(index * 0.08f, () => HighlightDroppedPiece(element.Piece));
                    animView.OnComplete += () =>
                    {
                        element.SyncRendererLayers(element.Piece.CachedPosition);
                    };
                });

                continue;
            }

            sequence.InsertCallback(index * 0.08f, () => HighlightDroppedPiece(element.Piece));
            sequence.Insert(index*0.08f, element.CachedTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                element.SyncRendererLayers(element.Piece.CachedPosition);
            }));
            
            index++;
        }

        sequence.OnComplete(() =>
        {
            CompleteAnimation(context);
        });
    }
    
    private void HighlightDroppedPiece(Piece pieceToHighlight)
    {
        pieceToHighlight.ActorView.ShowDropEffect(false, EnabledTopHighlight, EnabledBottomHighlight);
    }
}
