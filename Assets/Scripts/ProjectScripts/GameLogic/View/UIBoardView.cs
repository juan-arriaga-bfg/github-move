using DG.Tweening;
using UnityEngine;

public class UIBoardView : BoardElementView
{
    [SerializeField] protected Transform viewTransform;
    [SerializeField] protected CanvasGroup group;

    protected ViewDefinitionComponent controller;
    protected Piece Context;

    protected int multiSize;
    
    public bool IsShow { get; set; }
    
    private int priority = 0;
    
    public virtual int Priority
    {
        get { return priority; }
        set { priority = value; }
    }
    
    protected virtual ViewType Id { get; set; }

    public virtual Vector3 Ofset
    {
        get { return new Vector2(0, 0.5f); }
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
        ResetAnimation();
        
        base.ResetViewOnDestroy();
    }

    private void ResetAnimation()
    {
        DOTween.Kill(viewTransform);
        
        viewTransform.localScale = new Vector3(0, 0, 1);
        group.alpha = 0;
    }

    protected virtual void SetOfset()
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
            controller.HideView();
            UpdateVisibility(false);
            return;
        }
        
        UpdateVisibility(controller.ShowView(Priority));
    }

    protected virtual void UpdateView()
    {
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
        sequence.InsertCallback(0.2f, () => Cash());
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
            
        if (viewDef == null) return;
            
        viewDef.RemoveView(Id);
    }
}