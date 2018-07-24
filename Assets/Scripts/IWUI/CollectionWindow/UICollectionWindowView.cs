using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UICollectionWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private Image icon;
    [SerializeField] private Image back;
    
    [SerializeField] private GameObject button;

    private List<UiCollectionItem> items;
    
    public override void OnViewShow()
    {
        var windowModel = Model as UICollectionWindowModel;

        if (items == null)
        {
            items = GetComponentsInChildren<UiCollectionItem>().ToList();
        }

        var collection = windowModel.GetCollection();

        for (var i = 0; i < items.Count; i++)
        {
            items[i].Decoration(collection[i]);
        }
        
        base.OnViewShow();
        Decoration();
    }

    public override void OnViewShowCompleted()
    {
        var windowModel = Model as UICollectionWindowModel;
        if (windowModel.OnOpen != null) windowModel.OnOpen();
        Decoration();
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UICollectionWindowModel;
    }

    public void Decoration()
    {
        var windowModel = Model as UICollectionWindowModel;

        var isCollect = windowModel.IsCollectAll;
        
        icon.sprite = IconService.Current.GetSpriteById(string.Format("{0}_head" , windowModel.Hero));
        back.sprite = IconService.Current.GetSpriteById(string.Format("back_{0}active" , isCollect ? "" : "not_"));
        
        button.gameObject.SetActive(isCollect);

        back.raycastTarget = isCollect;
        
        if(isCollect == false) return;
        
        foreach (var item in items)
        {
            item.AllActive();
        }
    }

    public void OnClick()
    {
        var windowModel = Model as UICollectionWindowModel;

        if (windowModel.IsCollectAll)
        {
            UIService.Get.ShowWindow(UIWindowType.HeroesWindow);
        }
        
        Controller.CloseCurrentWindow();
    }
}
