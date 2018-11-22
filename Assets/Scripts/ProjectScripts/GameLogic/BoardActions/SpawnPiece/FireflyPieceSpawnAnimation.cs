using DG.Tweening;
using UnityEngine;

public class FireflyPieceSpawnAnimation : BoardAnimation 
{
	public Piece CreatedPiece;
	public FireflyPieceSpawnAction Action;
    
	public override void Animate(BoardRenderer context)
	{
		var boardElement = context.CreatePieceAt(CreatedPiece, Action.At);
        
		boardElement.CachedTransform.localScale = Vector3.zero;
        
		var sequence = DOTween.Sequence().SetId(animationUid);
		var particlePosition = new BoardPosition(Action.At.X, Action.At.Y, 2);
		
		sequence.Insert(0f, Action.View.CachedTransform.DOScale(Vector3.one * 0.5f, 0.3f));
		sequence.Insert(0f, Action.View.CachedTransform.DOMove(boardElement.CachedTransform.position, 0.3f));
		sequence.Insert(0.3f, Action.View.CachedTransform.DOScale(Vector3.zero, 0.3f));
		
		sequence.InsertCallback(0.25f, () => ParticleView.Show(R.FireflyExplosion, particlePosition));
		sequence.InsertCallback(0.25f, () => ParticleView.Show(R.SmolderingParticles, particlePosition));
		sequence.InsertCallback(0.25f, () => ParticleView.Show(R.MergeParticleSystem, particlePosition).SyncRendererLayers(new BoardPosition(Action.At.X, Action.At.Y, 4)));
		sequence.Insert(0.3f, boardElement.CachedTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
		
		sequence.InsertCallback(0.2f, () =>
		{
			var temp = Action.View.Plume.main;
			temp.loop = false;
		});

		sequence.AppendInterval(0.7f);
		
		sequence.OnComplete(() =>
		{
			context.DestroyElement(Action.View.gameObject);
			CompleteAnimation(context);
		});
	}
}