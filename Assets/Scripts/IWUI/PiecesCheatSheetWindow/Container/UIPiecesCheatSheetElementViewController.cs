﻿using System.Collections.Generic;
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
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        rootButton
            .ToState(GenericButtonState.Active)
            .SetDragDirection(new Vector2(0f, 1f))
            .SetDragThreshold(100f)
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
        BoardPosition pos = new BoardPosition(23, 10, BoardLayer.Piece.Layer);
        var availablePoints = new List<BoardPosition>();

        BoardService.Current.FirstBoard.BoardLogic.EmptyCellsFinder.FindNearWithPointInCenter(pos, availablePoints, 1, 100);
        if (availablePoints.Count > 0)
        {
            var window = context as UIBaseWindowView;
            
            DOTween.Sequence()
                   .Append(window.GetCanvasGroup().DOFade(0f, 0.1f))
                   .AppendInterval(0.3f)
                   .AppendCallback(() =>
                    {
                        BoardService.Current.FirstBoard.ActionExecutor.AddAction(new CreatePieceAtAction
                        {
                            At = availablePoints[0],
                            PieceTypeId = pieceId
                        });
                    })
                   .AppendInterval(0.6f)
                   .Append(window.GetCanvasGroup().DOFade(1f, 0.1f));
        }
    }

    private Sprite GetPieecSprite()
    {
        return IconService.Current.GetSpriteById(PieceType.Parse(pieceId));
    }
}