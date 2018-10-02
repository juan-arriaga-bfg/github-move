using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class UIChestsShopWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private GameObject itemPattern;
    [SerializeField] private NSText buttonShow;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform content;
    
    private List<UIChestsShopItem> items = new List<UIChestsShopItem>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIChestsShopWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        buttonShow.Text = windowModel.Button;
        
        var chests = windowModel.Chests;
        
        foreach (var chest in chests)
        {
            var item = Instantiate(itemPattern, itemPattern.transform.parent).GetComponent<UIChestsShopItem>();
            
            RegisterWindowViewController(item);
            
            item.Init(chest);
            items.Add(item);
        }
        
        itemPattern.SetActive(false);
        
        content.anchoredPosition = new Vector2(-375, 0);
        scroll.enabled = false;
        
        DOTween.Kill(content);
        content.DOAnchorPosX(0, 1.5f)
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
        
        var windowModel = Model as UIChestsShopWindowModel;
        
        DOTween.Kill(scroll);
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
        
        items = new List<UIChestsShopItem>();
    }
    
    public void OnClick()
    {
        Controller.CloseCurrentWindow();
    }
}