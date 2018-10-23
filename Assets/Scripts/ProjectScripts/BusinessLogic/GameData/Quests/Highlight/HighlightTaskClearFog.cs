using DG.Tweening;
using UnityEngine;

public class HighlightTaskClearFog : TaskHighlightUsingArrow
{
    protected override void ShowArrow(TaskEntity task)
    {
        TaskClearFogEntity fogTask = task as TaskClearFogEntity;
        if (fogTask == null)
        {
            Debug.LogError("[HighlightTaskClearFog] => task is not TaskClearFogEntity");
            return;
        }
        
        string fogUid = fogTask.FogId;
        var fogPos = GameDataService.Current.FogsManager.GetFogPositionByUid(fogUid, false);

        if (fogPos == null)
        {
            Debug.LogError($"[HighlightTaskClearFog] => fog with id {fogUid} not found!");
            return;
        }

        var pos = fogPos.Value;
        pos.Z = BoardService.Current.FirstBoard.BoardDef.PieceLayer;
        
        var fogPiece = BoardService.Current.FirstBoard.BoardLogic.GetPieceAt(pos);

        if (fogPiece == null)
        {
            Debug.LogError($"[HighlightTaskClearFog] => fog with id {fogUid} not found at {pos}!");
            return;
        }

        ViewDefinitionComponent viewDef = fogPiece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        var views = viewDef.GetViews();
        if (views == null || views.Count == 0)
        {
            Debug.LogError($"[HighlightTaskClearFog] => fog with id {fogUid} at {pos} have no views!");
            return;
        }

        var view = views[0];
        if (view is LockView)
        {
            Transform hintTarget = (view as LockView).GetHintTarget();
            HintArrowView.Show(hintTarget); 
        }
        else if (view is BubbleView)
        {
            var board = BoardService.Current.FirstBoard;

            const float DURATION = 1f;
            board.Manipulator.CameraManipulator.MoveTo(view.transform.position, true, DURATION);

            DOTween.Sequence()
                   .AppendInterval(DURATION - 0.15f)
                   .AppendCallback(() =>
                    {
                        view.Attention();
                    });
        }
        else
        {
            Debug.LogError($"[HighlightTaskClearFog] => fog with id {fogUid} at {pos} unknown view type: {view.GetType()}!"); 
        }
    }
}