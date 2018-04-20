using DG.Tweening;
using UnityEngine;

public class ResourceView : BoardElementView
{
    [SerializeField] private Transform body;
    [SerializeField] private SpriteRenderer icon;

    private CurrencyPair resource;
    
    private readonly ViewAnimationUid AnimationId = new ViewAnimationUid();
    private bool isCollect;

    private void Show(CurrencyPair resource)
    {
        this.resource = resource;
        
        DOTween.Kill(this);
        
        const float duration = 1f;
        var sequence = DOTween.Sequence().SetId(AnimationId).SetLoops(int.MaxValue);
        
        sequence.Insert(0, body.DOLocalMoveY(0.2f, duration).SetEase(Ease.InOutSine));
        sequence.Insert(duration, body.DOLocalMoveY(0f, duration).SetEase(Ease.InOutSine));
    }
    
    public void Collect()
    {
        if(isCollect) return;

        isCollect = true;
        
        const float duration = 0.3f;
        
        DOTween.Kill(this);
        DOTween.Kill(AnimationId);

        body.DOScale(Vector3.zero, duration).SetEase(Ease.OutBack);
        CurrencyHellper.Purchase(resource);
        DestroyOnBoard(duration);
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();

        isCollect = false;
        body.localScale = Vector3.one;
        icon.color = Color.white;
    }
    
    public static void Show(BoardPosition @from, BoardPosition traget, CurrencyPair resource)
    {
        const float duration = 0.1f;
        
        var board = BoardService.Current.GetBoardById(0);
        var to = board.BoardDef.GetPiecePosition(traget.X, traget.Y);
        var view = board.RendererContext.CreateBoardElementAt<ResourceView>(R.ResourceView, @from);

        to = new Vector3(to.x + Random.Range(-0.5f, 0.5f), to.y + Random.Range(-0.5f, 0.5f), to.z);
        
        var sequence = DOTween.Sequence().SetId(view);
        sequence.Insert(duration, view.CachedTransform.DOJump(new Vector3(to.x, to.y, view.CachedTransform.position.z), 1, 1, 0.4f).SetEase(Ease.InOutSine));
        sequence.Insert(duration, view.CachedTransform.DOScale(Vector3.one * 1.3f, 0.2f));
        sequence.Insert(duration + 0.2f, view.CachedTransform.DOScale(Vector3.one, 0.2f));
            
        sequence.Insert(duration + 0.4f, view.CachedTransform.DOScale(new Vector3(1f, 0.8f, 1f), 0.1f));
        sequence.Insert(duration + 0.5f, view.CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
        sequence.Insert(duration + 0.6f, view.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
        sequence.InsertCallback(duration + 0.7f, () => view.Show(resource));
    }
}