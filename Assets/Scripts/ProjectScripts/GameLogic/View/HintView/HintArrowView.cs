using DG.Tweening;
using UnityEngine;

public class HintArrowView : BoardElementView
{
    [SerializeField] private SpriteRenderer icon;
    
    public const float DURATION = 0.5f;
    
    private void Show(bool isLoop)
    {
        DOTween.Kill(animationUid);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        sequence.Insert(0, icon.DOFade(1, 1f));
        
        if(isLoop) return;
        
        sequence.Insert(3.5f, icon.DOFade(0, DURATION));
        sequence.InsertCallback(3.5f + DURATION * 1.1f, () => DestroyOnBoard(0));
    }
    
    public void Remove(float delay = 3.5f)
    {
        DOTween.Kill(animationUid);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        sequence.Insert(delay, icon.DOFade(0, DURATION));
        
        DestroyOnBoard(delay + DURATION * 1.1f);
    }
    
    public static HintArrowView Show(BoardPosition position, float offsetX = 0, float offsetY = 0, bool focus = true, bool loop = false)
    {
        var board = BoardService.Current.GetBoardById(0);
        var target = board.BoardLogic.GetPieceAt(position);

        var multi = target?.Multicellular;

        if (multi != null)
        {
            position = multi.GetTopPosition;
        }

        var arrowView = board.RendererContext.CreateBoardElementAt<HintArrowView>(R.HintArrow, new BoardPosition(position.X, position.Y, BoardLayer.UIUP1.Layer));

        arrowView.CachedTransform.localPosition = arrowView.CachedTransform.localPosition + (Vector3.up * 2) + new Vector3(offsetX, offsetY);
        arrowView.Show(loop);
        
        if (focus == false) return arrowView;

        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
        board.Manipulator.CameraManipulator.MoveTo(worldPos);
        
        return arrowView;
    }
    
    public static HintArrowView Show(Transform hintTarget, float offsetX = 0f, float offsetY = 0f, bool focus = true, bool loop = false)
    {
        BoardController board = BoardService.Current.FirstBoard;
        var arrowView = board.RendererContext.CreateBoardElementAt<HintArrowView>(R.HintArrow, new BoardPosition(0,0, BoardLayer.UIUP1.Layer));

        arrowView.CachedTransform.position = hintTarget.position + new Vector3(offsetX, offsetY);
        arrowView.Show(loop);
        
        if (focus == false) return arrowView;
        
        board.Manipulator.CameraManipulator.MoveTo(hintTarget.position);
        return arrowView;
    }

    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        var targetPosition = new BoardPosition(Context.Context.BoardDef.Width, 0, BoardLayer.UI.Layer);
        
        base.SyncRendererLayers(targetPosition);
        
        
    }
}