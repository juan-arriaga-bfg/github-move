using DG.Tweening;
using UnityEngine;

public class TutorialMergeFinger : BoardElementView
{
    [SerializeField] private Transform anchor;
    [SerializeField] private Transform shine;
    [SerializeField] private SpriteRenderer finger;

    private Sequence sequence;
    private BoardPosition start;
    private Transform piece;
    
    public void Init(BoardRenderer context, BoardPosition from, BoardPosition to)
    {
        base.Init(context);
        
        var target = Context.Context.BoardLogic.GetPieceAt(from);
        
        piece = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(PieceType.Parse(target.PieceType) + "_Tutor"));
        piece.SetParentAndReset(anchor);
        
        start = from.DownAtDistance(1);
        
        var fromPos = Context.Context.BoardDef.GetPiecePosition(from.X, from.Y);
        var toPos = Context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
        
        Reset();
        
        DOTween.Kill(anchor);
        
        const float duration = 0.5f;
        const float fadeDuration = 0.2f;
        
        sequence = DOTween.Sequence().SetId(anchor).SetLoops(int.MaxValue);

        sequence.AppendCallback(() => finger.DOFade(1f, fadeDuration));
        sequence.Append(CachedTransform.DOLocalMove(fromPos, duration));
        
        sequence.AppendCallback(() =>
        {
            anchor.gameObject.SetActive(true);
            shine.gameObject.SetActive(true);
        });
        
        sequence.Append(shine.DOScale(Vector3.one * 2f, fadeDuration));
        sequence.Append(shine.DOScale(Vector3.zero, fadeDuration * 0.5f));
        sequence.Append(CachedTransform.DOLocalMove(toPos, duration));
        sequence.AppendInterval(duration);
        
        sequence.AppendCallback(() =>
        {
            anchor.gameObject.SetActive(false);
            shine.gameObject.SetActive(false);
        });
        
        sequence.Append(finger.DOFade(0f, fadeDuration));
        sequence.AppendCallback(Reset);
        sequence.AppendInterval(duration);
        
        SyncRendererLayers(new BoardPosition(to.X, to.Y, BoardLayer.UIUP1.Layer));
    }

    public void Show()
    {
        if (gameObject.activeSelf == false) sequence?.Restart();
        
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        sequence?.Pause();
        Reset();
    }
    
    public override void OnFastInstantiate()
    {
    }
    
    public override void OnFastDestroy()
    {
        DOTween.Kill(anchor);
        sequence = null;
        gameObject.SetActive(true);
        UIService.Get.PoolContainer.Return(piece.gameObject);
    }

    private void Reset()
    {
        finger.DOFade(0, 0);
        anchor.gameObject.SetActive(false);
        shine.gameObject.SetActive(false);
        shine.localScale = Vector3.one;
        CachedTransform.localPosition = Context.Context.BoardDef.GetPiecePosition(start.X, start.Y);
    }

    private void OnDestroy()
    {
        DOTween.Kill(anchor);
    }
}