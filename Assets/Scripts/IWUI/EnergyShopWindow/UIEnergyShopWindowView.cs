using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class UIEnergyShopWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText secondMessage;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private GameObject itemPattern;
    
    private List<GameObject> items = new List<GameObject>();

    private bool isHint;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEnergyShopWindowModel;

        isHint = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        secondMessage.Text = windowModel.SecondMessage;
        buttonLabel.Text = windowModel.ButtonText;
        
        var products = windowModel.Products;
        
        foreach (var product in products)
        {
            var item = Instantiate(itemPattern, itemPattern.transform.parent).GetComponent<UIEnergyShopItem>();
            item.Init(product);
            items.Add(item.gameObject);
        }
        
        itemPattern.SetActive(false);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIEnergyShopWindowModel;
    }
    
    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        itemPattern.SetActive(true);

        foreach (var item in items)
        {
            Destroy(item);
        }
        
        items = new List<GameObject>();

        if (isHint)
        {
            Hint();
            return;
        }
        
        var windowModel = Model as UIEnergyShopWindowModel;
        windowModel.Spawn();
    }

    public void OnClick()
    {
        isHint = true;
        Controller.CloseCurrentWindow();
    }

    private void Hint()
    {
        var windowModel = Model as UIEnergyShopWindowModel;
        var ids = windowModel.SelectedPieces;
        var positions = new List<BoardPosition>();

        var board = BoardService.Current.GetBoardById(0);

        foreach (var id in ids)
        {
            positions.AddRange(board.BoardLogic.PositionsCache.GetPiecePositionsByType(id));
        }

        if (positions.Count == 0)
        {
            UIMessageWindowController.CreateImageMessage("Need more energy?", "collect_pieces", () => {});
            return;
        }
        
        HintArrowView.Show(positions[Random.Range(0, positions.Count)], 0, -0.5f);
    }
}