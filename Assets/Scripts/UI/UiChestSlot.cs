using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiChestSlot : MonoBehaviour
{
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private NSText timerLabel;
    
    [SerializeField] private Image top;
    [SerializeField] private Image bottom;
    [SerializeField] private Image button;
    
    [SerializeField] private GameObject currencyIcon;
    [SerializeField] private GameObject timerGo;
    [SerializeField] private GameObject chestGo;
    [SerializeField] private GameObject buttonGo;
    [SerializeField] private GameObject shineGo;

    private Chest chest;

    private bool isTimerWork;
    
    public void Init(Chest chest)
    {
        isTimerWork = false;
        
        if (chest == null)
        {
            timerGo.SetActive(false);
            chestGo.SetActive(false);
            buttonGo.SetActive(false);
            
            return;
        }

        this.chest = chest;
        
        var state = chest.State;
        
        chestGo.SetActive(true);
        timerGo.SetActive(state == ChestState.InProgress);
        buttonGo.SetActive(true);
        currencyIcon.SetActive(state == ChestState.InProgress);
        shineGo.SetActive(state == ChestState.Open);
        
        top.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, state == ChestState.Open ? 45 : 15);
        
        var id = chest.GetSkin();
        var color = "FAF9ADFF";
        
        top.sprite = IconService.Current.GetSpriteById(id + "_2");
        bottom.sprite = IconService.Current.GetSpriteById(id + "_1");

        switch (state)
        {
            case ChestState.Close:
                buttonLabel.Text = string.Format("<color=#{1}>UNLOCK IN </color><size=35>{0}</size>", chest.GetTimeText(), color);
                button.sprite = IconService.Current.GetSpriteById("btn_orange_norm");
                break;
            case ChestState.InProgress:
                buttonLabel.Text = string.Format("<size=40>{0}</size> <color=#{1}>OPEN</color>", this.chest.Def.Price.Amount, color);
                button.sprite = IconService.Current.GetSpriteById("btn_blue_norm");
                break;
            case ChestState.Open:
                buttonLabel.Text = string.Format("<color=#{0}>TAP TO OPEN</color>", color);
                button.sprite = IconService.Current.GetSpriteById("btn_green_norm");
                break;
            case ChestState.Finished:
                break;
        }
    }

    private void Update()
    {
        if (chest == null || chest.State != ChestState.InProgress)
        {
            if (isTimerWork)
            {
                isTimerWork = false;
                Init(chest);
            }
            
            return;
        }

        timerLabel.Text = chest.GetTimeLeftText();
        isTimerWork = true;
    }

    public void OnClick()
    {
        switch (chest.State)
        {
            case ChestState.Close:
                var modelMessage1 = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
                modelMessage1.Title = "Message";
                modelMessage1.Message = "Start the chest?";
                modelMessage1.AcceptLabel = "Ok";
                
                modelMessage1.OnAccept = () =>
                {
                    chest.State = ChestState.InProgress;
                    Init(chest);
                };
        
                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
                
                break;
            case ChestState.InProgress:
                var modelMessage2 = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
                modelMessage2.Title = "Message";
                modelMessage2.Message = string.Format("Are you sure you want to spend {0} crystals to Speed Up the chest?", 1);
                modelMessage2.AcceptLabel = "Ok";
                modelMessage2.CancelLabel = "Cancel";

                modelMessage2.OnCancel = () => {};
                modelMessage2.OnAccept = () =>
                {
                    var shopItem = new ShopItem
                    {
                        Uid = string.Format("purchase.test.{0}.10", chest.Currency), 
                        ItemUid = chest.Currency, 
                        Amount = 1,
                        CurrentPrices = new List<Price>
                        {
                            new Price{Currency = chest.Def.Price.Currency, DefaultPriceAmount = chest.Def.Price.Amount}
                        }
                    };
        
                    ShopService.Current.PurchaseItem
                    (
                        shopItem,
                        (item, s) =>
                        {
                            // on purchase success
                            chest.State = ChestState.Open;
                            Init(chest);
                        },
                        item =>
                        {
                            // on purchase failed (not enough cash)
                            UIMessageWindowController.CreateDefaultMessage("Not enough crystals!");
                        }
                    );
                };
        
                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
                break;
            case ChestState.Open:
                chest.State = ChestState.Finished;
                GameDataService.Current.ChestsManager.RemoveActiveChest(chest);
                
                var model = UIService.Get.GetCachedModel<UIChestRewardWindowModel>(UIWindowType.ChestRewardWindow);

                model.Chest = GameDataService.Current.ChestsManager.GetChest(chest.ChestType);
        
                UIService.Get.ShowWindow(UIWindowType.ChestRewardWindow);
                
                var main = UIService.Get.GetShowedWindowByName(UIWindowType.MainWindow).CurrentView as UIMainWindowView;
				
                main.UpdateSlots();
                break;
            case ChestState.Finished:
                break;
        }
    }
}