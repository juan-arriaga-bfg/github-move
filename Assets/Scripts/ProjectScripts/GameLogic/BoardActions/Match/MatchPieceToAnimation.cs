using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MatchPieceToAnimation : BoardAnimation
{
	public BoardPosition To;
	public List<BoardPosition> Positions;
    public List<Piece> NextPieces;

    public Vector3 GetToWorldPosition(BoardRenderer context)
    {
        if (NextPieces != null)
        {
            for (int i = 0; i < NextPieces.Count; i++)
            {
                var nextPiece = NextPieces[i];
                if (nextPiece.Multicellular != null)
                {
                    return nextPiece.Multicellular.GetWorldCenterPosition();
                }
            }
        }

        return context.Context.BoardDef.GetPiecePosition(To.X, To.Y);
    }

    public Vector3 GetTargetScale()
    {
        if (NextPieces != null)
        {
            for (int i = 0; i < NextPieces.Count; i++)
            {
                var nextPiece = NextPieces[i];
                
                if (nextPiece.Multicellular != null)
                {
                    return Vector3.one * nextPiece.Multicellular.GetWidth();
                }
            }
        }
        return Vector3.one;
    }

    public void ToggleView(bool state)
    {
	    if (NextPieces != null)
	    {
		    for (int i = 0; i < NextPieces.Count; i++)
		    {
			    var nextPiece = NextPieces[i];

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
		var points = Positions;
	    var to = GetToWorldPosition(context);
	    var targetScale = GetTargetScale();

	    ToggleView(false);
	    
		var sequence = DOTween.Sequence().SetId(animationUid);
		const float elementOffset = 0.00f;
		
		var particlePosition = new BoardPosition(To.X, To.Y, 2);
		
		sequence.timeScale = 1.2f;
		sequence.InsertCallback(0.0f, () => ParticleView.Show(R.SmolderingParticles, particlePosition, to, Vector3.one));
		sequence.InsertCallback(0.0f, () => ParticleView.Show(R.MergeParticleSystem, particlePosition, to, Vector3.one).SyncRendererLayers(new BoardPosition(To.X, To.Y, 4)));

		var mergeElementTypeDef = GetMergeTypeDef(context);
		PlaySoundByMergeType( mergeElementTypeDef );
		
		for (var i = 0; i < points.Count; i++)
		{
			var boardElement = context.GetElementAt(points[i]);

			var boardElementPiece = boardElement as PieceBoardElementView;
			
			if (points[i].Equals(To))
			{
				if (boardElementPiece == null || !boardElementPiece.IsMulticellularPiece)
				{
					sequence.Insert(0 + elementOffset * i,
						boardElement.CachedTransform.DOMove(
							new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.25f));
				}
//				sequence.Insert(0 + elementOffset*i, boardElement.CachedTransform.DOMove(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.25f));
				sequence.Insert(0.25f, boardElement.CachedTransform.DOScale(Vector3.one * 1.2f, 0.10f));
				sequence.Insert(0.35f, boardElement.CachedTransform.DOScale(Vector3.zero, 0.1f));
				boardElement.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
				continue;
			}
			
			SetTrailToPiece(boardElement, points[i]);
			sequence.Insert(0 + elementOffset*i, boardElement.CachedTransform.DOMove(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.25f));
			sequence.Insert(0.15f + elementOffset*i, boardElement.CachedTransform.DOScale(Vector3.zero, 0.20f));
		}

		sequence.InsertCallback(0.35f, () => ParticleView.Show(R.ExplosionParticleSystem, particlePosition, to, targetScale));
		
		sequence.OnComplete(() =>
		{
		    ToggleView(true);
		    
			foreach (var point in points)
			{
				context.RemoveElementAt(point);
			}

			CompleteAnimation(context);
		});
	}

	private void PlaySoundByMergeType( PieceTypeDef pieceTypeDef )
	{
		if (pieceTypeDef.Id == PieceType.Boost_CR.Id)
		{
			NSAudioService.Current.Play(SoundId.CrystalUse);
			return;
		}
		
		if (pieceTypeDef.Id >= PieceType.Soft1.Id && pieceTypeDef.Id <= PieceType.Soft6.Id)
		{
			NSAudioService.Current.Play(SoundId.MergeSoftCurr);
			return;
		}
		
		if (pieceTypeDef.Id >= PieceType.Hard1.Id && pieceTypeDef.Id <= PieceType.Hard6.Id)
		{
			NSAudioService.Current.Play(SoundId.MergeHardCurr);
			return;
		}
		
		if (pieceTypeDef.Filter.HasFlag(PieceTypeFilter.Chest))
		{
			NSAudioService.Current.Play(SoundId.MergeChest);
			return;
		}
		
		if (pieceTypeDef.Id >= PieceType.Boost_CR1.Id && pieceTypeDef.Id <= PieceType.Boost_CR2.Id)
		{
			NSAudioService.Current.Play(SoundId.MergeCrystalParts);
			return;
		}
		
		if (pieceTypeDef.Id == PieceType.Boost_CR3.Id)
		{
			NSAudioService.Current.Play(SoundId.MergeCrystalDone);
			return;
		}
		
		if (pieceTypeDef.Filter.HasFlag(PieceTypeFilter.Fake))
		{
			return;
		}
		
		NSAudioService.Current.Play(SoundId.MergeMain);
	}

	private PieceTypeDef GetMergeTypeDef(BoardRenderer context)
	{
		var mergeElementType = (context.GetElementAt(Positions[0]) as PieceBoardElementView).Piece.PieceType;
		var mergeElementTypeDef = PieceType.GetDefById(mergeElementType);
		return mergeElementTypeDef;
	} 
	
	private void SetTrailToPiece(BoardElementView pieceView, BoardPosition piecePosition)
	{
		var particles = ParticleView.Show(R.TrailMergeParticles, piecePosition);
		particles.transform.SetParent(pieceView.transform);
	}
}