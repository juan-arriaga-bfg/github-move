using DG.Tweening;
using UnityEngine;

public class IngredientPieceView : PieceBoardElementView
{
    [SerializeField] private Transform body;

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        StartAnimation();
    }

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        DOTween.Kill(body);
        body.localScale = Vector3.one;
        base.OnDragStart(boardPos, worldPos);
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);
        StartAnimation();
    }

    public override void ResetViewOnDestroy()
    {
        DOTween.Kill(body);
        body.localScale = Vector3.one;
        base.ResetViewOnDestroy();
    }

    private void StartAnimation()
    {
        body.localScale = Vector3.one;

        if (isLockVisual)
            return;
        
        var sequence = DOTween.Sequence().SetId(body);
        SetCustomMaterial(BoardElementMaterialType.PiecesLowHighlightMaterial, true);
        
        sequence.SetLoops(-1);
        
        sequence.Append(body.DOScale(Vector3.one * 1.1f, 0.55f).SetEase(Ease.OutSine));
        sequence.Append(body.DOScale(Vector3.one, 0.40f).SetEase(Ease.OutSine));
        
        sequence.Goto(Random.Range(0, 0.95f), true);
    }

    private void OnDisable()
    {
        DOTween.Kill(body);
    }

    public override void ToggleLockView(bool enabled)
    {
        base.ToggleLockView(enabled);
        
        StartAnimation();
    }
}