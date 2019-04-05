using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MatchSpawnPiecesAtAnimation : BoardAnimation 
{
	public List<Piece> Pieces;
	public List<BoardPosition> Positions;
    
    public void ToggleView(bool state)
    {
        if (Pieces != null)
        {
            for (int i = 0; i < Pieces.Count; i++)
            {
                var nextPiece = Pieces[i];

                if (nextPiece?.ViewDefinition == null) continue;
                
//                nextPiece.ViewDefinition.Visible = state;
                var views = nextPiece.ViewDefinition.GetViews();
                for (int j = 0; j < views.Count; j++)
                {
	                var view = views[j];
	                view.OnSwap(state);
//	                view.UpdateVisibility(state);
                }
            }
        }
    }
    
	public override void Animate(BoardRenderer context)
	{
		var sequence = DOTween.Sequence().SetId(animationUid);

	    ToggleView(false);
	   
		for (var i = 0; i < Pieces.Count; i++)
		{
			var piece = Pieces[i];
			var position = Positions[i];
			
			var boardElement = context.CreatePieceAt(piece, position);
		    
		    // create container for animation
		    Transform centerContainer = new GameObject("_AnimationContainer").transform;
		    centerContainer.position = boardElement.CachedTransform.position;
		    
		    if (piece.Multicellular != null)
		    {
		        centerContainer.position = piece.Multicellular.GetWorldCenterPosition();
		    }
		    
		    // put piece view to container
		    centerContainer.SetParent(boardElement.CachedTransform.parent);
		    boardElement.CachedTransform.SetParent(centerContainer);

		    centerContainer.localScale = Vector3.zero;
			boardElement.SyncRendererLayers(new BoardPosition(position.X, position.Y, BoardLayer.PieceUP1.Layer));
			
			var animationResource = AnimationOverrideDataService.Current.FindAnimation(piece.PieceType, def => def.OnMergeSpawn);
			if (string.IsNullOrEmpty(animationResource) == false)
			{
				centerContainer.localScale = Vector3.one;
			    var scaleForAnimationView = new Vector3(1f, 1f, 1f);
			    if (piece.Multicellular != null)
			    {
			        scaleForAnimationView = scaleForAnimationView * piece.Multicellular.GetWidth();
			    }

			    var At = piece.CachedPosition;
				var animView = context.CreateBoardElementAt<AnimationView>(animationResource, At);
			    
			    
				animView.SyncRendererLayers(new BoardPosition(At.X, At.Y, 6));

				animView.OnComplete += () =>
				{
					ToggleView(true);
					
					CompleteAnimation(context);
				};

				animView.OnLifetimeEnd += () =>
				{
					boardElement.SyncRendererLayers(new BoardPosition(At.X, At.Y, At.Z));
				};
				animView.Play(boardElement);
			    
			    animView.CachedTransform.position = centerContainer.position;
			    animView.CachedTransform.localScale = scaleForAnimationView;
			    
				return;
			}
			
			sequence.Insert(0.1f, centerContainer.DOScale(Vector3.one * 1.2f, 0.4f));
			sequence.Insert(0.6f, centerContainer.DOScale(Vector3.one, 0.3f));
			sequence.InsertCallback(0.9f, () =>
			{
			    if (centerContainer != null)
			    {
			        boardElement.CachedTransform.SetParent(centerContainer.parent);
			        
			        GameObject.Destroy(centerContainer.gameObject);

			        centerContainer = null;
			    }
			    
			    boardElement.SyncRendererLayers(position.Copy());
			});
		}
		
		sequence.OnComplete(() =>
		{
		    ToggleView(true);
		    
		    CompleteAnimation(context);
		});
	}
}