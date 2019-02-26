using System;
using DG.Tweening;
using UnityEngine;

public class UIBoardView : BoardElementView
{
    [SerializeField] protected Transform viewTransform;
    [SerializeField] protected CanvasGroup group;
    [SerializeField] protected Canvas canvas;
    
    [SerializeField] protected Transform anchor;

    protected readonly ViewAnimationUid attentionUid = new ViewAnimationUid();
    protected ViewDefinitionComponent controller;
    protected Piece Context;
    
    private Transform content;

    public virtual bool IsTop { get; set; }
    
    public Canvas GetCanvas()
    {
        return canvas;
    }

    public CanvasGroup GetCanvasGroup()
    {
        return group;
    }

    public Action OnShow;
    public Action OnHide;
    
    public bool IsShow { get; set; }
    
    private int priority = 0;
    protected int defaultPriority;
    
    public int Priority
    {
        get { return priority < 0 ? priority : priority / 10; }
        set { priority = value; }
    }
    
    public int Layer => defaultPriority;

    protected virtual ViewType Id { get; set; }

    protected virtual Vector3 offset => new Vector3(0, 0);
    
    public virtual void Init(Piece piece)
    {
        Context = piece;
        ResetAnimation();
        UpdateVisibility(false);
        controller = Context.ViewDefinition;
    }

    public override void ResetViewOnDestroy()
    {
        IsShow = false;
        ResetAnimation();
        
        base.ResetViewOnDestroy();
    }

    private void ResetAnimation()
    {
        DOTween.Kill(viewTransform);
        DOTween.Kill(attentionUid);
        
        viewTransform.localScale = new Vector3(0, 0, 1);
        viewTransform.localPosition = Vector3.zero;
        group.alpha = 0;
    }
    
    public void SetOffset(Vector3 value)
    {
        CachedTransform.localPosition = offset + value;
    }

    public void Change(bool isShow)
    {
        UpdateView();
        
        if(controller == null || IsShow == isShow) return;
        
        IsShow = isShow;
        
        if (isShow == false)
        {
            IsShow = Priority < 0;
            DOTween.Kill(attentionUid);
            UpdateVisibility(false);
            return;
        }
        
        UpdateVisibility(controller.ShowView(Priority));
    }

    protected virtual void UpdateView()
    {
    }

    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        base.SyncRendererLayers(boardPosition);
        
        if(canvas == null) return;

        canvas.sortingOrder = GetLayerIndexBy(boardPosition);
    }

    public virtual void Attention()
    {
        DOTween.Kill(attentionUid);
        
        var pos = viewTransform.localPosition;
        var sequence = DOTween.Sequence().SetId(attentionUid).SetEase(Ease.InOutSine);

        sequence.Insert(0f, viewTransform.DOScale(new Vector3(1.2f, 0.7f), 0.1f));
        sequence.Insert(0.1f, viewTransform.DOScale(new Vector3(0.7f, 1.2f), 0.1f));
        sequence.Insert(0.15f, viewTransform.DOLocalMoveY(pos.y + 15, 0.3f));
        sequence.Insert(0.2f, viewTransform.DOScale(Vector3.one, 0.1f));
        sequence.Insert(0.45f, viewTransform.DOLocalMoveY(pos.y, 0.2f));
        sequence.Insert(0.65f, viewTransform.DOScale(new Vector3(1.1f, 0.9f), 0.1f));
        sequence.Insert(0.75f, viewTransform.DOScale(Vector3.one, 0.1f));
    }
    
    public virtual void OnSwap(bool isEnd)
    {
        if(IsShow == false) return; 
        
        DOTween.Kill(viewTransform);
        
        var sequence = DOTween.Sequence().SetId(viewTransform);

        if (isEnd)
        {
            AddShowAnimation(sequence);
            return;
        }
        
        AddHideAnimation(sequence, false);
    }

    public virtual void OnDrag(bool isEnd)
    {
        if(IsShow == false) return; 
        
        DOTween.Kill(viewTransform);
        
        var sequence = DOTween.Sequence().SetId(viewTransform);

        if (isEnd)
        {
            AddShowAnimation(sequence);
            return;
        }
        
        AddHideAnimation(sequence);
    }
    
    public virtual void UpdateVisibility(bool isVisible)
    {
        DOTween.Kill(viewTransform);
        
        var sequence = DOTween.Sequence().SetId(viewTransform);
        
        if (Priority < 0 || isVisible)
        {
            sequence = AddShowAnimation(sequence);
            sequence.InsertCallback(0.25f, () =>
            {
                OnShow?.Invoke();
                OnShow = null;
            });
            return;
        }
        
        Context.Context.RendererContext.RemoveElement(this, false);
        sequence = AddHideAnimation(sequence);
        
        sequence.InsertCallback(0.15f, () =>
        {
            Cash();
            OnHide?.Invoke();
            OnHide = null;
        });
    }

    private Sequence AddShowAnimation(Sequence sequence, bool isAnimate = true)
    {
        if (isAnimate)
        {
            sequence.Insert(0f, group.DOFade(1, 0.3f));
            sequence.Insert(0F, viewTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        }
        else
        {
            group.alpha = 1f;
            viewTransform.localScale = Vector3.one;
        }

        return sequence;
    }

    private Sequence AddHideAnimation(Sequence sequence, bool isAnimate = true)
    {
        if (isAnimate)
        {
            sequence.Insert(0f, group.DOFade(0, 0.2f));
            sequence.Insert(0f, viewTransform.DOScale(Vector3.zero, 0.2f));
        }
        else
        {
            group.alpha = 0f;
            viewTransform.localScale = Vector3.zero;
        }

        return sequence;
    }
    
    private void Cash()
    {
        if(Id == ViewType.None) return;
        
        controller.RemoveView(Id);
    }
    
    protected void CreateIcon(string id)
    {
        if (anchor == null) return;
        if (content != null) UIService.Get.PoolContainer.Return(content.gameObject);
        
        content = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        content.SetParentAndReset(anchor);
    }
}