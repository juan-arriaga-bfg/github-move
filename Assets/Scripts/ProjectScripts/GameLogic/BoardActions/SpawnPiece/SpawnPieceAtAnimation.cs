using Debug = IW.Logger;
using UnityEngine;
using DG.Tweening;

public class SpawnPieceAtAnimation : BoardAnimation 
{
    public Piece CreatedPiece;
    public BoardPosition At;
    public string AnimationResource;
	public float Delay = 0f;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = context.CreatePieceAt(CreatedPiece, At);
        
        ToggleView(false, CreatedPiece);

        if (boardElement == null)
        {
            Debug.LogError($"[SpawnPieceAtAnimation] => Animate: Can't create piece with id {CreatedPiece.PieceType} at {At}");
        }

        if (string.IsNullOrEmpty(AnimationResource) == false)
        {
            var animView = context.CreateBoardElementAt<AnimationView>(AnimationResource, At);
            animView.OnComplete += () => CompleteAnimation(context);
            animView.Play(boardElement);
            return;
        }
        
        boardElement.CachedTransform.localScale = Vector3.zero;
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        sequence.AppendInterval(Delay);
        sequence.Append(boardElement.CachedTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

        sequence.OnComplete(() =>
        {
            ToggleView(true, CreatedPiece);
            
            CompleteAnimation(context);
        });
    }
}