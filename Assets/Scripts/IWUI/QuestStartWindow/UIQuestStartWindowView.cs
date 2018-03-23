using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestStartWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText subTitle;
    [SerializeField] private NSText timeLabel;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private Image chest;
    [SerializeField] private Image cap;
    
    [SerializeField] private Button StartButton;
    
    [SerializeField] private GameObject itemTemplate;

    private bool InAdventure;

    private List<UiQuestStartItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIQuestStartWindowModel;

        InAdventure = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        subTitle.Text = windowModel.SubTitle;
        timeLabel.Text = windowModel.TimeText;
        buttonLabel.Text = windowModel.ButtonText;
        
        var id = windowModel.GetChestSkin();
        
        chest.sprite = IconService.Current.GetSpriteById(id + "_1");
        cap.sprite = IconService.Current.GetSpriteById(id + "_2");

        UpdateItems(windowModel);
    }
    
    public override void UpdateView(IWWindowModel model)
    {
        base.UpdateView(model);

        UpdateItems(model as UIQuestStartWindowModel);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIQuestStartWindowModel;
        
        if(InAdventure) return;

        foreach (var item in items)
        {
            item.HeroGoHome();
        }
    }

    public void OnClick()
    {
        var windowModel = Model as UIQuestStartWindowModel;
        
        InAdventure = true;
        windowModel.Obstacle.State = ObstacleState.InProgres;
        Controller.CloseCurrentWindow();
    }

    private void UpdateItems(UIQuestStartWindowModel model)
    {
        var conditions = model.GetConditionHeroes();

        if (items == null)
        {
            for (var i = 1; i < conditions.Count; i++)
            {
                var go = Instantiate(itemTemplate, itemTemplate.transform.parent);
            }

            items = itemTemplate.transform.parent.GetComponentsInChildren<UiQuestStartItem>().ToList();
        }
        
        for (var i = 0; i < items.Count; i++)
        {
            if (i >= conditions.Count)
            {
                items[i].gameObject.SetActive(false);
                continue;
            }
            
            items[i].gameObject.SetActive(true);
            items[i].Init(model.Obstacle, conditions[i].HeroAbility);
        }
        
        StartButton.interactable = true;
        
        foreach (var item in items)
        {
            if (item.InAdventure()) continue;
            
            StartButton.interactable = false;
            return;
        }
    }
}
