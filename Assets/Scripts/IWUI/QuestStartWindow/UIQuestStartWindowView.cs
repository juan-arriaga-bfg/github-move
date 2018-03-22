using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestStartWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText subTitle;
    [SerializeField] private NSText timeLabel;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private Image chest;
    [SerializeField] private Image cap;

    [SerializeField] private List<UiQuestStartToggle> toggles;
    
    [SerializeField] private Button StartButton;

    private bool InAdventure;

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

        foreach (var obj in toggles)
        {
            obj.HeroInHome();
        }
    }

    public void OnClick()
    {
        var windowModel = Model as UIQuestStartWindowModel;
        
        var price = windowModel.Obstacle.Def.StartPrice;
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", Currency.Quest.Name), 
            ItemUid = Currency.Quest.Name, 
            Amount = 1,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount}
            }
        };

        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                InAdventure = true;
                windowModel.Obstacle.State = ObstacleState.InProgres;
                Controller.CloseCurrentWindow();
            },
            item =>
            {
                // on purchase failed (not enough cash)
                UIMessageWindowController.CreateDefaultMessage("Not enough coins!");
            }
        );
    }

    private void UpdateItems(UIQuestStartWindowModel model)
    {
        var conditions = model.GetConditionHeroes();
        
        foreach (var toggle in toggles)
        {
            var isActive = conditions.Find(hero => hero.Hero == toggle.heroName) != null;
            
            toggle.Init(model.Obstacle);
            toggle.gameObject.SetActive(isActive);
        }
        
        StartButton.interactable = true;
        
        foreach (var obj in toggles)
        {
            if (obj.InAdventure() == false)
            {
                StartButton.interactable = false;
                break;
            }
        }
    }
}
