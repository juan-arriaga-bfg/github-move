using UnityEngine;

public class UISoftShopElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#ButtonLabel")] private NSText btnLabel;
    [IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
    
    private bool isClick;
    
    public override void Init()
    {
        base.Init();
        
        isClick = false;
        
        var contentEntity = entity as UISoftShopElementEntity;

        btnLabel.Text = contentEntity.ButtonLabel;
        
        btnBuy.Init()
            .ToState(GenericButtonState.Active)
            .OnClick(OnBuyClick);
    }
    
    public override void OnViewCloseCompleted()
    {
        if (isClick == false) return;

        var contentEntity = entity as UISoftShopElementEntity;
        
        CurrencyHellper.Purchase(contentEntity.Product, contentEntity.Price, null, new Vector2(Screen.width/2, Screen.height/2));
    }
    
    private void OnBuyClick()
    {
        if(isClick) return;
		
        isClick = true;
        
        var contentEntity = entity as UISoftShopElementEntity;
        
        if (CurrencyHellper.IsCanPurchase(contentEntity.Price, true) == false)
        {
            isClick = false;
            return;
        }
        
        context.Controller.CloseCurrentWindow();
    }
}