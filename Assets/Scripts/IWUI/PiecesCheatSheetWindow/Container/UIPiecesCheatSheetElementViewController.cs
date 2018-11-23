using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPiecesCheatSheetElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#NameLabel")] private NSText lblName;
    
    [IWUIBinding("#IdLabel")] private NSText lblId;
    
    [IWUIBinding("#Icon")] private Image ico;
    
    [IWUIBinding("#RootCanvas", true)] private CanvasGroup mainCanvasGroup;
    
    [IWUIBinding] private UIButtonViewController rootButton;

    private int pieceId;
    
    public override void Init()
    {
        base.Init();
        
        var targetEntity = entity as UIPiecesCheatSheetElementEntity;

        Init(targetEntity.PieceId);
    }
    
    public void Init(int pieceId)
    {
        this.pieceId = pieceId;

        if (pieceId <= 0)
        {
            lblName.gameObject.SetActive(false);
            ico.gameObject.SetActive(false);
            return;
        }
        
        // var pieceManager = GameDataService.Current.PiecesManager;
        PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);

        lblName.Text = pieceTypeDef.Abbreviations[0];
        lblId.Text = "id " + pieceId.ToString();

        var spr = GetPieecSprite();
        if (spr != null)
        {
            ico.sprite = spr;
        }

        rootButton
           .Init()
           .ToState(GenericButtonState.Active)
           .SetDragDirection(new Vector2(0f, 1f))
           .SetDragThreshold(10f)
           .OnBeginDrag(OnBeginDragEventHandler);
    }

    private void OnBeginDragEventHandler(UIButtonViewController obj, int pointerId)
    {
        if (BoardService.Current.FirstBoard.BoardLogic.DragAndDrop.IsActive) return;
  
        if (Input.touchSupported == false)
        {
            pointerId = 0;
        }
        
        BoardService.Current.FirstBoard.BoardLogic.DragAndDrop.Begin(pointerId, pieceId);
    }

    private void OnClickEventHandler(UIButtonViewController obj)
    {
        BoardPosition pos = new BoardPosition(23, 10, BoardService.Current.GetBoardById(0).BoardDef.PieceLayer);
        var availablePoints = new List<BoardPosition>();

        BoardService.Current.GetBoardById(0).BoardLogic.EmptyCellsFinder.FindNearWithPointInCenter(pos, availablePoints, 1, 100);
        if (availablePoints.Count > 0)
        {
            DOTween.Sequence()
                   .Append(mainCanvasGroup.DOFade(0f, 0.1f))
                   .AppendInterval(0.3f)
                   .AppendCallback(() =>
                    {
                        BoardService.Current.GetBoardById(0).ActionExecutor.AddAction(new CreatePieceAtAction
                        {
                            At = availablePoints[0],
                            PieceTypeId = pieceId
                        });
                    })
                   .AppendInterval(0.6f)
                   .Append(mainCanvasGroup.DOFade(1f, 0.1f));

        }
    }

    private Sprite GetPieecSprite()
    {
        return IconService.Current.GetSpriteById(PieceType.Parse(pieceId));
    }
}