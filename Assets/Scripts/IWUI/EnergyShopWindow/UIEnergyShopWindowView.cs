using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class UIEnergyShopWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private GameObject itemPattern;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform content;
    
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
        content.anchoredPosition = new Vector2(-375, 0);
        scroll.enabled = false;
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();

        DOTween.Kill(content);
        content.DOAnchorPosX(0, 2.5f)
            .SetEase(Ease.InOutBack)
            .SetId(content)
            .OnComplete(() =>
            {
                scroll.enabled = true;
            });
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIEnergyShopWindowModel;
        DOTween.Kill(content);
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