using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PieceBoardElementView : BoardElementView
{
    public Piece Piece { get; set; }

    [SerializeField] private Transform selectionView;
    
    [SerializeField] protected List<SpriteRenderer> bodySprites;
    [SerializeField] protected List<ParticleSystem> particles;
    
    [SerializeField] private Material errorSelectionMaterial;
    [SerializeField] private Material defaultSelectionMaterial;
    [SerializeField] private Material highlightPieceMaterial;

    private BoardElementView lockedSubtrate;

    private Animation cachedSelectionAnimation;

    private readonly ViewAnimationUid selectedAnimationId = new ViewAnimationUid();
    
    private SpriteRenderer selectionSprite;
    
    private readonly Color baseColor = new Color(0.6f, 0.4f, 0.2f);
    private readonly Color dragErrorColor = new Color(0.7f, 0.1f, 0.1f);
    private readonly Color dragSpriteErrorColor = new Color(1f, 0.44f, 0.44f, 0.9f);
    private bool isLockVisual = false;
    
    public bool IsHighlighted { get; protected set; }

    public SpriteRenderer SelectionSprite
    {
        get
        {
            if (selectionView != null && selectionSprite == null)
            {
                selectionSprite = selectionView.GetComponent<SpriteRenderer>();
            }
            return selectionSprite;
        }
    }
    
    public virtual void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context);

        var view = transform.Find("View");
        
        if (bodySprites == null || bodySprites.Count == 0)
        {
            bodySprites = new List<SpriteRenderer>(view.GetComponentsInChildren<SpriteRenderer>());   
        }

        if (particles == null || particles.Count == 0)
        {
            particles = new List<ParticleSystem>(view.GetComponentsInChildren<ParticleSystem>());
        }

        Piece = piece;
        Piece.ActorView = this;

        if (selectionView != null)
        {
            selectionSprite = selectionView.GetComponent<SpriteRenderer>();
        }

        lastBoardPosition = piece.CachedPosition;

        if (cachedRenderers == null || cachedRenderers.size == 0)
            CacheLayers();
        foreach (var rend in cachedRenderers)
        {
            rend.CacheDefaultMaterial();
        }

        CheckLock();
    }

    protected List<LockerComponent> GetPieceLockers()
    {
        var lockers = new List<LockerComponent>();
        foreach (var component in Piece.ComponentsCache.Values)
        {
            var ilocker = component as ILockerComponent;
            if (ilocker != null)
                lockers.Add(ilocker.Locker);
        }

        return lockers;
    }
    
    private void CheckLock()
    {
        var lockers = GetPieceLockers();
        var lockedCount = 0;
        foreach (var lockerComponent in lockers)
        {
            if (lockerComponent.IsLocked)
                lockedCount++;
        }
        ToggleLockView(lockedCount == lockers.Count);
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
            ToggleSelection(true, Piece.Draggable.IsValidDrag(boardPos));
        }
    }
    
    public virtual void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        OnDrag(Piece.CachedPosition, worldPos);
        Piece.Context.HintCooldown.Pause(this);
        Piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
    }

    public virtual void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        Piece.Context.HintCooldown.Resume(this);
        
        if (selectionView == null) return;
        
        ToggleSelection(false);
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

    public virtual void ToggleLockView(bool enabled)
    {
        if (isLockVisual == enabled)
            return;

        if (enabled)
        {
            SetGrayscale();
            SaveCurrentMaterialAsDefault();
        }
        else
        {
            ClearCurrentMaterialAsDefault();
            ResetDefaultMaterial();
        }

        if (enabled)
        {
            var pieceDef = PieceType.GetDefById(Piece.PieceType);
            var defaultSubtrate = Piece.PieceType == PieceType.LockedEmpty.Id
                               || Piece.PieceType == PieceType.Fog.Id
                               || pieceDef.Filter.HasFlag(PieceTypeFilter.Obstacle)
                               || pieceDef.Filter.HasFlag(PieceTypeFilter.Mine);
            if (defaultSubtrate == false)
            {
                var substratePosition =
                    new BoardPosition(Piece.CachedPosition.X, Piece.CachedPosition.Y, Piece.CachedPosition.Z - 1);
                lockedSubtrate = Context.CreateBoardElementAt<BoardElementView>(R.LockedSubstrate, substratePosition);    
            }
            
        }
        else if (lockedSubtrate != null)
        {
            Context.DestroyElement(lockedSubtrate);
            lockedSubtrate = null;
        }
        
        particles.ForEach(particle => particle.gameObject.SetActive(!enabled));
        
        
        isLockVisual = enabled;
    }

    public override void ResetViewOnDestroy()
    {
        if (lockedSubtrate != null)
        {
            Context.DestroyElement(lockedSubtrate);
            lockedSubtrate = null;
        }

        base.ResetViewOnDestroy();
    }

    public virtual void ToggleHighlight(bool enabled)
    {
        if (highlightPieceMaterial == null || IsHighlighted == enabled)
        {
            return;
        }
        
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        if (!enabled)
        {
            ResetDefaultMaterial();
            IsHighlighted = false;
        }
        else
        {
            SetHighlight(true, new List<GameObject>{SelectionSprite == null ? null : SelectionSprite.gameObject});
            IsHighlighted = true;
        }
    }

    public void ToggleSelection(bool enabled, bool isValid = true)
    {
        if (SelectionSprite == null)
        {
            return;
        }
        
        if (enabled)
        {
            var duration = 0.2f;

            DOTween.Kill(animationUid);

            var sequence = DOTween.Sequence().SetId(animationUid);

            if ((Piece == null || Piece.Draggable != null) && isValid)
            {
                bodySprites?.ForEach(sprite => sequence.Insert(0f, sprite.DOColor(Color.white, duration)));

                sequence.Insert(0f, selectionSprite.DOColor(baseColor, duration));
                selectionSprite.material = defaultSelectionMaterial;
            }
            else
            {
                bodySprites?.ForEach(sprite => sequence.Insert(0f, sprite.DOColor(dragSpriteErrorColor, duration)));
                sequence.Insert(0f, selectionSprite.DOColor(dragErrorColor, duration));
                selectionSprite.material = errorSelectionMaterial;
            }

            selectionView.gameObject.SetActive(true);
        }
        else
        {
            DOTween.Kill(animationUid);
            bodySprites.ForEach(sprite => sprite.color = Color.white);
            selectionSprite.color = baseColor;
            selectionView.gameObject.SetActive(false);
        }
    }
}