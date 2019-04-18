using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ReproductionPieceAnimation : BoardAnimation
{
    public BoardPosition From;
    public Vector3? StartPosition;
    
    public BoardElementView BoardElement;
    
    public Dictionary<BoardPosition, Piece> Pieces;

    public Func<int, string> AnimationResourceSearch;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = BoardElement ? BoardElement : context.GetElementAt(From);
        var startPosition = StartPosition ?? context.Context.BoardDef.GetPiecePosition(From.X, From.Y);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        if (Pieces.Count != 0)
        {
            sequence.Append(boardElement.CachedTransform.DOScale(new Vector3(1.2f, 0.7f, 1f), 0.1f));
            sequence.Append(boardElement.CachedTransform.DOScale(new Vector3(0.7f, 1.2f, 1f), 0.1f));
            sequence.Append(boardElement.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));    
        }
        
        NSAudioService.Current.Play(SoundId.DropObject);

        var index = 0;

        foreach (var pair in Pieces)
        {
            var position = pair.Key;
            var to = context.Context.BoardDef.GetPiecePosition(position.X, position.Y);
            var element = context.CreatePieceAt(pair.Value, position);
            var delay = 0.1f + index * 0.02f;
            
            index++;

            element.CachedTransform.localScale = Vector3.zero;
            element.CachedTransform.localPosition = startPosition;
            
            element.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
            
            sequence.Insert(delay, element.CachedTransform.DOJump(new Vector3(to.x, to.y, element.CachedTransform.position.z), 1, 1, 0.4f).SetEase(Ease.InOutSine));
            sequence.InsertCallback(0.4f, () => context.ResetBoardElement(element, position));
            
            sequence.Insert(delay, element.CachedTransform.DOScale(Vector3.one * 1.3f, 0.2f));
            sequence.Insert(delay + 0.2f, element.CachedTransform.DOScale(Vector3.one, 0.2f));

            var animationResource = AnimationResourceSearch?.Invoke(element.Piece.PieceType);
            
            if (string.IsNullOrEmpty(animationResource) == false)
            {
                sequence.InsertCallback(delay + 0.4f, () =>
                {
                    var animView = context.CreateBoardElementAt<AnimationView>(animationResource, position);
                    animView.Play(element);
                });

                continue;
            }

            sequence.Insert(delay + 0.4f, element.CachedTransform.DOScale(new Vector3(1f, 0.8f, 1f), 0.1f));
            sequence.Insert(delay + 0.5f, element.CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
            sequence.Insert(delay + 0.6f, element.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
        }

        sequence.OnComplete(() =>
        {
            CompleteAnimation(context);
        });
    }
}