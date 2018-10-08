using UnityEngine;
using System.Collections.Generic;

public class UIExchangeWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText buttonBuyLabel;
    [SerializeField] private GameObject pattern;
    
    private List<UIExchangeWindowItem> items = new List<UIExchangeWindowItem>();

    private bool isClick;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        buttonBuyLabel.Text = windowModel.Button;

        isClick = false;
        
        var products = windowModel.Products;
        
        foreach (var product in products)
        {
            var item = Instantiate(pattern, pattern.transform.parent).GetComponent<UIExchangeWindowItem>();
            
            RegisterWindowViewController(item);
            
            item.Init(product);
            items.Add(item);
        }
        
        pattern.SetActive(false);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
    }
    
    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        pattern.SetActive(true);

        foreach (var item in items)
        {
            UnRegisterWindowViewController(item);
            Destroy(item.gameObject);
        }
        
        items = new List<UIExchangeWindowItem>();
        
        if(isClick == false) return;
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        CurrencyHellper.Purchase(windowModel.Products, windowModel.Price, success =>
        {
            if(success == false) return;
            
            windowModel.OnClick?.Invoke();
        });
    }

    public void OnClick()
    {
        if(isClick) return;
        
        UIExchangeWindowModel windowModel = Model as UIExchangeWindowModel;
        
        if(CurrencyHellper.IsCanPurchase(windowModel.Price, true) == false) return;

        isClick = true;
        
        Controller.CloseCurrentWindow();
    }
}
