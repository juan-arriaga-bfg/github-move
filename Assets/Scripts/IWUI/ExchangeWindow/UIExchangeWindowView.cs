using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;

public class UIExchangeWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#BuyButtonLabel")] private NSText buttonBuyLabel;
    [IWUIBinding("#FindButtonLabel")] private NSText buttonFindLabel;
    
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
    [IWUIBinding("#FindButton")] private UIButtonViewController btnFind;
    [IWUIBinding("#TutorAnchor")] private Transform tutorAnchor;
    
    private bool isClick;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        var isBuy = BoardService.Current.FirstBoard.TutorialLogic.CheckFirstOrder();
        
        btnBuy.gameObject.SetActive(isBuy);
        btnFind.gameObject.SetActive(!isBuy);
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        buttonBuyLabel.Text = windowModel.Button;
        buttonFindLabel.Text = windowModel.Button;

        isClick = false;
        
        Fill(UpdateEntities(windowModel.Products), content);
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnBuy, OnBuyClick);
        InitButtonBase(btnFind, OnFindClick);
        
        if (BoardService.Current.FirstBoard.TutorialLogic.CheckFirstOrder() == false) CachedHintArrowComponent.ShowArrow(tutorAnchor, 5f);
    }

    private List<IUIContainerElementEntity> UpdateEntities(List<CurrencyPair> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UISimpleScrollElementEntity
            {
                ContentId = def.Currency,
                LabelText = def.Amount.ToString(),
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }
    
    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        if(isClick == false) return;
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        CurrencyHelper.Purchase(windowModel.Products, windowModel.Price, success =>
        {
            if(success == false) return;
            
            Analytics.SendPurchase("buy_ingredients", "item1", new List<CurrencyPair>{windowModel.Price}, null, false, false);
            windowModel.OnClick?.Invoke();
        });
    }

    private void OnBuyClick()
    {
        if(isClick) return;
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        if(CurrencyHelper.IsCanPurchase(windowModel.Price, true) == false) return;

        isClick = true;
        
        Controller.CloseCurrentWindow();
    }

    private void OnFindClick()
    {
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;

        if (BoardService.Current.FirstBoard.BoardLogic.PositionsCache.GetCountByType(PieceType.PR_C4.Id) == 0)
        {
            HighlightFogHelper.HighlightNextFog(0);
            Controller.CloseCurrentWindow();
            UIService.Get.CloseWindow(UIWindowType.OrdersWindow, true);
            return;
        }
        
        foreach (var pair in windowModel.Products)
        {
            if(HighlightTaskPointToPieceHelper.FindAndPointToRandomPiece(PieceType.Parse(pair.Currency)) == false) continue;
            
            Controller.CloseCurrentWindow();
            UIService.Get.CloseWindow(UIWindowType.OrdersWindow, true);
            return;
        }
        
        foreach (var pair in windowModel.Products)
        {
            if(HighlightTaskPointToPieceSourceHelper.PointToPieceSource(PieceType.Parse(pair.Currency), PieceTypeFilter.ProductionField, PieceTypeFilter.Obstacle) == false) continue;
            
            Controller.CloseCurrentWindow();
            UIService.Get.CloseWindow(UIWindowType.OrdersWindow, true);
            return;
        }

        HighlightFogHelper.HighlightNextFog(0);
        Controller.CloseCurrentWindow();
        UIService.Get.CloseWindow(UIWindowType.OrdersWindow, true);
    }
}
