using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UITavernWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText subTitle;
    
    [SerializeField] private Transform heroParent;
    [SerializeField] private GameObject heroTemplate;

    private List<UiHeroItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UITavernWindowModel windowModel = Model as UITavernWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        subTitle.Text = windowModel.SubTitle;
        
        UpdateItems(windowModel);
    }

    public override void UpdateView(IWWindowModel model)
    {
        base.UpdateView(model);

        UpdateItems(model as UITavernWindowModel);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UITavernWindowModel windowModel = Model as UITavernWindowModel;
    }

    private void UpdateItems(UITavernWindowModel model)
    {
        var heroes = model.Heroes();

        if (items == null)
        {
            for (var i = 1; i < heroes.Count; i++)
            {
                CreateItem(heroParent, heroTemplate);
            }

            items = heroParent.GetComponentsInChildren<UiHeroItem>().ToList();
        }
        
        for (var i = 0; i < heroes.Count; i++)
        {
            items[i].Init(heroes[i], model.Obstacle);
        }
    }

    private void CreateItem(Transform parent, GameObject template)
    {
        var go = Instantiate(template, parent);
        go.transform.localScale = Vector3.one;
    }
}
