﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FogPieceView : PieceBoardElementView, IBoardEventListener
{
	[SerializeField] private GameObject fogItem;
	[SerializeField] private GameObject touchItem;
	
	[SerializeField] private TouchRegion touchRegion;

    private readonly Color highlightColor = new Color(222 / 255f, 222 / 255f, 1, 1);
    private readonly Color initialColor = new Color(1, 1, 1, 1);
    
	private List<GameObject> views = new List<GameObject>();
	private readonly List<SpriteRenderer> fogSprites = new List<SpriteRenderer>();
	private FogObserver observer;
    private Piece currentPiece;

    private readonly object HIGHLIGHT_ANIMATION_ID = new ViewAnimationUid();
    
	public override void Init(BoardRenderer context, Piece piece)
	{
		base.Init(context, piece);

	    currentPiece = piece;
	    
		observer = piece.GetComponent<FogObserver>(FogObserver.ComponentGuid);
		
		if(observer == null) return;
		
		foreach (var position in observer.Mask)
		{
			var fog = Instantiate(fogItem, fogItem.transform.parent);
			var touch = Instantiate(touchItem, touchItem.transform.parent);
			var sprite = fog.GetComponent<SpriteRenderer>();
			
			fogSprites.Add(sprite);
			
			sprite.sprite = IconService.Current.GetSpriteById($"Fog{Random.Range(1, 4)}{CheckBorder(position.X, position.Y)}");
			sprite.transform.localScale = Vector3.one * 1.1f;
			sprite.sortingOrder = position.X * Context.Context.BoardDef.Width - position.Y + 101;
			
			fog.transform.position = touch.transform.position = Context.Context.BoardDef.GetSectorCenterWorldPosition(position.X, position.Y, 0);
			
			touchRegion.AddTouchRegion(touch.GetComponent<RectTransform>());
			
			views.Add(fog);
			views.Add(touch);
		}

		fogItem.SetActive(false);
		touchItem.SetActive(false);
	    
	    Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
	    Context.Context.BoardEvents.AddListener(this, GameEventsCodes.FogTap);

	    HighlightIfCanClear();
	}

	public void UpdateBorder()
	{
		if(observer == null) return;
		
		for (var i = 0; i < observer.Mask.Count; i++)
		{
			var position = observer.Mask[i];
			var str = CheckBorder(position.X, position.Y);
			var sprite = fogSprites[i];
			
			if(string.IsNullOrEmpty(str) || sprite.sprite.name.Contains(str)) continue;
			
			sprite.sprite = IconService.Current.GetSpriteById($"Fog{Random.Range(1, 4)}{str}");
		}
	}

	private string CheckBorder(int x, int y)
	{
		var right = new BoardPosition(x+1, y, Context.Context.BoardDef.PieceLayer);
		var left = new BoardPosition(x, y-1, Context.Context.BoardDef.PieceLayer);
		
		var pieceR = Context.Context.BoardLogic.GetPieceAt(right);
		var pieceL = Context.Context.BoardLogic.GetPieceAt(left);
		
		return pieceR == null || pieceL == null || pieceR.PieceType != PieceType.Fog.Id || pieceL.PieceType != PieceType.Fog.Id ? "_border" : "";
	}

    private void HighlightIfCanClear()
    {
        if (observer.CanBeCleared())
        {
            ToggleHighlight(true);
        }
    }

    public override void SyncRendererLayers(BoardPosition boardPosition)
	{
		CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y, -boardPosition.Z * 0.1f);
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();

		foreach (var view in views)
		{
			Destroy(view);
		}
		
		views = new List<GameObject>();
		
		touchRegion.ClearTouchRegion();
		
		fogItem.SetActive(true);
		touchItem.SetActive(true);
		
		observer.Clear();
	    
	    Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
	    Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.FogTap);
	}

    public void OnBoardEvent(int code, object context)
    {
        switch (code)
        {
            case GameEventsCodes.ClosePieceUI:
                if (IsHighlighted && !observer.CanBeCleared())
                {
                    ToggleHighlight(false);
                }
                break;
            
            case GameEventsCodes.FogTap:
                if (context == currentPiece != IsHighlighted && !observer.CanBeCleared())
                {
                    ToggleHighlight(context == currentPiece);
                }
                break;
        }
    }

    public override void ToggleHighlight(bool enabled)
    {
        if (enabled == IsHighlighted)
        {
            return;
        }

        IsHighlighted = enabled;

        DOTween.Kill(HIGHLIGHT_ANIMATION_ID);
       
        const float TIME = 0.35f;
        
        foreach (var spr in fogSprites)
        {
           spr.DOColor(enabled ? highlightColor : initialColor, TIME).SetId(HIGHLIGHT_ANIMATION_ID);
        }
    }

    public override void OnTap(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnTap(boardPos, worldPos);
        Piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.FogTap, Piece);
    }
}