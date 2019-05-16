using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScatterPiecesAnimation : BoardAnimation
{
    public BoardPosition From;
    public Dictionary<BoardPosition, Piece> Replace;
    public Dictionary<BoardPosition, Piece> Pieces;
    
    public Func<int, string> AnimationResourceSearchOnRemove;
    
    public bool RewardEffect = false;
    
    public override void Animate(BoardRenderer context)
    {
        var target = context.GetElementAt(From);
        var startPosition = context.Context.BoardDef.GetPiecePosition(From.X, From.Y);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        if (Pieces.Count > 0)
        {
            PlaysSound();
            
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
                var maxDelay = 0.1f + (Pieces.Count - 1) * 0.02f;
                var jumpDuration = 0.4f + maxDelay - delay;
            
                element.CachedTransform.localScale = Vector3.zero;
                element.CachedTransform.localPosition = startPosition;
            
                element.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
                
                sequence.Insert(delay, element.CachedTransform.DOJump(new Vector3(to.x, to.y, element.CachedTransform.position.z), 1, 1, jumpDuration).SetEase(Ease.InOutSine));
                sequence.InsertCallback(delay + jumpDuration, () =>
                {
//                    Debug.LogError($"ScatterPiecesAnimation:{Time.time}: jump completed");
                    context.ResetBoardElement(element, position);
                });

                if (RewardEffect)
                {
                    sequence.InsertCallback(0.0f, () =>
                    {
                        ShowRewardParticle(position, element.transform);
                    });
                }
                
                sequence.Insert(delay, element.CachedTransform.DOScale(Vector3.one * 1.3f, jumpDuration * 0.5f));
                sequence.Insert(delay + jumpDuration * 0.5f, element.CachedTransform.DOScale(Vector3.one, 0.2f));
            
                sequence.Insert(delay + jumpDuration, element.CachedTransform.DOScale(new Vector3(1f, 0.8f, 1f), 0.1f));
                sequence.Insert(delay + jumpDuration + 0.1f, element.CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
                sequence.Insert(delay + jumpDuration + 0.2f, element.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
            
                index++;
            }
        }

        sequence.AppendCallback(() =>
        {
//            Debug.LogError($"ScatterPiecesAnimation:{Time.time}: completed drop");
        });
        
        if (Replace != null)
        {
            var maxDelay = 0.1f + (Pieces.Count - 1) * 0.02f;
            var jumpDuration = 0.4f;
            float replaceDuration = 0.4f;
            float replaceDelay = jumpDuration + maxDelay + 0.3f - replaceDuration;
            
            var targetPiece = target as PieceBoardElementView;
            if (targetPiece != null)
            {
                var animationResource = AnimationResourceSearchOnRemove?.Invoke(targetPiece.Piece.PieceType);
                if (string.IsNullOrEmpty(animationResource) == false)
                {
                    sequence.InsertCallback(replaceDelay, () =>
                    {
                        var animView = context.CreateBoardElementAt<AnimationView>(animationResource, From);
                        animView.Play(targetPiece);
                    });
                }    
            }
        
            sequence.Insert(replaceDelay, target.CachedTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack));
            
            sequence.InsertCallback(replaceDelay, () =>
            {
//                Debug.LogError($"ScatterPiecesAnimation:{Time.time}: start replace"); 
                
                context.RemoveElementAt(From);
                
                foreach (var pair in Replace)
                {
                    var next = context.CreatePieceAt(pair.Value, pair.Key);
                    
                    ShowRewardParticle(pair.Key, next.transform);
                    
                    next.CachedTransform.localScale = Vector3.zero;
                    next.CachedTransform.DOScale(Vector3.one, replaceDuration).SetEase(Ease.OutBack);
                }
            });
  
//            sequence.AppendInterval(0.4f);
        }
        
        sequence.OnComplete(() =>
        {
//            Debug.LogError($"ScatterPiecesAnimation:{Time.time}: global complete");
            
            foreach (var pair in Pieces)
            {
                context.Context.BoardLogic.AddPieceToBoard(pair.Key.X, pair.Key.Y, pair.Value);
            }

            if (Replace != null)
            {
                context.Context.BoardLogic.RemovePieceAt(From);

                foreach (var pair in Replace)
                {
                    context.Context.BoardLogic.AddPieceToBoard(pair.Key.X, pair.Key.Y, pair.Value);
                }
            }
            
            CompleteAnimation(context);
        });
    }

    private void ShowRewardParticle(BoardPosition position, Transform parent)
    {
        if (RewardEffect)
        {
            var particleView = ParticleView.Show(R.RewardDropParticles, position);
            particleView.SyncRendererLayers(new BoardPosition(position.X, position.Y, BoardLayer.Default.Layer));
            if (parent != null)
            {
                particleView.transform.SetParent(parent);
            }
        }
    }

    private void PlaysSound()
    {
        NSAudioService.Current.Play(SoundId.DropObject);
    }
}