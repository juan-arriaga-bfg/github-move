using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIProductionWindowView : IWUIWindowView
{
    [SerializeField] private RectTransform body;
    [SerializeField] private GameObject pattern;
    [SerializeField] private LayoutElement border;
    [SerializeField] private int index = 3;

    private const int open = -85;
    private const int close = 115;
    
    private bool isOpen;
    
    private List<UIProductionItem> items = new List<UIProductionItem>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        body.DOAnchorPosX(close, 0f).SetId(body);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIProductionWindowModel windowModel = Model as UIProductionWindowModel;
    }

    public void UpdateList()
    {
        var windowModel = Model as UIProductionWindowModel;

        var productions = windowModel.Productions;
        
        while (items.Count > productions.Count)
        {
            var i = items.Count - 1;
            var item = items[i];
            
            items.RemoveAt(i);
            Destroy(item.gameObject);
        }
        
        for (var i = 0; i < productions.Count; i++)
        {
            if (i == items.Count)
            {
                pattern.SetActive(true);
                var item = Instantiate(pattern, pattern.transform.parent).GetComponent<UIProductionItem>();
                items.Add(item);
            }
            
            items[i].Init(productions[i]);
        }
        
        pattern.SetActive(false);
        border.preferredHeight = items.Count > 4 ? 970 : -1;
    }
    
    public void OnClick()
    {
        DOTween.Kill(body, true);
        
        isOpen = !isOpen;
        
        if(isOpen) UpdateList();

        body.DOAnchorPosX(isOpen ? open : close, 0.3f).SetId(body).SetEase(Ease.OutBack);
    }
    
    public void SetPieceToUI(Transform pieceView)
    {
        var boardDef = BoardService.Current.GetBoardById(0).BoardDef;
        var boardCamera = boardDef.ViewCamera;
        
        var uiLayer = Controller.Window.Layers[0];
        var uiCamera = uiLayer.ViewCamera;
        
        // get screen position
        var screenPos = boardCamera.WorldToScreenPoint(pieceView.position);
        
        // get world position in UI space
        var worldUIPos = uiCamera.ScreenToWorldPoint(screenPos);
        
        // set parent and position
        pieceView.SetParent(transform);
        pieceView.position = worldUIPos;
        pieceView.localPosition = new Vector3(pieceView.localPosition.x, pieceView.localPosition.y, 0f);
        
        // set layer
        pieceView.SetLayerRecursive(uiLayer.CurrentLayer);
    }
}

