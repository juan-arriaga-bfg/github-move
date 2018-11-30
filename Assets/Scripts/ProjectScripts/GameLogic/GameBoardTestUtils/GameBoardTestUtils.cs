using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardTestUtils : MonoBehaviour
{
    public void GenerateTestField()
    {
        var board = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;
        var boardRender = board.RendererContext;
       
        var clearFieldAction = new ClearBoardAction();
        board.ActionExecutor.AddAction(clearFieldAction, BoardActionMode.SingleMode, 100);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            GenerateTestField();
        }
    }
}
