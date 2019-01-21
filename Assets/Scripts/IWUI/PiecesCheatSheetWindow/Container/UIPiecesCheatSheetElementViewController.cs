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
    
    [IWUIBinding("#Anchor")] private Transform anchor;
    
    [IWUIBinding] private UIButtonViewController rootButton;

    private int pieceId;
    private Transform content;
    
    public override void Init()
    {
        base.Init();
        
        var targetEntity = entity as UIPiecesCheatSheetElementEntity;
        
        pieceId = targetEntity.PieceId;
        
        PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);

        lblName.Text = pieceTypeDef.Abbreviations[0];
        lblId.Text = $"id {pieceId}";
        CreateIcon(PieceType.Parse(pieceId));
        
        rootButton
            .ToState(GenericButtonState.Active)
            .SetDragDirection(new Vector2(0f, 1f))
            .SetDragThreshold(100f)
            .OnBeginDrag(OnBeginDragEventHandler);
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
  
        if (Input.touchSupported == false) pointerId = 0;
        
        BoardService.Current.FirstBoard.BoardLogic.DragAndDrop.Begin(pointerId, pieceId);
    }

    private void CreateIcon(string id)
    {
        if (anchor == null) return;
        if (content != null) UIService.Get.PoolContainer.Return(content.gameObject);
        
        content = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        content.SetParentAndReset(anchor);
    }
}