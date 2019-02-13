using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;

public class UIShopElementViewController : UISimpleScrollElementViewController
{
    [IWUIBindingNullable("#NameLabel")] protected NSText nameLabel;
    [IWUIBinding("#ButtonLabel")] protected NSText btnLabel;
    [IWUIBinding("#BuyButton")] protected UIButtonViewController btnBuy;
    
    private bool isClick;
    private BoardPosition? rewardPosition;

    private int piecesAmount;
    private Dictionary<int, int> piecesReward;
    private List<CurrencyPair> currenciesReward;
    
    public bool IsNeedReopen;

    private bool IsBuyUsingCash()
    {
        if (!(entity is UIShopElementEntity contentEntity)) return false;
        
        return contentEntity.Price?.Currency == Currency.Cash.Name;
    }
    
    public override void Init()
    {
        base.Init();
        
        isClick = false;
        IsNeedReopen = false;
        rewardPosition = null;
        piecesReward = null;
        currenciesReward = null;
        piecesAmount = 0;

        if(!(entity is UIShopElementEntity contentEntity)) return;
        
        piecesReward = CurrencyHelper.FiltrationRewards(contentEntity.Products, out currenciesReward);
        btnLabel.Text = contentEntity.ButtonLabel;

        piecesAmount = piecesReward.Sum(pair => pair.Value);

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
            CurrencyHelper.PurchaseAndProvideSpawn(piecesReward, currenciesReward, contentEntity.Price, rewardPosition, null, false, true);
            Analytics.SendPurchase(GetAnalyticLocation(), GetAnalyticReason(), new List<CurrencyPair>{contentEntity.Price}, new List<CurrencyPair>(currenciesReward), false, false);
        }
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
        
        if(!(entity is UIShopElementEntity contentEntity) || entity == null || isClick == false) return;
        
        PlaySoundOnPurchase(contentEntity.Products);
    }

    private void OnBuyClick()
    {
        if(isClick) return;
		
        var board = BoardService.Current.FirstBoard;
        
        if (board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceReward(piecesAmount, true, out var position) == false)
        {
            return;
        }
        
        rewardPosition = position;
        isClick = true;
        
        var contentEntity = entity as UIShopElementEntity;
        
        if (IsBuyUsingCash()) OnBuyUsingCash(contentEntity);
        else OnBuyUsingNoCash(contentEntity);
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
            var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

            model.Title = "[DEBUG]";
            model.Message = $"Product with id '{contentEntity.PurchaseKey}' not registered. Purchase will be processed using debug flow without real store.";
            model.AcceptLabel = LocalizationService.Get("common.button.ok", "common.button.ok");
            model.OnAccept = () =>
            {
                context.Controller.CloseCurrentWindow();
                CurrencyHelper.PurchaseAndProvideSpawn(piecesReward, currenciesReward, contentEntity.Price, rewardPosition, null, false, true);
                Analytics.SendPurchase(GetAnalyticLocation(), GetAnalyticReason(), new List<CurrencyPair>{contentEntity.Price}, new List<CurrencyPair>(currenciesReward), false, false);
            };
            model.OnClose = () =>
            {
                isClick = false;
            };

            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            return;
        }
        // END

        SellForCashService.Current.Purchase(contentEntity.PurchaseKey, (isOk, productId) =>
        {
            if (isOk == false)
            {
                isClick = false;
            }
            else
            {
                context.Controller.CloseCurrentWindow();
            }
        });
    }
    
    private void OnBuyUsingNoCash(UIShopElementEntity contentEntity)
    {
        if (CurrencyHelper.IsCanPurchase(contentEntity.Price) == false)
        {
            isClick = false;
            IsNeedReopen = true;
        }

        if (isClick == false || contentEntity.Price.Currency != Currency.Crystals.Name)
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

    protected virtual string GetAnalyticLocation()
    {
        var model = context.Model as UISoftShopWindowModel;
        
        if (model.ShopType.Id == Currency.Energy.Id) return "screen_energy";
        if (model.ShopType.Id == Currency.Coins.Id) return "screen_soft";

        return string.Empty;
    }

    private string GetAnalyticReason()
    {
        return $"item{CachedTransform.GetSiblingIndex()}";
    }
}