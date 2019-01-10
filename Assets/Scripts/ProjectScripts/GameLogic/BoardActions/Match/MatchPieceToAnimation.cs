﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MatchPieceToAnimation : BoardAnimation
{
	public BoardPosition To;
	public List<BoardPosition> Positions;
	
	public override void Animate(BoardRenderer context)
	{
		var points = Positions;
		var to = context.Context.BoardDef.GetPiecePosition(To.X, To.Y);
		
		
		
		
		var sequence = DOTween.Sequence().SetId(animationUid);
		const float elementOffset = 0.00f;
		
		var particlePosition = new BoardPosition(To.X, To.Y, 2);
		
		sequence.timeScale = 1.2f;
		sequence.InsertCallback(0.0f, () => ParticleView.Show(R.SmolderingParticles, particlePosition));
		sequence.InsertCallback(0.0f, () => ParticleView.Show(R.MergeParticleSystem, particlePosition).SyncRendererLayers(new BoardPosition(To.X, To.Y, 4)));

		var mergeElementTypeDef = GetMergeTypeDef(context);
		PlaySoundByMergeType( mergeElementTypeDef );
		
		for (var i = 0; i < points.Count; i++)
		{
			var boardElement = context.GetElementAt(points[i]);			
			
			if (points[i].Equals(To))
			{
				sequence.Insert(0.25f, boardElement.CachedTransform.DOScale(Vector3.one * 1.2f, 0.10f));
				sequence.Insert(0.35f, boardElement.CachedTransform.DOScale(Vector3.zero, 0.1f));
				boardElement.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
				continue;
			}
			
			SetTrailToPiece(boardElement, points[i]);
			sequence.Insert(0 + elementOffset*i, boardElement.CachedTransform.DOMove(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.25f));
			sequence.Insert(0.15f + elementOffset*i, boardElement.CachedTransform.DOScale(Vector3.zero, 0.20f));
		}

		sequence.InsertCallback(0.35f, () => ParticleView.Show(R.ExplosionParticleSystem, particlePosition));
		
		sequence.OnComplete(() =>
		{
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
			//TODO insert sound
			Debug.LogError("Not implemented sound #crystal_use");
			return;
		}
		
		if (pieceTypeDef.Id >= PieceType.Soft1.Id && pieceTypeDef.Id <= PieceType.Soft6.Id)
		{
			//TODO insert sound
			Debug.LogError("Not implemented sound #merge_soft_curr");
			return;
		}
		
		if (pieceTypeDef.Id >= PieceType.Hard1.Id && pieceTypeDef.Id <= PieceType.Hard6.Id)
		{
			//TODO insert sound
			Debug.LogError("Not implemented sound #merge_hard_curr");
			return;
		}
		
		if (pieceTypeDef.Filter.HasFlag(PieceTypeFilter.Chest))
		{
			//TODO insert sound
			Debug.LogError("Not implemented sound #merge_chest");
			return;
		}
		
		if (pieceTypeDef.Id >= PieceType.Boost_CR1.Id && pieceTypeDef.Id <= PieceType.Boost_CR2.Id)
		{
			//TODO insert sound
			Debug.LogError("Not implemented sound #merge_crystal_parts");
			return;
		}
		
		if (pieceTypeDef.Id == PieceType.Boost_CR3.Id)
		{
			//TODO insert sound
			Debug.LogError("Not implemented sound #merge_crystal_done");
			return;
		}
		
		//TODO insert sound
		Debug.LogError("Not implemented sound #merge_main");
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