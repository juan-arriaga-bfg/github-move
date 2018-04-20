using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UISimpleQuestStartWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText subTitle;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private Image chest;
    [SerializeField] private Image cap;
    
    [SerializeField] private Button ClaimButton;
    
    [SerializeField] private GameObject itemTemplate;
    
    private List<UiSimpleQuestStartItem> items;
    
    private Quest quest;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UISimpleQuestStartWindowModel;

        quest = windowModel.Quest;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        subTitle.Text = windowModel.SubTitle;
        buttonLabel.Text = windowModel.ButtonText;
        
        var id = windowModel.GetChestSkin();
        
        chest.sprite = IconService.Current.GetSpriteById(id + "_1");
        cap.sprite = IconService.Current.GetSpriteById(id + "_2");
        
        UpdateItems(windowModel);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UISimpleQuestStartWindowModel;
    }

    public void OnClick()
    {
        GameDataService.Current.QuestsManager.RemoveActiveQuest(quest);
        GameDataService.Current.QuestsManager.AddActiveQuest();
        
        Controller.CloseCurrentWindow();
        
        var main = UIService.Get.GetShowedWindowByName(UIWindowType.MainWindow).CurrentView as UIMainWindowView;
				
        main.UpdateQuest();
        
        quest.Complete(() => { CurrencyHellper.Purchase(quest.Reward); });
    }
    
    private void UpdateItems(UISimpleQuestStartWindowModel model)
    {
        if (items == null)
        {
            /*for (var i = 1; i < 3; i++)
            {
                Instantiate(itemTemplate, itemTemplate.transform.parent);
            }*/

            items = itemTemplate.transform.parent.GetComponentsInChildren<UiSimpleQuestStartItem>().ToList();
        }
        
        for (var i = 0; i < items.Count; i++)
        {
            items[i].gameObject.SetActive(true);
            items[i].Init(quest);
        }
        
        ClaimButton.interactable = quest.Check();
    }
}
