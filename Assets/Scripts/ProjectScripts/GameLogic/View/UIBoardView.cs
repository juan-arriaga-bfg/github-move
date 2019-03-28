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

    public RectTransform GetViewTransform()
    {
        return (RectTransform)viewTransform;
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

    // Used for camera focus
    // Measured in World Space
    public virtual float GetHeight()
    {
        // Working code to get dialog size. BUT we need to wait for "show" animation end before calculations
        // Image image = viewTransform.GetComponent<Image>();
        // if (image == null)
        // {
        //     Debug.LogWarning($"[TouchReactionDefinitionOpenBubble] => FitToScreen: image is null");
        //     return;
        // }
        // Vector3[] imageCorners = new Vector3[4];
        // image.rectTransform.GetWorldCorners(imageCorners);
        // var tl = imageCorners[1].y;
        
        return 2.5f;
    }
    
    // Used for camera focus
    // Measured in World Space
    public virtual float GetWidth()
    {
        return 3.2f;
    }
    
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
        if (isVisible && Context.Context.PathfindLocker.HasPath(Context) == false)
        {
            UpdateVisibility(false);
            return;
        }
        
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
    
    public virtual void FitToScreen()
    {
        var viewTransform = GetViewTransform(); 
        if (viewTransform == null)
        {
            Debug.LogWarning($"[TouchReactionDefinitionOpenBubble] => FitToScreen: viewTransform is null");
            return;
        }

        Camera camera = Camera.main;

        var resourcesView = UIService.Get.GetShowedView<UIResourcePanelWindowView>(UIWindowType.ResourcePanelWindow);
        var mainView = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);

        float safeZoneTop   = resourcesView.GetSafeZoneHeightInWorldSpace();
        float safeZoneLeft  = mainView.GetSafeZoneWidthAtLeftSideInWorldSpace();
        float safeZoneRight = mainView.GetSafeZoneWidthAtRightSideInWorldSpace();
        
        float cameraTop   = camera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        float cameraLeft  = camera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        float cameraRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        Vector3 anchorPos = viewTransform.transform.position;
        float halfWidth = GetWidth() / 2f;
        float height = GetHeight();
        
        float dialogTop   = anchorPos.y + height;
        float dialogLeft  = anchorPos.x - halfWidth;   
        float dialogRight = anchorPos.x + halfWidth;   

        bool needToMove = false;
        Vector3 newPos = camera.transform.position;

        var deltaTop = dialogTop + safeZoneTop - cameraTop;
        if (deltaTop > 0)
        {
            needToMove = true;
            newPos += new Vector3(0, deltaTop);
        }
        
        var deltaLeft = cameraLeft - (dialogLeft - safeZoneLeft);
        if (deltaLeft > 0)
        {
            needToMove = true;
            newPos += new Vector3(-deltaLeft, 0);
        }

        var deltaRight = cameraRight - (dialogRight + safeZoneRight);
        if (deltaRight < 0)
        {
            needToMove = true;
            newPos -= new Vector3(deltaRight, 0);
        }

        if (needToMove)
        {
            BoardService.Current.FirstBoard.Manipulator.CameraManipulator.MoveTo(newPos, true, 0.4f, Ease.OutCubic);
        }
    }
}