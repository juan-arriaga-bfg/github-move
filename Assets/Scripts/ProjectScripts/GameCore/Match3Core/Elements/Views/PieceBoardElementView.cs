﻿using UnityEngine;
using DG.Tweening;

public class PieceBoardElementView : BoardElementView
{
    [SerializeField] private Transform selectionView;
    [SerializeField] protected SpriteRenderer bodySprite;
    [SerializeField] private Material errorSelectionMaterial;
    [SerializeField] private Material defaultSelectionMaterial;
    [SerializeField] private Material highlightPieceMaterial;
    
    public Piece Piece { get; set; }
    
    private Animation cachedSelectionAnimation;

    private readonly ViewAnimationUid selectedAnimationId = new ViewAnimationUid();
    
    private SpriteRenderer selectionSprite;
    
    private readonly Color baseColor = new Color(0.6f, 0.4f, 0.2f);
    private readonly Color dragErrorColor = new Color(0.7f, 0.1f, 0.1f);
    private readonly Color dragSpriteErrorColor = new Color(1f, 0.44f, 0.44f, 0.9f);
    
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

        if (bodySprite == null)
        {
            var view = transform.Find("View");
            bodySprite = view.GetComponentInChildren<SpriteRenderer>();
        }
        
        Piece = piece;
        Piece.ActorView = this;
        
        if (selectionView != null)
        {
            selectionSprite = selectionView.GetComponent<SpriteRenderer>();
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
    
    public virtual void ToggleHighlight(bool enabled)
    {
        if (highlightPieceMaterial == null)
        {
            return;
        }

        if (IsHighlighted == enabled)
        {
            return;
        }
        
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;
            if (rend.gameObject == selectionSprite.gameObject) continue;
            
            if (!enabled)
            {
                rend.ResetDefaultMaterial();
                IsHighlighted = false;
            }
            else
            {
                rend.CacheDefaultMaterial();
                rend.CachedRenderer.material = highlightPieceMaterial;
                // rend.CachedRenderer.material.SetFloat("_HighlightSpeed", 1f);
                IsHighlighted = true;
            }
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
                if (bodySprite != null) sequence.Insert(0f, bodySprite.DOColor(Color.white, duration));

                sequence.Insert(0f, selectionSprite.DOColor(baseColor, duration));
                selectionSprite.material = defaultSelectionMaterial;
            }
            else
            {
                if (bodySprite != null) sequence.Insert(0f, bodySprite.DOColor(dragSpriteErrorColor, duration));

                sequence.Insert(0f, selectionSprite.DOColor(dragErrorColor, duration));
                selectionSprite.material = errorSelectionMaterial;
            }

            selectionView.gameObject.SetActive(true);
        }
        else
        {
            DOTween.Kill(animationUid);
            if (bodySprite != null) bodySprite.color = Color.white;
            selectionSprite.color = baseColor;
            selectionView.gameObject.SetActive(false);
        }
    }
}