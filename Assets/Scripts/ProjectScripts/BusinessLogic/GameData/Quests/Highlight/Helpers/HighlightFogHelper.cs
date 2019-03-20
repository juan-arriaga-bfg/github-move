using Debug = IW.Logger;
using DG.Tweening;
using UnityEngine;

public static class HighlightFogHelper
{
    public static bool HighlightNextFog(float delay)
    {
        var fogManager = GameDataService.Current.FogsManager;
        var fog = fogManager.GetNextRandomFog();
        if (fog == null)
        {
            Debug.LogError($"[HighlightFogHelper] => HighlightNextFog: GetNextRandomFog returns null!");
            return false;
        }

        var fogUid = fog.Uid;
        return HighlightByUid(fogUid, delay);
    }
    
    public static bool HighlightByUid(string fogUid, float delay)
    {
        var fogPos = GameDataService.Current.FogsManager.GetFogPositionByUid(fogUid);

        if (fogPos == null)
        {
            Debug.LogError($"[HighlightFogHelper] => fog with id {fogUid} not found!");
            return false;
        }

        var pos = fogPos.Value;
        pos.Z = BoardLayer.Piece.Layer;
        
        var fogPiece = BoardService.Current.FirstBoard.BoardLogic.GetPieceAt(pos);

        if (fogPiece == null)
        {
            Debug.LogError($"[HighlightFogHelper] => fog with id {fogUid} not found at {pos}!");
            return false;
        }
        
        var fogObserver = fogPiece.GetComponent<FogObserver>(FogObserver.ComponentGuid);

        if (fogObserver?.LockView != null)
        {
            HintArrowView.Show(fogObserver.LockView.GetHintTarget());
            return true;
        }
        
        var views = fogPiece.ViewDefinition.GetViews();
        if (views == null || views.Count == 0)
        {
            Debug.LogError($"[HighlightFogHelper] => fog with id {fogUid} at {pos} have no views!");
            return false;
        }

        var view = views[0];

        if (view is FogProgressView || view is BubbleView)
        {
            var board = BoardService.Current.FirstBoard;

            // wait for task dialog closing
            DOTween.Sequence()
                   .AppendInterval(delay)
                   .AppendCallback(() =>
                    {
                        const float DURATION = 1f;

                        if (board.Manipulator.CameraManipulator.CameraMove.IsLocked == false)
                        {
                            board.Manipulator.CameraManipulator.MoveTo(view.transform.position, true, DURATION);
                        }
                        
                        DOTween.Sequence()
                               .AppendInterval(DURATION - 0.15f)
                               .AppendCallback(() => { view.Attention(); });
                    });
            return true;
        }
        
        Debug.LogError($"[HighlightFogHelper] => fog with id {fogUid} at {pos} unknown view type: {view.GetType()}!");
        return false;
    }
}