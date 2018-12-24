using System;
using System.Collections.Generic;
using System.Linq;
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

    private List<BoardElementView> lockedSubtrates = new List<BoardElementView>();

    private Animation cachedSelectionAnimation;

    private readonly ViewAnimationUid selectedAnimationId = new ViewAnimationUid();
    
    private SpriteRenderer selectionSprite;
    
    private readonly Color baseColor = new Color(0.6f, 0.4f, 0.2f);
    private readonly Color dragErrorColor = new Color(0.7f, 0.1f, 0.1f);
    private readonly Color dragSpriteErrorColor = new Color(1f, 0.44f, 0.44f, 0.9f);

    private MulticellularPieceBoardObserver multicellularPieceBoardObserver;

    protected bool isLockVisual = false;

    private List<ParticleView> lockParticles = new List<ParticleView>();

    private GodRayView dropEffectView;
    
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
    
    private HintArrowView arrow;
    
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

		multicellularPieceBoardObserver = Piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);

        if (multicellularPieceBoardObserver != null)
        {
            SyncRendererLayers(piece.CachedPosition);
        }

        ResetDefaultMaterial();
        CacheDefaultMaterials();
        
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
        RemoveArrow();

        HideDropEffect();

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
        RemoveArrow();
        DOTween.Kill(selectedAnimationId);
        var sequence = DOTween.Sequence().SetId(selectedAnimationId);
        sequence.Append(CachedTransform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        sequence.Append(CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
        sequence.Append(CachedTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
    }

    public virtual void UpdateView()
    {
        
    }

    private void SpawnLockParticles()
    {
        var order = 0;
        foreach (var spriteRenderer in bodySprites)
        {
            if (spriteRenderer.sortingOrder > order)
                order = spriteRenderer.sortingOrder;
        }

        order++;
        
        List<BoardPosition> piecePositions = new List<BoardPosition>();
        if (Piece.Multicellular == null)
        {
            piecePositions.Add(Piece.CachedPosition);
        }
        else
        {
            foreach (var maskPos in Piece.Multicellular.Mask)
            {
                piecePositions.Add(Piece.Multicellular.GetPointInMask(Piece.CachedPosition, maskPos));
            }
        }

        foreach (var pos in piecePositions)
        {
            var particle = ParticleView.Show(R.LockParticles, pos.SetZ(BoardLayer.FX.Layer)); 
            lockParticles.Add(particle);
            particle.ParticleRenderer.sortingOrder = order;
        }
    }

    /// <summary>
    /// if piece is multicellular
    /// </summary>
    public virtual bool IsMulticellularPiece
    {
        get
        {
            return multicellularPieceBoardObserver != null;
        }
    }
    
    /// <summary>
    /// Get CachedPosition or Mask positions (if piece is multicellular)
    /// </summary>
    public virtual List<BoardPosition> GetPieceLocatedPositions()
    {
        var points = new List<BoardPosition>();

        if (multicellularPieceBoardObserver != null)
        {
            for (int i = 0; i < multicellularPieceBoardObserver.Mask.Count; i++)
            {
                var point = multicellularPieceBoardObserver.GetPointInMask(Piece.CachedPosition, multicellularPieceBoardObserver.Mask[i]);
                points.Add(point);
            }
        }
        else
        {
            if (Piece != null)
            {
                points.Add(Piece.CachedPosition);
            }
        }

        return points;
    }

    private void RemoveLockParticles()
    {
        foreach (var lockParticle in lockParticles)
        {
            Context.DestroyElement(lockParticle);
        }
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
                               || pieceDef.Filter.HasFlag(PieceTypeFilter.Tree)
                               || pieceDef.Filter.HasFlag(PieceTypeFilter.Mine);
            if (defaultSubtrate == false)
            {
                var substratePositions = GetPieceLocatedPositions();
                for (int i = 0; i < substratePositions.Count; i++)
                {
                    var substratePosition = substratePositions[i];
                    substratePosition = new BoardPosition(substratePosition.X, substratePosition.Y, substratePosition.Z - 1);
                    var lockedSubtrate = Context.CreateBoardElementAt<BoardElementView>(R.LockedSubstrate, substratePosition);
                    lockedSubtrates.Add(lockedSubtrate);
                }
            }

            if (Piece.PieceType != PieceType.Fog.Id)
            {
                RemoveLockParticles();
                SpawnLockParticles();    
            }
            
        }
        else
        {
            if (Piece.PieceType != PieceType.Fog.Id)
            {
                RemoveLockParticles();    
            }

            for (int i = 0; i < lockedSubtrates.Count; i++)
            {
                var lockedSubtrate = lockedSubtrates[i];
                if (lockedSubtrate != null)
                {
                    Context.DestroyElement(lockedSubtrate);
                }
            }
            lockedSubtrates.Clear();
        }
        
        
        particles.ForEach(particle => particle.gameObject.SetActive(!enabled));
        
        
        isLockVisual = enabled;
    }

    private void CacheDefaultMaterials()
    {
        if (cachedRenderers == null || cachedRenderers.size == 0)
        {
            CacheLayers();   
        }

        if (cachedRenderers != null)
        {
            foreach (var rend in cachedRenderers)
            {
                rend.CacheDefaultMaterial();
            }    
        }
    }
    
    public override void ResetViewOnDestroy()
    {
        for (int i = 0; i < lockedSubtrates.Count; i++)
        {
            var lockedSubtrate = lockedSubtrates[i];
            if (lockedSubtrate != null)
            {
                Context.DestroyElement(lockedSubtrate);
            }
        }
        lockedSubtrates.Clear();
        
        DestroyDropEffect();
        
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

    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        base.SyncRendererLayers(boardPosition);

        if (Piece == null) return;
        
        if (multicellularPieceBoardObserver == null) return;
        
        var targetPosition = multicellularPieceBoardObserver.GetUpPosition;
        
        base.SyncRendererLayers(new BoardPosition(targetPosition.X, targetPosition.Y, boardPosition.Z));
    }
    
    public void AddArrow()
    {
        if (arrow != null) return;
        
        arrow = HintArrowView.Show(Piece.CachedPosition, 0, -0.5f, false, true);
    }

    public void RemoveArrow(float delay = 0)
    {
        if (arrow == null) return;

        arrow.CachedTransform.SetParent(null);
        arrow.Remove(delay);
        arrow = null;
    }


    public void ShowDropEffect(bool focus)
    {
        if (dropEffectView != null)
        {
            return;
        }
        
        dropEffectView = GodRayView.Show(Piece.CachedPosition, 5, DestroyDropEffect, 0, 0, focus);
        
        dropEffectView.transform.SetParent(transform);
        dropEffectView.transform.localPosition = Vector3.zero;
    }

    public void DestroyDropEffect()
    {
        if (dropEffectView == null)
        {
            return;
        }
        
        dropEffectView.gameObject.SetActive(false);
        Context.DestroyElement(dropEffectView);
        dropEffectView = null;
    }
    
    public void HideDropEffect(bool animated = true)
    {
        if (dropEffectView == null)
        {
            return;
        }
        
        dropEffectView.Hide(true);
    }

    public virtual bool AvailiableLockTouchMessage()
    {
        return true;
    }
}