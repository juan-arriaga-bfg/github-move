using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using UnityEngine;

public class UIOfferWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#ButtonBuyLabel")] private NSText buyLabel;
    [IWUIBinding("#SaleLabel")] private NSText sale;
    
    [IWUIBinding("#FakePrice")] private NSText priceFake;
    [IWUIBinding("#RealPrice")] private NSText priceReal;
    
    [IWUIBinding("#Product1")] private NSText product1;
    [IWUIBinding("#Product2")] private NSText product2;
    [IWUIBinding("#Product3")] private NSText product3;
    [IWUIBinding("#Product4")] private NSText product4;
    
    [IWUIBinding("#Line")] private GameObject line;
    
    [IWUIBinding("#ButtonBuy")] private UIButtonViewController btnBuy;

    private List<NSText> products;

    private bool isClick;

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        products = new List<NSText>
        {
            product1,
            product2,
            product3,
            product4,
        };
    }

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIOfferWindowModel;

        isClick = false;
        
        SetTitle(windowModel.Title);
        buyLabel.Text = windowModel.Button;
        
        priceFake.TextLabel.color = new Color(1, 1, 1, 0.5f);
        
        priceFake.Text = (windowModel.Product.Price.Amount + windowModel.Product.Price.Amount * windowModel.Product.Sale * 0.01f).ToString();
        priceReal.Text = windowModel.Product.Price.Amount.ToString();
        
        sale.Text = $"-{windowModel.Product.Sale}%";
        
        line.SetActive(windowModel.Product.Products.Count > 2);

        for (var i = 0; i < products.Count; i++)
        {
            var label = products[i];
            var isActive = i < windowModel.Product.Products.Count;

            label.gameObject.SetActive(isActive);

            if (isActive == false) break;

            label.Text = windowModel.Product.Products[i].ToStringIcon();
        }
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
