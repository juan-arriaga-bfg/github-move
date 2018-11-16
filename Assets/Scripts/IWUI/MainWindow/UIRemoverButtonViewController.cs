using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRemoverButtonViewController : IWUIWindowViewController, IPointerDownHandler
{
    [SerializeField] private Image icon;

    [SerializeField] private NSText label;

    [SerializeField] private Transform labelPanelView;

    [SerializeField] private Transform shineView;

    [SerializeField] private Transform warningView;

    [SerializeField] private Transform iconView;
    
    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);

        var removerComponent = BoardService.Current.FirstBoard.BoardLogic.Remover;
        removerComponent.OnBeginRemoverEvent += OnBeginRemoverEvent;
        removerComponent.OnEndRemoverEvent += OnEndRemoverEvent;
    }
    
    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
        
        var removerComponent = BoardService.Current.FirstBoard.BoardLogic.Remover;
        removerComponent.OnBeginRemoverEvent -= OnBeginRemoverEvent;
        removerComponent.OnEndRemoverEvent -= OnEndRemoverEvent;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        int pointerId = eventData.pointerId;
        if (Input.touchSupported == false)
        {
            pointerId = 0;
        }
        bool isReady = BoardService.Current.FirstBoard.BoardLogic.Remover.BeginRemover(pointerId);
    }
    
}
