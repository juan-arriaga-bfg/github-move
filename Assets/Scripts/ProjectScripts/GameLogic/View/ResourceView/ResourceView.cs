using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceView : BoardElementView
{
    [SerializeField] private Transform body;
    [SerializeField] private SpriteRenderer icon;

    private CurrencyPair resource;
    private BoardPosition pos;
    
    private readonly ViewAnimationUid AnimationId = new ViewAnimationUid();
    private bool isCollect;

    private void Show(CurrencyPair resource)
    {
        this.resource = resource;

        icon.sprite = IconService.Current.GetSpriteById(this.resource.Currency);

        icon.transform.localScale = Vector3.one * (this.resource.Currency.IndexOf("Charger", StringComparison.Ordinal) == -1 ? 0.9f : 2f);
    }

    private void StartAnimation()
    {
        DOTween.Kill(body);
        
        const float duration = 1f;
        var sequence = DOTween.Sequence().SetId(AnimationId).SetLoops(int.MaxValue);
        
        sequence.Insert(0, body.DOLocalMoveY(0.2f, duration).SetEase(Ease.InOutSine));
        sequence.Insert(duration, body.DOLocalMoveY(0f, duration).SetEase(Ease.InOutSine));
    }
    
    public void Collect()
    {
        if(isCollect) return;

        isCollect = true;
        
        DOTween.Kill(body);
        DOTween.Kill(AnimationId);
        
        var position = BoardPosition.Default();
        
        var target = BoardService.Current.GetBoardById(0).BoardDef.GetPiecePosition(position.X, position.Y);
        var duration = Vector3.Distance(body.position, target) / 20f;
        
        var sequence = DOTween.Sequence().SetId(body);
        sequence.Insert(0.1f, body.DOJump(target, 1, 1, duration).SetEase(Ease.Linear));
        sequence.Insert(0.1f, body.DOScale(Vector3.one * 1.3f, duration*0.5f));
        sequence.Insert(0.1f + duration*0.5f, body.DOScale(Vector3.one * 0.2f, duration*0.5f));
        
        DestroyOnBoard(duration);
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        DOTween.Kill(body);
        DOTween.Kill(AnimationId);
        
        isCollect = false;
        body.localScale = Vector3.one;
        body.localPosition = Vector3.zero;
        icon.color = Color.white;
    }
    
    public static void Show(BoardPosition at, CurrencyPair resource)
    {
        const float duration = 0.1f;
        const int distance = 3;
        
        var board = BoardService.Current.GetBoardById(0);
        
        var left = NormalizePosition(board, at.LeftAtDistance(distance));
        var right = NormalizePosition(board, at.RightAtDistance(distance));
        var up = NormalizePosition(board, at.UpAtDistance(distance));
        var down = NormalizePosition(board, at.DownAtDistance(distance));
        
        var leftPos = board.BoardDef.GetPiecePosition(left.X, left.Y);
        var rightPos = board.BoardDef.GetPiecePosition(right.X, right.Y);
        
        var upPos = board.BoardDef.GetPiecePosition(up.X, up.Y);
        var downPos = board.BoardDef.GetPiecePosition(down.X, down.Y);
        
        var to = new Vector3(Random.Range(leftPos.x, rightPos.x), Random.Range(upPos.y, downPos.y), leftPos.z);
        var view = board.RendererContext.CreateBoardElementAt<ResourceView>(R.ResourceView, at);
        
        var sequence = DOTween.Sequence().SetId(view.body);

        view.pos = at;
        
        sequence.InsertCallback(0, () => view.Show(resource));
        sequence.Insert(duration, view.CachedTransform.DOJump(new Vector3(to.x, to.y, view.CachedTransform.position.z), 1, 1, 0.4f).SetEase(Ease.InOutSine));
        sequence.Insert(duration, view.CachedTransform.DOScale(Vector3.one * 1.3f, 0.2f));
        sequence.Insert(duration + 0.2f, view.CachedTransform.DOScale(Vector3.one, 0.2f));
            
        sequence.Insert(duration + 0.4f, view.CachedTransform.DOScale(new Vector3(1f, 0.8f, 1f), 0.1f));
        sequence.Insert(duration + 0.5f, view.CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
        sequence.Insert(duration + 0.6f, view.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
        sequence.InsertCallback(duration + 0.7f, () => view.StartAnimation());
    }

    private static BoardPosition NormalizePosition(BoardController board, BoardPosition position)
    {
        position.X = Mathf.Clamp(position.X, 1, board.BoardDef.Width);
        position.Y = Mathf.Clamp(position.Y, 1, board.BoardDef.Height);
        
        return position;
    }
}