using UnityEngine;
using DG.Tweening;

public class PieceBoardElementView : BoardElementView
{
    public Piece Piece { get; set; }

    [SerializeField] private Transform selectionView;

    private Animation cachedSelectionAnimation;

    private const string selectionActiveAnimationName = "PieceSelectionGrid";
    
    private readonly ViewAnimationUid selectedAnimationId = new ViewAnimationUid();
    
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
    
    public void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
//        if (selectionView == null) return;
//        
//        selectionView.gameObject.SetActive(true);

        DOTween.Kill(selectedAnimationId);
        var sequence = DOTween.Sequence().SetId(selectedAnimationId);
        sequence.Append(CachedTransform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        sequence.Append(CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
        sequence.Append(CachedTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
    }

    public void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        if (selectionView == null) return;
        
        selectionView.gameObject.SetActive(false);
    }

    public void OnTap(BoardPosition boardPos, Vector2 worldPos)
    {

    }

    public virtual void UpdateView()
    {
    }
}
