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
        var sequence = DOTween.Sequence().SetId(body);
        SetCustomMaterial(BoardElementMaterialType.PiecesLowHighlightMaterial, true);

        sequence.SetLoops(-1);
        
        sequence.AppendInterval(5f);
        
        sequence.Append(body.DOScale(Vector3.one * 1.2f, 0.25f));
        sequence.Append(body.DOScale(Vector3.one * 0.9f, 0.15f));
        sequence.Append(body.DOScale(Vector3.one * 1.2f, 0.25f));
        sequence.Append(body.DOScale(Vector3.one, 0.15f));
        
        sequence.Goto(Random.Range(0, 5), true);
    }
}