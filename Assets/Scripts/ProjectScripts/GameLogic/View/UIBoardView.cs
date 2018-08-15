using System;
using DG.Tweening;
using UnityEngine;

public class UIBoardView : BoardElementView
{
    [SerializeField] protected Transform viewTransform;
    [SerializeField] protected CanvasGroup group;
    [SerializeField] protected Canvas canvas;

    protected ViewAnimationUid attentionUid = new ViewAnimationUid();
    protected ViewDefinitionComponent controller;
    protected Piece Context;

    public Action OnHide;
    
    protected int multiSize;
    
    public bool IsShow { get; set; }
    
    private int priority = 0;
    private Vector3 offset = new Vector3(0, 0.5f);
    
    protected int defaultPriority;
    
    public int Priority
    {
        get { return priority < 0 ? priority : priority / 10; }
        set { priority = value; }
    }
    
    public int Layer => defaultPriority;

    protected virtual ViewType Id { get; set; }
    
    public virtual Vector3 Ofset
    {
        get { return offset; }
        set { offset = value; }
    }
    
    public virtual void Init(Piece piece)
    {
        Context = piece;
        ResetAnimation();
        UpdateVisibility(false);
        controller = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        multiSize = GetMultiSize(piece);
        SetOfset();
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

    public virtual void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionBottom(multiSize) + Ofset;
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
        
        if(canvas == null || Context == null) return;
        
        canvas.sortingOrder = boardPosition.X * Context.Context.BoardDef.Width - boardPosition.Y + 5000;
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
    
    public virtual void UpdateVisibility(bool isVisible)
    {
        DOTween.Kill(viewTransform);
        
        var sequence = DOTween.Sequence().SetId(viewTransform);

        if (Priority < 0 || isVisible)
        {
            sequence.Insert(0f, group.DOFade(1, 0.3f));
            sequence.Insert(0F, viewTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
            return;
        }
        
        sequence.Insert(0f, group.DOFade(0, 0.2f));
        sequence.Insert(0f, viewTransform.DOScale(Vector3.zero, 0.2f));
        sequence.InsertCallback(0.2f, Cash);
        sequence.InsertCallback(0.15f, () =>
        {
            if(OnHide != null) OnHide();
            OnHide = null;
        });
    }
    
    private int GetMultiSize(Piece piece)
    {
        var multi = piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);

        if (multi == null) return 1;
        
        return (int)Mathf.Sqrt(multi.Mask.Count + 1);
    }

    private void Cash()
    {
        if(Id == ViewType.None) return;
        
        var viewDef = Context.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        
        if(viewDef == null) return;
        
        viewDef.RemoveView(Id);
    }
}