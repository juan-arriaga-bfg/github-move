using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UIShopElementViewController : UISimpleScrollElementViewController
{
    [IWUIBindingNullable("#NameLabel")] protected NSText nameLabel;
    [IWUIBinding("#ButtonLabel")] protected NSText btnLabel;
    [IWUIBinding("#BuyButton")] protected UIButtonViewController btnBuy;
    
    private bool isClick;
    private bool isCanPurchase;
    
    public bool IsNeedReopen => isClick == false && isCanPurchase == false;

    private bool IsBuyUsingCash()
    {
        var contentEntity = entity as UIShopElementEntity;
        return contentEntity.Price.Currency == Currency.Cash.Name;
    }
    
    public override void Init()
    {
        base.Init();
        
        isClick = false;
        isCanPurchase = true;
        
        var contentEntity = entity as UIShopElementEntity;

        btnLabel.Text = contentEntity.ButtonLabel;

        if (nameLabel != null) nameLabel.Text = contentEntity.NameLabel;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        btnBuy
            .ToState(GenericButtonState.Active)
            .OnClick(OnBuyClick);
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        var contentEntity = entity as UIShopElementEntity;
        
        if(entity == null || isClick == false) return;

        if (!IsBuyUsingCash())
        {
            CurrencyHellper.PurchaseAndProvide(contentEntity.Products, contentEntity.Price);
        }
    }
    
    private void OnBuyClick()
    {
        if(isClick) return;
		
        isClick = true;
        
        var contentEntity = entity as UIShopElementEntity;

        if (IsBuyUsingCash())
        {
            OnBuyUsingCash(contentEntity);
        }
        else
        {
            OnBuyUsingNoCash(contentEntity);
        }
    }

    private void OnBuyUsingCash(UIShopElementEntity contentEntity)
    {
        // HACK to handle the case when we have a purchase but BFG still not add it to the Store
        if (IapService.Current.IapCollection.Defs.All(e => e.Id != contentEntity.PurchaseKey))
        {
            context.Controller.CloseCurrentWindow();
            
            var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

            model.Title = "[DEBUG]";
            model.Message = $"Product with id '{contentEntity.PurchaseKey}' not registered. Purchase will be processed using debug flow without real store.";
            model.AcceptLabel = LocalizationService.Get("common.button.ok",            "common.button.ok");

            model.OnAccept = () =>
            {
                CurrencyHellper.PurchaseAndProvide(contentEntity.Products, contentEntity.Price);
            };

            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            return;
        }
        // END

        SellForCashService.Current.Purchase(contentEntity.PurchaseKey, (isOk, productId) =>
        {
            if (isOk)
            {
                context.Controller.CloseCurrentWindow();
            }

            isClick = false;
        });
    }
    
    private void OnBuyUsingNoCash(UIShopElementEntity contentEntity)
    {
        if (CurrencyHellper.IsCanPurchase(contentEntity.Price) == false)
        {
            isCanPurchase = false;
            isClick = false;
        }

        if (isCanPurchase == false || contentEntity.Price.Currency != Currency.Crystals.Name)
        {
            context.Controller.CloseCurrentWindow();
            return;
        }

        var model = UIService.Get.GetCachedModel<UIConfirmationWindowModel>(UIWindowType.ConfirmationWindow);

        model.IsMarket = false;
        model.Icon = contentEntity.ContentId;

        model.Price = contentEntity.Price;
        model.Product = contentEntity.Products[0];

        model.OnAccept = context.Controller.CloseCurrentWindow;
        model.OnCancel = () => { isClick = false; };

        UIService.Get.ShowWindow(UIWindowType.ConfirmationWindow);
    }
}