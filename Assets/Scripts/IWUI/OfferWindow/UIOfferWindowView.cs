using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using TMPro;
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
    
    private VertexGradient hardGradient;
    private VertexGradient otherGradient;
    
    private bool isClick;

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        ColorUtility.TryParseHtmlString("#FF5FEF", out var hardTop);
        ColorUtility.TryParseHtmlString("#E17AFF", out var hardBottom);
        
        ColorUtility.TryParseHtmlString("#FFD45F", out var otherTop);
        ColorUtility.TryParseHtmlString("#FF9241", out var otherBottom);
        
        hardGradient = new VertexGradient(hardTop, hardTop, hardBottom, hardBottom);
        otherGradient = new VertexGradient(otherTop, otherTop, otherBottom, otherBottom);
        
        base.InitView(model, controller);
    }

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIOfferWindowModel;
        var count = windowModel.Product.Products.Count;
        
        products = new List<NSText>
        {
            product1,
            product2,
            product3,
            product4,
        };
        
        if (count == 3) products.RemoveAt(1);
        
        isClick = false;
        
        SetTitle(windowModel.Title);
        buyLabel.Text = windowModel.Button;
        
        priceFake.TextLabel.color = new Color(1, 1, 1, 0.5f);

        priceFake.Text = windowModel.PriceFake;
        priceReal.Text = windowModel.PriceReal;
        
        sale.Text = $"-{windowModel.Product.Sale}%";
        
        line.SetActive(count > 2);

        for (var i = 0; i < products.Count; i++)
        {
            var label = products[i];
            var isActive = i < count;

            label.gameObject.SetActive(isActive);

            if (isActive == false) break;

            var product = windowModel.Product.Products[i];
            var isCrystals = product.Currency == Currency.Crystals.Name;

            label.Text = product.ToStringIcon();
            label.TextLabel.colorGradient = isCrystals ? hardGradient : otherGradient;
            label.StyleId = isCrystals ? 23 : 24;
            label.ApplyStyle();
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

        CurrencyHelper.PurchaseAsyncOnlyCurrency(windowModel.Product.Products, flyPosition, null);
	    
        isClick = false;
        PurchaseComplete();
    }

    private void PurchaseComplete()
    {
        var windowModel = Model as UIOfferWindowModel;
        
        CurrencyHelper.Purchase(Currency.Offer.Name, 1);
        Analytics.SendPurchase($"shop_{Currency.Offer.Name.ToLower()}", $"item{windowModel.ProductIndex}", new List<CurrencyPair>{windowModel.Product.Price}, new List<CurrencyPair>(windowModel.Product.Products), true, false);
        Controller.CloseCurrentWindow();
    }
}
