using UnityEngine;
using System.Collections.Generic;

public class UIEnergyShopWindowView : UIGenericPopupWindowView 
{   
    [SerializeField] private GameObject itemPattern;
    
    private List<UIEnergyShopItem> items = new List<UIEnergyShopItem>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEnergyShopWindowModel;
        
        SetTitle(windowModel.Title);
        
        var products = windowModel.Products;
        
        foreach (var product in products)
        {
            var item = Instantiate(itemPattern, itemPattern.transform.parent).GetComponent<UIEnergyShopItem>();
            
            RegisterWindowViewController(item);
            
            item.Init(product);
            items.Add(item);
        }
        
        itemPattern.SetActive(false);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIEnergyShopWindowModel;
    }
    
    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        itemPattern.SetActive(true);

        foreach (var item in items)
        {
            UnRegisterWindowViewController(item);
            Destroy(item.gameObject);
        }
        
        items = new List<UIEnergyShopItem>();
    }
}