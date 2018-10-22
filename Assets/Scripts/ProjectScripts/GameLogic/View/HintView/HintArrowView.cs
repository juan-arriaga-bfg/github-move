using DG.Tweening;
using UnityEngine;

public class HintArrowView : BoardElementView
{
    [SerializeField] private SpriteRenderer icon;
    
    public const float DURATION = 5f;
    
    private void Show()
    {
        DOTween.Kill(animationUid);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        sequence.Insert(0, icon.DOFade(1, 1f));
        sequence.Insert(DURATION * 0.5f, icon.DOFade(0, DURATION * 0.5f));
        
        DestroyOnBoard(DURATION);
    }

    public static void Show(BoardPosition position, float offsetX = 0, float offsetY = 0, bool focus = true)
    {
        var board = BoardService.Current.GetBoardById(0);
        var target = board.BoardLogic.GetPieceAt(position);

        var multi = target?.Multicellular;

        if (multi != null)
        {
            position = multi.GetTopPosition;
        }

        var arrowView = board.RendererContext.CreateBoardElementAt<HintArrowView>(R.HintArrow, position);

        arrowView.CachedTransform.localPosition = arrowView.CachedTransform.localPosition + (Vector3.up * 2) + new Vector3(offsetX, offsetY);
        arrowView.Show();
        
        if(focus == false) return;
        
        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
        board.Manipulator.CameraManipulator.MoveTo(worldPos);
    }
}