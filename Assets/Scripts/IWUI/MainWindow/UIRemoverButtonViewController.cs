using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRemoverButtonViewController : IWUIWindowViewController
{
    [SerializeField] private Image icon;

    [SerializeField] private NSText label;

    [SerializeField] private Transform labelPanelView;

    [SerializeField] private Transform shineView;

    [SerializeField] private Transform warningView;

    [SerializeField] private Transform iconView;

    [IWUIBinding] private UIButtonViewController rootButton;

    private int cachedPointerId = -2;

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);
        
        label.Text = LocalizationService.Get("window.main.remove", "window.main.remove");
        
        var removerComponent = BoardService.Current.FirstBoard.BoardLogic.Remover;
        removerComponent.OnBeginRemoverEvent += OnBeginRemoverEvent;
        removerComponent.OnEndRemoverEvent += OnEndRemoverEvent;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        rootButton
            .ToState(GenericButtonState.Active)
            .SetDragDirection(new Vector2(0f, 1f))
            .SetDragThreshold(30f)
            .OnBeginDrag(OnBeginDragEventHandler)
            .OnClick(OnClickEventHandler);
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
        
        var removerComponent = BoardService.Current.FirstBoard.BoardLogic.Remover;
        removerComponent.OnBeginRemoverEvent -= OnBeginRemoverEvent;
        removerComponent.OnEndRemoverEvent -= OnEndRemoverEvent;
    }
    
    private void OnClickEventHandler(int pointerId)
    {
        if (BoardService.Current.FirstBoard.BoardLogic.Remover.IsActive) return;
        
        if (Input.touchSupported == false)
        {
            pointerId = 0;
        }

        cachedPointerId = pointerId;
        
        bool isReady = BoardService.Current.FirstBoard.BoardLogic.Remover.BeginRemover(pointerId);
    }
    
    private void OnBeginDragEventHandler(UIButtonViewController obj, int pointerId)
    {
        if (BoardService.Current.FirstBoard.BoardLogic.Remover.IsActive) return;
        
        //TODO insert sound
        Debug.LogError("Not implemented sound #remover_take");
        
        if (Input.touchSupported == false)
        {
            pointerId = 0;
        }

        cachedPointerId = pointerId;
        
        bool isReady = BoardService.Current.FirstBoard.BoardLogic.Remover.BeginRemover(pointerId);
    }

    private void OnEndRemoverEvent()
    {
        AnimateShow();
    }

    private void OnBeginRemoverEvent()
    {   
        AnimateHide();
    }

    protected virtual void AnimateShow()
    {   
        if (CachedCanvasGroup == null) return;

        DOTween.Kill(this);
        var sequence = DOTween.Sequence().SetId(this);
        sequence.Append(CachedCanvasGroup.DOFade(1f, 0.35f));
    }
    
    protected virtual void AnimateHide()
    {
        if (CachedCanvasGroup == null) return;
        
        DOTween.Kill(this);
        var sequence = DOTween.Sequence().SetId(this);
        sequence.Append(CachedCanvasGroup.DOFade(0f, 0.35f));
    }

}
