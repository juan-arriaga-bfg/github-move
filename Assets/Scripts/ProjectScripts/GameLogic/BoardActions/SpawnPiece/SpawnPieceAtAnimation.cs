using UnityEngine;
using DG.Tweening;

public class SpawnPieceAtAnimation : BoardAnimation 
{
    public Piece CreatedPiece;
    public BoardPosition At;
    public string AnimationResource;
    
    public override void Animate(BoardRenderer context)
    {
        var boardElement = context.CreatePieceAt(CreatedPiece, At);

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

        boardElement.SyncRendererLayers(new BoardPosition(At.X, At.Y, 5));

        var animationResource = AnimationDataManager.FindAnimation(CreatedPiece.PieceType, def => def.OnMergeSpawn);
        if (string.IsNullOrEmpty(animationResource) == false)
        {
            var animView = context.CreateBoardElementAt<AnimationView>(animationResource, At);
            animView.SyncRendererLayers(new BoardPosition(At.X, At.Y, 6));

            animView.OnComplete += () =>
            {
                CompleteAnimation(context);
            };

            animView.OnLifetimeEnd += () =>
            {
                boardElement.SyncRendererLayers(new BoardPosition(At.X, At.Y, At.Z));
            };
            animView.Play(boardElement);
            return;
        }
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        boardElement.CachedTransform.localScale = Vector3.zero;
        sequence.Insert(0.1f, boardElement.CachedTransform.DOScale(Vector3.one * 1.2f, 0.4f));
        sequence.Insert(0.6f, boardElement.CachedTransform.DOScale(Vector3.one, 0.3f));
        
        
        sequence.OnComplete(() =>
        {
            boardElement.SyncRendererLayers(new BoardPosition(At.X, At.Y, At.Z));
            CompleteAnimation(context);
        });
    }
}