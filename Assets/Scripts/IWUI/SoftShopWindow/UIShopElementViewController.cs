using System.Collections.Generic;
using UnityEngine;

public class UIShopElementViewController : UISimpleScrollElementViewController
{
    [IWUIBindingNullable("#NameLabel")] protected NSText nameLabel;
    [IWUIBinding("#ButtonLabel")] protected NSText btnLabel;
    [IWUIBinding("#BuyButton")] protected UIButtonViewController btnBuy;
    
    private bool isClick;
    private bool isCanPurchase;
    
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
        
        if(entity == null) return;
        
        if (isClick == false)
        {
            if (isCanPurchase == false)
            {
                CurrencyHellper.OpenShopWindow(contentEntity.Price.Currency);
            }
            
            return;
        }
        
        CurrencyHellper.Purchase(contentEntity.Products, contentEntity.Price, success =>
        {
            
            GetReward();
        });
    }
    
    private void OnBuyClick()
    {
        if(isClick) return;
		
        isClick = true;
        
        var contentEntity = entity as UIShopElementEntity;
        
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

    private void GetReward()
    {
        var board = BoardService.Current.FirstBoard;
        var positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceTypeFilter.Character, 1);
        
        if(positions.Count == 0) return;

        var position = positions[0];
        var contentEntity = entity as UIShopElementEntity;
        
        List<CurrencyPair> currencysReward;
        var piecesReward = CurrencyHellper.FiltrationRewards(contentEntity.Products, out currencysReward);

        foreach (var currency in currencysReward)
        {
            if(currency.Currency == Currency.Energy.Name)
                NSAudioService.Current.Play(SoundId.buy_energy, false, 1);
            if(currency.Currency == Currency.Coins.Name)
                NSAudioService.Current.Play(SoundId.buy_soft_curr, false, 1);
        }
        
        board.ActionExecutor.AddAction(new EjectionPieceAction
        {
            GetFrom = () => position,
            Pieces = piecesReward,
            OnComplete = () =>
            {
                var view = board.RendererContext.GetElementAt(position) as CharacterPieceView;
                
                if(view != null) view.StartRewardAnimation();
                    
                AddResourceView.Show(position, currencysReward);
            }
        });
    }
}