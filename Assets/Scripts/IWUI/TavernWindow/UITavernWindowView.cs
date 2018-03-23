using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UITavernWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText subTitle;
    [SerializeField] private NSText labelAbility;
    
    [SerializeField] private Image abilityIcon;
    
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

        if (windowModel.CurrentAbility == null)
        {
            labelAbility.gameObject.SetActive(false);
            abilityIcon.gameObject.SetActive(false);
        }
        else
        {
            labelAbility.gameObject.SetActive(true);
            abilityIcon.gameObject.SetActive(true);

            labelAbility.Text = windowModel.CurrentAbility.Value.ToString();
            abilityIcon.sprite = IconService.Current.GetSpriteById(windowModel.CurrentAbility.Ability.ToString());
        }
        
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
        windowModel.Obstacle = null;
        windowModel.CurrentAbility = null;
    }

    private void UpdateItems(UITavernWindowModel model)
    {
        var heroes = model.Heroes();

        if (items == null)
        {
            for (var i = 1; i < heroes.Count + 2; i++)
            {
                CreateItem(heroParent, heroTemplate);
            }

            items = heroParent.GetComponentsInChildren<UiHeroItem>().ToList();
        }
        
        for (var i = 0; i < items.Count; i++)
        {
            var hero = i < heroes.Count ? heroes[i] : null;
            
            items[i].Init(hero, model.Obstacle, model.CurrentAbility);
        }
    }

    private void CreateItem(Transform parent, GameObject template)
    {
        var go = Instantiate(template, parent);
        go.transform.localScale = Vector3.one;
    }
}
