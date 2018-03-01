using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChestWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText chestTypeLabel;
    [SerializeField] private NSText btnOpenLabel;
    [SerializeField] private NSText btnStartLabel;
    
    [SerializeField] private NSText timerLabel;
    [SerializeField] private NSText timerLengthLabel;
    
    [SerializeField] private GameObject btnOpen;
    [SerializeField] private GameObject btnStart;
    [SerializeField] private GameObject timer;
    
    [SerializeField] private RectTransform IconBox;
    [SerializeField] private RectTransform NameBox;
    
    [SerializeField] private Image chest;
    
    private ChestDef def;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIChestWindowModel;
        var state = windowModel.Chest.State;

        def = windowModel.Chest;
        
        SetMessage(windowModel.Message);

        chestTypeLabel.Text = windowModel.ChestTypeText;
        
        btnOpenLabel.Text = windowModel.ButtonText;
        btnStartLabel.Text = windowModel.ButtonText;
        timerLengthLabel.Text = windowModel.TimerLength;
        
        btnOpen.SetActive(state == ChestState.InProgres);
        btnStart.SetActive(state == ChestState.Lock);
        
        timerLengthLabel.gameObject.SetActive(state == ChestState.Lock);
        timer.SetActive(state == ChestState.InProgres);

        chest.sprite = windowModel.ChestImage;
        
        IconBox.anchoredPosition = new Vector2(IconBox.anchoredPosition.x, state == ChestState.Lock ? 48 : 20);
        NameBox.anchoredPosition = new Vector2(NameBox.anchoredPosition.x, state == ChestState.Lock ? -95 : -123);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIChestWindowModel windowModel = Model as UIChestWindowModel;
        def = null;
    }
    
    private void Update()
    {
        if (def != null && def.State == ChestState.Open) Controller.CloseCurrentWindow();
        if (def == null || def.State != ChestState.InProgres) return;
        
        var windowModel = Model as UIChestWindowModel;
        var text = def.GetCurrentTime();
        
        timerLabel.Text = text;
        btnOpenLabel.Text = windowModel.ButtonText;
        
        if (def.State != ChestState.InProgres) Controller.CloseCurrentWindow();
    }
    
    public void OnClickStart()
    {
        var windowModel = Model as UIChestWindowModel;
        windowModel.Chest.StartTime = DateTime.Now;
        windowModel.Chest.State = ChestState.InProgres;
        
        (UIService.Get.GetShowedWindowByName(UIWindowType.MainWindow).CurrentView as UIMainWindowView).UpdateSlots();
        Controller.CloseCurrentWindow();
    }
    
    public void OnClickOpen()
    {
        var windowModel = Model as UIChestWindowModel;
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", Currency.Chest.Name), 
            ItemUid = Currency.Chest.Name, 
            Amount = 1,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = windowModel.Chest.Price.Currency, DefaultPriceAmount = windowModel.GetUnlockPrice()}
            }
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                var chestRewardmodel = UIService.Get.GetCachedModel<UIChestRewardWindowModel>(UIWindowType.ChestRewardWindow);

                def.State = ChestState.Open;
                chestRewardmodel.Chest = def;
                UIService.Get.ShowWindow(UIWindowType.ChestRewardWindow);
                GameDataService.Current.RemoveActiveChest(def);
        
                (UIService.Get.GetShowedWindowByName(UIWindowType.MainWindow).CurrentView as UIMainWindowView).UpdateSlots();
                Controller.CloseCurrentWindow();
            },
            item =>
            {
                // on purchase failed (not enough cash)
            }
        );
    }
}