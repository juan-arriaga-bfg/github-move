using UnityEngine;
using System.Collections.Generic;

public class UIEnergyShopWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private GameObject itemPattern;
    
    private List<GameObject> items = new List<GameObject>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEnergyShopWindowModel;
        
        SetTitle(windowModel.Title);
        
        var products = windowModel.Products;
        
        foreach (var product in products)
        {
            var item = Instantiate(itemPattern, itemPattern.transform.parent).GetComponent<UIEnergyShopItem>();
            item.Init(product);
            items.Add(item.gameObject);
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
            Destroy(item);
        }
        
        items = new List<GameObject>();
    }
}