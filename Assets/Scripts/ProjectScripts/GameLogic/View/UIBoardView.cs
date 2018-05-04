using UnityEngine;

public class UIBoardView : BoardElementView
{
    [SerializeField] protected GameObject viewGo;

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

    public virtual Vector3 Ofset
    {
        get { return Vector2.zero; }
    }
    
    public virtual void Init(Piece piece)
    {
        Context = piece;
        UpdateVisibility(false);
        controller = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        multiSize = GetMultiSize(piece);
        SetOfset();
    }

    protected virtual void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionBottom(multiSize) + Ofset;
    }

    public void Change(bool isShow)
    {
        if(controller == null) return;
        
        IsShow = isShow;
        
        if (isShow == false)
        {
            controller.HideView();
            UpdateVisibility(false);
            return;
        }
        
        UpdateVisibility(controller.ShowView(Priority));
    }

    public virtual void UpdateVisibility(bool isVisible)
    {
        viewGo.SetActive(Priority < 0 || isVisible);
    }

    private int GetMultiSize(Piece piece)
    {
        var multi = piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);

        if (multi == null) return 0;
        
        return (int)Mathf.Sqrt(multi.Mask.Count + 1);
    }
}