using UnityEngine;
using DG.Tweening;

public class PieceBoardElementView : BoardElementView
{
    public Piece Piece { get; set; }

    [SerializeField] private Transform selectionView;

    private Animation cachedSelectionAnimation;

    private const string selectionActiveAnimationName = "PieceSelectionGrid";
    
    private readonly ViewAnimationUid selectedAnimationId = new ViewAnimationUid();

    private DraggablePieceComponent draggable;

    public DraggablePieceComponent Draggable
    {
        get
        {
            return draggable ?? (draggable =
                       Piece.GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid));
        }
        set { draggable = value; }
    }
    
    private SpriteRenderer selectionSprite;
    private Color baseColor;
    private Color dragErrorColor = new Color(0.6235294f, 0, 0.05653697f);
    
    public virtual void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context);

        Piece = piece;
        Piece.ActorView = this;
        
        if (selectionView != null)
        {
            var renderer = selectionView.GetComponent<SpriteRenderer>();
            selectionSprite = renderer;
            if (renderer != null)
                baseColor = renderer.color;
        }

        lastBoardPosition = piece.CachedPosition;
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

    private BoardPosition lastBoardPosition;
    public void OnDrag(BoardPosition boardPos, Vector2 worldPos)
    {
        if (selectionView == null) return;

        if (!lastBoardPosition.Equals(boardPos))
        {
            lastBoardPosition = boardPos;
            var duration = 0.2f;
            DOTween.Kill(animationUid);
            if (Draggable.IsValidDrag(boardPos))
                selectionSprite.DOColor(baseColor, duration).SetId(animationUid);
            else
                selectionSprite.DOColor(dragErrorColor, duration).SetId(animationUid);
        }
        
        selectionView.gameObject.SetActive(true);
        
        if (cachedSelectionAnimation != null)
        {
            if (cachedSelectionAnimation.isPlaying == false)
            {
                cachedSelectionAnimation.Play(selectionActiveAnimationName);
            }
        }
    }

    public virtual void OnDragToNewPosition(BoardPosition boardPos, Vector2 worldPos)
    {
        
    }
    
    public virtual void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        
    }

    public virtual void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        lastBoardPosition = Piece.CachedPosition;
        
        if (selectionView == null) return;
        DOTween.Kill(animationUid);
        selectionSprite.color = baseColor;
        selectionView.gameObject.SetActive(false);
    }

    public virtual void OnTap(BoardPosition boardPos, Vector2 worldPos)
    {
        DOTween.Kill(selectedAnimationId);
        var sequence = DOTween.Sequence().SetId(selectedAnimationId);
        sequence.Append(CachedTransform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        sequence.Append(CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
        sequence.Append(CachedTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
    }

    public virtual void UpdateView()
    {
    }
}