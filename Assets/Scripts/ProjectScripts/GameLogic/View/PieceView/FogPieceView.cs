using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

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
    
    private List<PieceBoardElementView> fakePieces = new List<PieceBoardElementView>();
    
    public bool IsReadyToClear { get; protected set; }

    // private readonly object HIGHLIGHT_ANIMATION_ID = new ViewAnimationUid();

    public List<PieceBoardElementView> FakePieces => fakePieces;
    
	public override void Init(BoardRenderer context, Piece piece)
	{
		base.Init(context, piece);

	    currentPiece = piece;
	    
		observer = piece.GetComponent<FogObserver>(FogObserver.ComponentGuid);
		
		if(observer == null) return;

	    int fogIndex = 0;
		foreach (var position in observer.Mask)
		{
			var fog = Instantiate(fogItem, fogItem.transform.parent);
			var touch = Instantiate(touchItem, touchItem.transform.parent);
			var sprite = fog.GetComponent<SpriteRenderer>();
			
			fogSprites.Add(sprite);

			sprite.sprite = IconService.Current.GetSpriteById($"Fog{Random.Range(1, 4)}{CheckBorder(position.X, position.Y)}");
			sprite.transform.localScale = Vector3.one * 1.1f;

		    sprite.transform.SetSiblingIndex(GetLayerIndexBy(new BoardPosition(position.X, position.Y, BoardLayer.Piece.Layer)));

		    sprite.sortingOrder = 0;
		    
		    var cachedSpriteSortingGroup = sprite.GetComponent<SortingGroup>();
		    if (cachedSpriteSortingGroup == null)
		    {
		        cachedSpriteSortingGroup = sprite.gameObject.AddComponent<SortingGroup>();
		    }
		    cachedSpriteSortingGroup.sortingOrder = GetLayerIndexBy(new BoardPosition(position.X, position.Y, BoardLayer.Piece.Layer));

			fog.transform.position = touch.transform.position = Context.Context.BoardDef.GetSectorCenterWorldPosition(position.X, position.Y, 0);
			
			touchRegion.AddTouchRegion(touch.GetComponent<RectTransform>());
			
			views.Add(fog);
			views.Add(touch);

		    fogIndex++;
		}
	    
	    ClearCacheLayers();
	    SyncRendererLayers(currentPiece.CachedPosition);

		fogItem.SetActive(false);
		touchItem.SetActive(false);
	    
	    Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
	    Context.Context.BoardEvents.AddListener(this, GameEventsCodes.FogTap);

	    HighlightIfCanClear();
	}

	public void UpdateBorder()
	{
		if(observer == null) return;
	    
	    if (observer.IsRemoved) return;
	    
	    HighlightIfCanClear(true);
	    
	    if (IsHighlighted == false)
	    {
	        for (var i = 0; i < observer.Mask.Count; i++)
	        {
	            var position = observer.Mask[i];
	            var str = CheckBorder(position.X, position.Y);
	            var sprite = fogSprites[i];

	            if (string.IsNullOrEmpty(str) || sprite.sprite.name.Contains(str)) continue;

	            sprite.sprite = IconService.Current.GetSpriteById($"Fog{Random.Range(1, 4)}{str}");
	        }
	    }
	}

	private string CheckBorder(int x, int y)
	{
		var right = new BoardPosition(x+1, y, BoardLayer.Piece.Layer);
		var left = new BoardPosition(x, y-1, BoardLayer.Piece.Layer);
		
		var pieceR = Context.Context.BoardLogic.GetPieceAt(right);
		var pieceL = Context.Context.BoardLogic.GetPieceAt(left);
		
		return pieceR == null || pieceL == null || pieceR.PieceType != PieceType.Fog.Id || pieceL.PieceType != PieceType.Fog.Id ? "_border" : "";
	}

    private void HighlightIfCanClear(bool isAnimate = false)
    {
        if (observer.CanBeCleared())
        {
            ToggleReadyToClear(true, isAnimate);
	        
	        if (observer.Def?.Pieces == null) return;
            
            if (fakePieces.Count > 0) return;
            
            foreach (var pieceDefs in observer.Def.Pieces)
            {
                foreach (var pos in pieceDefs.Value)
                {
                    var pieceId = PieceType.Parse(pieceDefs.Key);
                    var fakePos = new BoardPosition(pos.X, pos.Y, 0);
                    var fakePiece = Context.Context.BoardLogic.DragAndDrop.CreateFakePieceAt(pieceId, fakePos);
	                
                    if (fakePiece != null)
                    {
                        fakePiece.SyncRendererLayers(new BoardPosition(pos.X, pos.Y, 1));
                    
                        var underFogMaterial = fakePiece.SetCustomMaterial(BoardElementMaterialType.PiecesUnderFogMaterial, true, this);
                        underFogMaterial.SetFloat("_AlphaCoef", 0f);
                        underFogMaterial.DOFloat(0.4f, "_AlphaCoef", 2f).SetId(underFogMaterial);
                    
                        fakePieces.Add(fakePiece);
                    }
                }
            }
        }
    }

    // ignored base SyncRendererLayers cause parts of fog need to has separate layer 
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
    
    public virtual void ToggleReadyToClear(bool enabled, bool isAnimate = false)
    {
        if (enabled == IsHighlighted && IsReadyToClear == enabled)
        {
            return;
        }

        IsHighlighted = enabled;
        IsReadyToClear = enabled;
	    // DOTween.Kill(HIGHLIGHT_ANIMATION_ID);
	    
        var fogMaterial = SetCustomMaterial(BoardElementMaterialType.FogDefaultMaterial, enabled, this);
	    
        if (isAnimate)
        {
            DOTween.Kill(fogMaterial);
            fogMaterial.SetFloat("_ScaleCoef", 1f);
            fogMaterial.DOFloat(1f, "_ScaleCoef", 3f).SetId(fogMaterial);
            fogMaterial.SetFloat("_AlphaCoef", 1f);
            fogMaterial.DOFloat(0.85f, "_AlphaCoef", 3f).SetId(fogMaterial);
        }
        else
        {
            fogMaterial.SetFloat("_ScaleCoef", 1f);
            fogMaterial.SetFloat("_AlphaCoef", 0.85f);
        }
        
        for (var i = 0; i < fogSprites.Count; i++)
        {
            var sprite = fogSprites[i];
            sprite.sprite = IconService.Current.GetSpriteById("fog_opacity_1");
        }
    }

    public override void SetGrayscale()
    {
        
    }

    public override void ToggleHighlight(bool enabled)
    {
        if (enabled == IsHighlighted)
        {
            return;
        }

        IsHighlighted = enabled;

        if (IsHighlighted)
        {
            SetCustomMaterial(BoardElementMaterialType.PiecesFogHighlightMaterial, true);
        }
        else
        {
            ResetDefaultMaterial();
        }

        // DOTween.Kill(HIGHLIGHT_ANIMATION_ID);
       
        // const float TIME = 0.35f;
        
        // foreach (var spr in fogSprites)
        // {
           // spr.DOColor(enabled ? highlightColor : initialColor, TIME).SetId(HIGHLIGHT_ANIMATION_ID);
        // }
    }

    public override void OnTap(BoardPosition boardPos, Vector2 worldPos)
    {
	    if(observer.IsActive == false) return;
	    
        base.OnTap(boardPos, worldPos);
        Piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.FogTap, Piece);
    }

    public override bool AvailiableLockTouchMessage()
    {
	    return false;
    }
}