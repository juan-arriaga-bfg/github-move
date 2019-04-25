using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class IslandFogElementView : AnimatedBoardElementView
{
    [SerializeField] private GameObject fogItem;
    [SerializeField] private BoardPosition center;
    [SerializeField] private List<BoardPosition> size;
    
    private readonly List<SpriteRenderer> fogSprites = new List<SpriteRenderer>();
    
    public override void Init(BoardRenderer context)
    {
        center = center.SetZ(BoardLayer.PieceUP1.Layer);
        
        foreach (var position in size)
        {
            var fog = Instantiate(fogItem, fogItem.transform.parent);
            var sprite = fog.GetComponent<SpriteRenderer>();
            var positionUp = position.SetZ(BoardLayer.PieceUP1.Layer);
			
            fogSprites.Add(sprite);

            sprite.sprite = IconService.Current.GetSpriteById($"Fog{Random.Range(1, 4)}{CheckBorder(position.X, position.Y)}");
            sprite.transform.localScale = Vector3.one * 1.1f;
            sprite.transform.SetSiblingIndex(GetLayerIndexBy(positionUp));

            sprite.sortingOrder = 0;
		    
            var cachedSpriteSortingGroup = sprite.GetComponent<SortingGroup>();
            if (cachedSpriteSortingGroup == null) cachedSpriteSortingGroup = sprite.gameObject.AddComponent<SortingGroup>();
            
            cachedSpriteSortingGroup.sortingOrder = GetLayerIndexBy(positionUp);
            fog.transform.position = Context.Context.BoardDef.GetSectorCenterWorldPosition(position.X, position.Y, 0);
        }
	    
        ClearCacheLayers();
        SyncRendererLayers(center);
        
        fogItem.SetActive(false);
	    
        base.Init(context);
    }
    
    private string CheckBorder(int x, int y)
    {
        var right = new BoardPosition(x+1, y, BoardLayer.Piece.Layer);
        var left = new BoardPosition(x, y-1, BoardLayer.Piece.Layer);
		
        var pieceR = Context.Context.BoardLogic.GetPieceAt(right);
        var pieceL = Context.Context.BoardLogic.GetPieceAt(left);
		
        return pieceR == null || pieceL == null || pieceR.PieceType != PieceType.Fog.Id || pieceL.PieceType != PieceType.Fog.Id ? "_border" : "";
    }
}