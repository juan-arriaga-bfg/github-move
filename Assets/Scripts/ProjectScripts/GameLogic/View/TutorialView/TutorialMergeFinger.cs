using DG.Tweening;
using UnityEngine;

public class TutorialMergeFinger : BoardElementView
{
    [SerializeField] private Transform anchor;

    private Sequence sequence;
    private BoardPosition start;
    private Transform piece;
    
    public void Init(BoardRenderer context, BoardPosition from, BoardPosition to)
    {
        base.Init(context);
        
        var target = Context.Context.BoardLogic.GetPieceAt(from);
        
        piece = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(PieceType.Parse(target.PieceType) + "_Tutor"));
        piece.SetParentAndReset(anchor);
        
        start = from.DownAtDistance(2).LeftAtDistance(1);
        
        var fromPos = Context.Context.BoardDef.GetPiecePosition(from.X, from.Y);
        var toPos = Context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
        
        Reset();
        
        DOTween.Kill(anchor);
        
        sequence = DOTween.Sequence().SetId(anchor).SetLoops(int.MaxValue);
        
        sequence.Insert(0f, CachedTransform.DOLocalMove(fromPos + new Vector3(0.1f, -0.2f), 1.5f));
        sequence.Insert(1.5f, CachedTransform.DOLocalMove(fromPos, 0.1f));
        sequence.InsertCallback(1.6f, () => anchor.gameObject.SetActive(true));
        sequence.Insert(1.7f, CachedTransform.DOLocalMove(toPos, 1f));
        sequence.InsertCallback(3f, Reset);
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
        CachedTransform.localPosition = Context.Context.BoardDef.GetPiecePosition(start.X, start.Y);
    }
}