using UnityEngine;
using DG.Tweening;
using Lean.Touch;

public class PieceBoardElementView : BoardElementView
{
    public Piece Piece { get; set; }

    [SerializeField] private Transform selectionView;

    private Animation cachedSelectionAnimation;

    private const string selectionActiveAnimationName = "PieceSelectionGrid";
    
    private readonly ViewAnimationUid selectedAnimationId = new ViewAnimationUid();

    private Transform saveParent;
    private int saveLayer;
    private LayerMask saveMask;

    private bool isDragStart;
    
    public virtual void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context);

        Piece = piece;
        Piece.ActorView = this;
    }

    protected virtual void OnEnable()
    {
        if (selectionView == null) return;
        
        if (cachedSelectionAnimation == null)
        {
            cachedSelectionAnimation = selectionView.GetComponent<Animation>();
        }
        
        selectionView.gameObject.SetActive(false);
    }

    public void OnDrag(BoardPosition boardPos, Vector2 worldPos)
    {
        if(isDragStart == false) return;

        var window = UIService.Get.GetShowedWindowByName(UIWindowType.ProductionWindow);
        var uiCamera = window.Layers[0].ViewCamera;
        var screenPos = Piece.Context.BoardDef.ViewCamera.WorldToScreenPoint(worldPos);
        
        CachedTransform.position = uiCamera.ScreenToWorldPoint(screenPos);
        CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y, 0f);
        
        if (selectionView == null) return;
        
        selectionView.gameObject.SetActive(true);
        
        if (cachedSelectionAnimation != null)
        {
            if (cachedSelectionAnimation.isPlaying == false)
            {
                cachedSelectionAnimation.Play(selectionActiveAnimationName);
            }
        }
    }

    public virtual void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        isDragStart = true;

        var uiLayer = UIService.Get.GetShowedWindowByName(UIWindowType.ProductionWindow).Layers[0];
        var scale = uiLayer.ViewCamera.orthographicSize / Piece.Context.BoardDef.ViewCamera.orthographicSize;

        saveParent = CachedTransform.parent;
        saveLayer = CachedTransform.gameObject.layer;
        saveMask = LeanTouch.Instance.GuiLayers;

        CachedTransform.SetParent(transform);
        CachedTransform.SetLayerRecursive(uiLayer.CurrentLayer);
        CachedTransform.localScale = Vector3.one * scale;

        OnDrag(boardPos, worldPos);
        LeanTouch.Instance.GuiLayers = LayerMask.GetMask("Default");
    }

    public virtual void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        if(isDragStart == false) return;

        isDragStart = false;
        
        ResetSate(boardPos);
        
        if (selectionView == null) return;
        
        selectionView.gameObject.SetActive(false);
    }

    public void OnTap(BoardPosition boardPos, Vector2 worldPos)
    {
        DOTween.Kill(selectedAnimationId);
        var sequence = DOTween.Sequence().SetId(selectedAnimationId);
        sequence.Append(CachedTransform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        sequence.Append(CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
        sequence.Append(CachedTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
        
        if(isDragStart == false) return;
        
        ResetSate(boardPos);
    }

    private void ResetSate(BoardPosition boardPos)
    {
        if(saveParent == null) return;
        
        CachedTransform.SetParent(saveParent);
        CachedTransform.SetLayerRecursive(saveLayer);
        CachedTransform.localPosition =  Piece.Context.BoardDef.GetPiecePosition(boardPos.X, boardPos.Y);
        CachedTransform.localScale = Vector3.one;

        LeanTouch.Instance.GuiLayers = saveMask;
        
        saveParent = null;
    }
    
    public virtual void UpdateView()
    {
    }

    public bool Drop(Vector2 worldPos)
    {
        var window = UIService.Get.GetShowedWindowByName(UIWindowType.ProductionWindow);
        var uiCamera = window.Layers[0].ViewCamera;
        var screenPos = Piece.Context.BoardDef.ViewCamera.WorldToScreenPoint(worldPos);
        var view = window.CurrentView as UIProductionWindowView;

        if (view.Drop(Piece.PieceType, uiCamera.ScreenToWorldPoint(screenPos)) == false) return false;
        
        Piece.Context.RendererContext.RemoveElementAt(Piece.CachedPosition);
        Piece.Context.BoardLogic.RemovePieceAt(Piece.CachedPosition);

        return true;
    }
}