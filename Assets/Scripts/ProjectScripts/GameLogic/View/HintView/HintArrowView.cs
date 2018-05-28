using DG.Tweening;
using UnityEngine;

public class HintArrowView : BoardElementView
{
    [SerializeField] private SpriteRenderer icon;

    private void Show()
    {
        float duration = 5f;
        
        DOTween.Kill(animationUid);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        sequence.Insert(0, icon.DOFade(1, 1f));
        sequence.Insert(duration * 0.5f, icon.DOFade(0, duration * 0.5f));
        
        DestroyOnBoard(duration);
    }

    public static void Show(BoardPosition position, float offsetX = 0, float offsetY = 0)
    {
        var board = BoardService.Current.GetBoardById(0);
        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
        var arrowView = board.RendererContext.CreateBoardElementAt<HintArrowView>(R.HintArrow, position);

        arrowView.CachedTransform.localPosition = arrowView.CachedTransform.localPosition + (Vector3.up * 2) + new Vector3(offsetX, offsetY);
        arrowView.Show();
        
        board.Manipulator.CameraManipulator.ZoomTo(0.6f, worldPos);
    }
}