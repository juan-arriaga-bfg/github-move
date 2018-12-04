using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HighlightTaskCollectFirefly : ITaskHighlight
{
    public const float DELAY_BEFORE_SHOW_ARROW = 0.5f;
    public const float ARROW_LIFETIME = 1.5f;
    
    public bool Highlight(TaskEntity task)
    {
        return HighlightVisibleFirefly();
    }

    private bool HighlightVisibleFirefly()
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var fireflyLogic = boardLogic.FireflyLogic;
        var fireflyViews = fireflyLogic.Views;

        if (fireflyViews == null || fireflyViews.Count == 0)
        {
            return false;
        }
        
        var camera = Camera.main;
        if (camera == null)
        {
            return false;
        }
        
        var cameraManipulator = camera.GetComponent<CameraManipulator>();
        var region = cameraManipulator.CurrentCameraSettings.CameraClampRegion;

        var visibleFireflies = new List<FireflyView>();
        
        foreach (var view in fireflyViews)
        {
            var pos = view.transform.position;
            if (region.Contains(pos))
            {
                visibleFireflies.Add(view);
            }
        }

        if (visibleFireflies.Count == 0)
        {
            return false;
        }

        int index = Random.Range(0, visibleFireflies.Count);
        var selectedView = visibleFireflies[index];
        
        var board = BoardService.Current.FirstBoard;
        board.Manipulator.CameraManipulator.MoveTo(selectedView.transform.position);


        selectedView.AddArrow(DELAY_BEFORE_SHOW_ARROW, false);
        
        return true;
    }
}