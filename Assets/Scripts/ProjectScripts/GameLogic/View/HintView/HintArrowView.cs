﻿using System;
using DG.Tweening;
using UnityEngine;

public class HintArrowView : BoardElementView, IHintArrow
{
    private static HintArrowView currentHintArrow;
    
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private Animation animation;
    
    public const float FADE_DURATION = 0.5f;

    private Action onRemove;
    public BoardPosition CachedPosition;
    
    private void Show(bool isLoop, float delayBeforeShow = 0)
    {
        gameObject.SetActive(true);
        icon.color = new Color(1, 1, 1, 0);
        animation.Stop();
        onRemove = null;
        
        DOTween.Kill(animationUid);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        sequence.InsertCallback(delayBeforeShow, () =>
        {
            animation.Play();
        });
        
        sequence.Insert(delayBeforeShow, icon.DOFade(1, 1f));
        
        if(isLoop) return;
        
        sequence.Insert(delayBeforeShow + 3.5f, icon.DOFade(0, FADE_DURATION));
        sequence.InsertCallback(delayBeforeShow + 3.5f + FADE_DURATION * 1.1f, () => DestroyOnBoard(0));
        
        sequence.InsertCallback(delayBeforeShow + 3.5f, () =>
        {
            onRemove?.Invoke();
            onRemove = null;
        });
    }

    public void UpdatePosition(BoardPosition position)
    {
        
    }
    
    public void Remove(float delay = 3.5f)
    {
        delay = Mathf.Max(0, delay);
        
        DOTween.Kill(animationUid);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        sequence.Insert(delay, icon.DOFade(0, FADE_DURATION));

        sequence.InsertCallback(delay, () =>
        {
            onRemove?.Invoke();
            onRemove = null;
        });
        
        DestroyOnBoard(delay + FADE_DURATION * 1.1f);
    }
    
    public static HintArrowView Show(BoardPosition position, float offsetX = 0, float offsetY = 0, bool focus = true, bool loop = false, float delayBeforeShow = 0)
    {  
        var board = BoardService.Current.FirstBoard;
        var target = board.BoardLogic.GetPieceAt(position);
        var boardLogic = board.BoardLogic;
        
        var multi = target?.Multicellular;

        if (multi != null)
        {
            position = multi.GetTopPosition;
        }

        var cachedPosition = new BoardPosition(position.X, position.Y, BoardLayer.UIUP1.Layer);
        var arrowView = board.RendererContext.CreateBoardElementAt<HintArrowView>(R.HintArrow, cachedPosition);

        arrowView.CachedTransform.localPosition = arrowView.CachedTransform.localPosition + (Vector3.up * 2) + new Vector3(offsetX, offsetY);
        arrowView.Show(loop, delayBeforeShow);
        arrowView.CachedPosition = cachedPosition;
        
        var targetPiece = boardLogic.GetPieceAt(position);
        
        if (targetPiece?.ActorView != null)
        {
            void DisableHint(){targetPiece.ActorView.RemoveArrow(0f);}

            targetPiece.ActorView.OnTapCallback += DisableHint;
            targetPiece.ActorView.OnDragStartCallback += DisableHint;
            
            arrowView.SetOnRemoveAction(() =>
            {
                targetPiece.ActorView.OnTapCallback -= DisableHint;
                targetPiece.ActorView.OnDragStartCallback -= DisableHint;
            });    
        }
        
        if (focus == false) return arrowView;

        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
        board.Manipulator.CameraManipulator.MoveTo(worldPos);
        
        return arrowView;
    }
    
    public static HintArrowView Show(Transform hintTarget, float offsetX = 0f, float offsetY = 0f, bool focus = true, bool loop = false, float delayBeforeShow = 0)
    {
        BoardController board = BoardService.Current.FirstBoard;
        var arrowView = board.RendererContext.CreateBoardElementAt<HintArrowView>(R.HintArrow, new BoardPosition(0,0, BoardLayer.UIUP1.Layer));

        arrowView.CachedTransform.position = hintTarget.position + new Vector3(offsetX, offsetY);
        arrowView.Show(loop, delayBeforeShow);
        
        if (focus == false) return arrowView;
        
        board.Manipulator.CameraManipulator.MoveTo(hintTarget.position);
        return arrowView;
    }

    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        var targetPosition = new BoardPosition(Context.Context.BoardDef.Width, 0, BoardLayer.UI.Layer);

        base.SyncRendererLayers(targetPosition);
    }

    public void SetOnRemoveAction(Action onRemove)
    {
        this.onRemove = onRemove;
    }
}