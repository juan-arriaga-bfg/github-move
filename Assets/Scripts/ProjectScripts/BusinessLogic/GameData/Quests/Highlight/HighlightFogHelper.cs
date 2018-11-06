using DG.Tweening;
using UnityEngine;

public static class HighlightFogHelper
{
    public static bool HighlightNextFog(float delay)
    {
        var fogUid = GameDataService.Current.FogsManager.GetUidOfFirstNotClearedFog();
        if (string.IsNullOrEmpty(fogUid))
        {
            return false;
        }

        return HighlightByUid(fogUid, delay);
    }
    
    public static bool HighlightByUid(string fogUid, float delay)
    {
        var fogPos = GameDataService.Current.FogsManager.GetFogPositionByUid(fogUid);

        if (fogPos == null)
        {
            Debug.LogError($"[HighlightTaskClearFog] => fog with id {fogUid} not found!");
            return false;
        }

        var pos = fogPos.Value;
        pos.Z = BoardService.Current.FirstBoard.BoardDef.PieceLayer;
        
        var fogPiece = BoardService.Current.FirstBoard.BoardLogic.GetPieceAt(pos);

        if (fogPiece == null)
        {
            Debug.LogError($"[HighlightTaskClearFog] => fog with id {fogUid} not found at {pos}!");
            return false;
        }

        ViewDefinitionComponent viewDef = fogPiece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        var views = viewDef.GetViews();
        if (views == null || views.Count == 0)
        {
            Debug.LogError($"[HighlightTaskClearFog] => fog with id {fogUid} at {pos} have no views!");
            return false;
        }

        var view = views[0];
        if (view is LockView)
        {
            Transform hintTarget = (view as LockView).GetHintTarget();
            HintArrowView.Show(hintTarget);
            return true;
        }

        if (view is BubbleView)
        {
            var board = BoardService.Current.FirstBoard;

            // wait for task dialog closing
            DOTween.Sequence()
                   .AppendInterval(delay)
                   .AppendCallback(() =>
                    {
                        const float DURATION = 1f;
                        board.Manipulator.CameraManipulator.MoveTo(view.transform.position, true, DURATION);

                        DOTween.Sequence()
                               .AppendInterval(DURATION - 0.15f)
                               .AppendCallback(() => { view.Attention(); });
                    });
            return true;
        }
        
        Debug.LogError($"[HighlightTaskClearFog] => fog with id {fogUid} at {pos} unknown view type: {view.GetType()}!");
        return false;
    }
}