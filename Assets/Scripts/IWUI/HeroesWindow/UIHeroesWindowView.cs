using UnityEngine;
using System.Collections.Generic;

public class UIHeroesWindowView : UIGenericPopupWindowView
{
    [SerializeField] private GameObject item;

    private List<UIHeroItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIHeroesWindowModel;
        
        SetTitle(windowModel.Title);
        
        var heroes = windowModel.Heroes;

        if (items == null)
        {
            items = new List<UIHeroItem> {item.GetComponent<UIHeroItem>()};
        }

        if (items.Count < heroes.Count)
        {
            for (var i = items.Count; i < heroes.Count; i++)
            {
                items.Add(CreateItem());
            }
        }

        if (items.Count > heroes.Count)
        {
            for (var i = items.Count - 1; i >= heroes.Count; i--)
            {
                var hero = items[i];
                
                items.RemoveAt(i);
                Destroy(hero.gameObject);
            }
        }

        for (var i = 0; i < items.Count; i++)
        {
            var hero = items[i];
            
            hero.Decoration(heroes[i]);
        }
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIHeroesWindowModel;
    }

    private UIHeroItem CreateItem()
    {
        var hero = Instantiate(item, item.transform.parent);

        return hero.GetComponent<UIHeroItem>();
    }
}
