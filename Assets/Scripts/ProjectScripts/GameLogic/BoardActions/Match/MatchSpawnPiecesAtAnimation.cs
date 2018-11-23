using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MatchSpawnPiecesAtAnimation : BoardAnimation 
{
	public List<Piece> Pieces;
	public List<BoardPosition> Positions;
    
	public override void Animate(BoardRenderer context)
	{
		var sequence = DOTween.Sequence().SetId(animationUid);

		for (var i = 0; i < Pieces.Count; i++)
		{
			var piece = Pieces[i];
			var position = Positions[i];
			
			var boardElement = context.CreatePieceAt(piece, position);

			boardElement.CachedTransform.localScale = Vector3.zero;
			boardElement.SyncRendererLayers(new BoardPosition(position.X, position.Y, 5));
			
			sequence.Insert(0.1f, boardElement.CachedTransform.DOScale(Vector3.one * 1.2f, 0.4f));
			sequence.Insert(0.6f, boardElement.CachedTransform.DOScale(Vector3.one, 0.3f));
			sequence.InsertCallback(0.9f, () => boardElement.SyncRendererLayers(position.Copy()));
		}
		
		sequence.OnComplete(() => CompleteAnimation(context));
	}
}