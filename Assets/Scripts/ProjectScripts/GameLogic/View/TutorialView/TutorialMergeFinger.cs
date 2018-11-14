using DG.Tweening;
using UnityEngine;

public class TutorialMergeFinger : BoardElementView
{
    [SerializeField] private Transform anchor;
    [SerializeField] private Transform shine;

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
        const float shineDuration = 0.2f;
        
        sequence = DOTween.Sequence().SetId(anchor).SetLoops(int.MaxValue);
        
        sequence.Insert(0f, CachedTransform.DOLocalMove(fromPos, duration));
        
        sequence.InsertCallback(duration, () =>
        {
            anchor.gameObject.SetActive(true);
            shine.gameObject.SetActive(true);
        });
        
        sequence.Insert(duration, shine.DOScale(Vector3.one * 2f, shineDuration));
        sequence.Insert(duration + shineDuration, shine.DOScale(Vector3.zero, shineDuration));
        sequence.Insert(duration + shineDuration + 0.1f, CachedTransform.DOLocalMove(toPos, duration));
        sequence.InsertCallback((duration * 2) + 0.3f, Reset);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Reset();
        sequence?.Restart();
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
        anchor.gameObject.SetActive(false);
        shine.gameObject.SetActive(false);
        shine.localScale = Vector3.one;
        CachedTransform.localPosition = Context.Context.BoardDef.GetPiecePosition(start.X, start.Y);
    }
}