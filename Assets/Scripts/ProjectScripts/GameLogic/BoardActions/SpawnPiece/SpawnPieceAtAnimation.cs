using UnityEngine;
using DG.Tweening;

public class SpawnPieceAtAnimation : BoardAnimation 
{
    public Piece CreatedPiece;
    public BoardPosition At;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = context.CreatePieceAt(CreatedPiece, At);

        if (boardElement == null)
        {
            Debug.LogError($"[SpawnPieceAtAnimation] => Animate: Can't create piece with id {CreatedPiece.PieceType} at {At}");
        }

        var def = AnimationDataManager.GetDefinition(CreatedPiece.PieceType);
        if (def != null && string.IsNullOrEmpty(def.OnFogSpawn) == false)
        {
            var animView = context.CreateBoardElementAt<AnimationView>(def.OnFogSpawn, At);
            animView.OnComplete = () => CompleteAnimation(context);
            animView.Play(boardElement);
            return;
        }
        
        boardElement.CachedTransform.localScale = Vector3.zero;
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        sequence.Append(boardElement.CachedTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

        sequence.OnComplete(() =>
        {
            CompleteAnimation(context);
        });
    }
}

public class MatchSpawnPieceAtAnimation : BoardAnimation 
{
    public Piece CreatedPiece;
    public BoardPosition At;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = context.CreatePieceAt(CreatedPiece, At);

        boardElement.CachedTransform.localScale = Vector3.zero;
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        //sequence.timeScale = 0.5f;
        //ParticleView.Show(R.OutMergeParticleSystem, new BoardPosition(At.X, At.Y, 4));
        //ParticleView.Show(R.OutFrontMergeParticleSystem, new BoardPosition(At.X, At.Y, 6));
        boardElement.SyncRendererLayers(new BoardPosition(At.X, At.Y, 5));
        sequence.Insert(0.1f, boardElement.CachedTransform.DOScale(Vector3.one * 1.2f, 0.4f));
        sequence.Insert(0.6f, boardElement.CachedTransform.DOScale(Vector3.one, 0.3f));
        //sequence.Insert(0.1f, boardElement.CachedTransform.DOLocalJump(boardElement.transform.localPosition, 1, 1, 0.3f));
        
        sequence.OnComplete(() =>
        {
            boardElement.SyncRendererLayers(new BoardPosition(At.X, At.Y, At.Z));
            CompleteAnimation(context);
        });
    }
}