using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using UnityEngine;

public class UIOfferWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#ButtonBuyLabel")] private NSText buyLabel;
    [IWUIBinding("#FakePrice")] private NSText priceFake;
    [IWUIBinding("#RealPrice")] private NSText priceReal;
    [IWUIBinding("#Product")] private NSText product;
    [IWUIBinding("#SaleLabel")] private NSText sale;
    
    [IWUIBinding("#ButtonBuy")] private UIButtonViewController btnBuy;

    private bool isClick;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIOfferWindowModel;

        isClick = false;
        
        SetTitle(windowModel.Title);
        buyLabel.Text = windowModel.Button;

        priceFake.Text = (windowModel.Product.Price.Amount + windowModel.Product.Price.Amount * windowModel.Product.Sale * 0.01f).ToString();
        priceReal.Text = windowModel.Product.Price.Amount.ToString();
        
        sale.Text = $"-{windowModel.Product.Sale}%";
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
		
        btnBuy
            .ToState(GenericButtonState.Active)
            .OnClick(OnClick);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIOfferWindowModel;
    }

    private void OnClick()
    {
        if (isClick) return;
		
        isClick = true;
        
        var windowModel = Model as UIOfferWindowModel;
	    
        // HACK to handle the case when we have a purchase but BFG still not add it to the Store
        if (IapService.Current.IapCollection.Defs.All(e => e.Id != windowModel.Product.PurchaseKey))
        {
            var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

            model.Title = "[DEBUG]";
            model.Message = $"Product with id '{windowModel.Product.PurchaseKey}' not registered. Purchase will be processed using debug flow without real store.";
            model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
            model.OnAccept = OnPurchase;
            model.OnClose = () => { isClick = false; };

            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            return;
        }
        // END

        SellForCashService.Current.Purchase(windowModel.Product.PurchaseKey, (isOk, productId) =>
        {
            isClick = false;
		    
            if (isOk == false) return;
            
            PurchaseComplete();
        });
    }

    private void OnPurchase()
    {
        var windowModel = Model as UIOfferWindowModel;
        var flyPosition = GetComponentInChildren<Canvas>().worldCamera.WorldToScreenPoint(btnBuy.transform.position);
        
        var transactions = CurrencyHelper.PurchaseAsync(windowModel.Product.Products, success =>
        {
            if (success == false ) return;
		    
            isClick = false;
        }, flyPosition);

        foreach (var transaction in transactions)
        {
            transaction.Complete();
        }

        PurchaseComplete();
    }

    private void PurchaseComplete()
    {
        var windowModel = Model as UIOfferWindowModel;
        
        CurrencyHelper.Purchase(Currency.Offer.Name, 1);
        Analytics.SendPurchase($"shop_{Currency.Offer.Name.ToLower()}", "item1", new List<CurrencyPair>{windowModel.Product.Price}, new List<CurrencyPair>(windowModel.Product.Products), true, false);
        Controller.CloseCurrentWindow();
    }
}
