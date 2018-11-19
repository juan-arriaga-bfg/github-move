using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRemoverButtonViewController : IWUIWindowViewController, IPointerDownHandler, IBeginDragHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Image icon;

    [SerializeField] private NSText label;

    [SerializeField] private Transform labelPanelView;

    [SerializeField] private Transform shineView;

    [SerializeField] private Transform warningView;

    [SerializeField] private Transform iconView;

    private bool isDown = false;

    private bool isDrag = false;
    
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
        isDown = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDown && BoardService.Current.FirstBoard.BoardLogic.Remover.IsActive == false)
        {
            var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

            model.Image = "";
        
            model.Title = LocalizationService.Get("window.remover.hint.title",     "Shovel Hint");
            model.Message = LocalizationService.Get("window.remover.hint.message", "Drag and drop shovel to the piece to remove it");
            model.AcceptLabel = LocalizationService.Get("common.button.ok",  "Ok");
 
            model.OnAccept = () =>
            {
                
            };

            model.OnCancel = null;
            
            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
        }
        isDown = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (BoardService.Current.FirstBoard.BoardLogic.Remover.IsActive) return;
        
        int pointerId = eventData.pointerId;
        if (Input.touchSupported == false)
        {
            pointerId = 0;
        }

        
        bool isReady = BoardService.Current.FirstBoard.BoardLogic.Remover.BeginRemover(pointerId);

        if (isReady)
        {
            isDown = false;
            isDrag = true;
        }
    }
}
