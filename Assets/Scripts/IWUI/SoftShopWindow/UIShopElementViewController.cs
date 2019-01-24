﻿using System.Collections.Generic;
using System.Linq;

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
        
        if(!(entity is UIShopElementEntity contentEntity) || entity == null || isClick == false) return;

        if (!IsBuyUsingCash())
        {
            CurrencyHelper.PurchaseAndProvideSpawn(contentEntity.Products, contentEntity.Price);
        }
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
        
        if(!(entity is UIShopElementEntity contentEntity) || entity == null || isClick == false) return;
        
        if(isCanPurchase) PlaySoundOnPurchase(contentEntity.Products);
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
    
    private void PlaySoundOnPurchase(List<CurrencyPair> products)
    {
        foreach (var product in products)
        {
            if(product.Currency == Currency.Energy.Name)
                NSAudioService.Current.Play(SoundId.BuyEnergy, false, 1);
            if(product.Currency == Currency.Coins.Name)
                NSAudioService.Current.Play(SoundId.BuySoftCurr, false, 1);    
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
                CurrencyHelper.PurchaseAndProvideSpawn(contentEntity.Products, contentEntity.Price);
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
        if (CurrencyHelper.IsCanPurchase(contentEntity.Price) == false)
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

        model.OnAcceptTap = () => PlaySoundOnPurchase(contentEntity.Products);
        model.OnAccept = context.Controller.CloseCurrentWindow;
        model.OnCancel = () => { isClick = false; };

        UIService.Get.ShowWindow(UIWindowType.ConfirmationWindow);
    }
}