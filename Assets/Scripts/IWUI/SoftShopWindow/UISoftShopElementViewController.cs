using UnityEngine;

public class UISoftShopElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#ButtonLabel")] private NSText btnLabel;
    [IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
    
    private bool isClick;
    private bool isCanPurchase;
    
    public override void Init()
    {
        base.Init();
        
        isClick = false;
        isCanPurchase = true;
        
        var contentEntity = entity as UISoftShopElementEntity;

        btnLabel.Text = contentEntity.ButtonLabel;
        
        btnBuy.Init()
            .ToState(GenericButtonState.Active)
            .OnClick(OnBuyClick);
    }
    
    public override void OnViewCloseCompleted()
    {
        var contentEntity = entity as UISoftShopElementEntity;
        
        if(entity == null) return;
        
        if (isClick == false)
        {
            if (isCanPurchase == false)
            {
                CurrencyHellper.OpenShopWindow(contentEntity.Price.Currency);
            }
            
            return;
        }
        
        CurrencyHellper.Purchase(contentEntity.Product, contentEntity.Price, null, new Vector2(Screen.width/2, Screen.height/2));
    }
    
    private void OnBuyClick()
    {
        if(isClick) return;
		
        isClick = true;
        
        var contentEntity = entity as UISoftShopElementEntity;
        
        if (CurrencyHellper.IsCanPurchase(contentEntity.Price) == false)
        {
            isCanPurchase = false;
            isClick = false;
        }
        
        context.Controller.CloseCurrentWindow();
    }
}