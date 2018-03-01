using System.Collections.Generic;
using UnityEngine;

public class UICharacterWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText levelLabel;
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private NSText progressLabel;
    [SerializeField] private NSText cardName;
    [SerializeField] private NSText cardLabel;
    [SerializeField] private NSText damageLabel;

    [SerializeField] private RectTransform progress;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UICharacterWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        buttonLabel.Text = windowModel.ButtonText;
        
        levelLabel.Text = windowModel.LevelText;
        
        progressLabel.Text = windowModel.ProgressText;
        damageLabel.Text = windowModel.DamageText;
        
        cardName.Text = windowModel.HeroName;
        cardLabel.Text = windowModel.CardTupeText;
        
        progress.sizeDelta = new Vector2(320 * windowModel.CurrentProgress/(float)windowModel.TotalProgress, progress.sizeDelta.y);
    }

    public void OnClick()
    {
        var windowModel = Model as UICharacterWindowModel;
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", Currency.Level.Name), 
            ItemUid = Currency.Level.Name, 
            Amount = 1,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = Currency.RobinCards.Name, DefaultPriceAmount = windowModel.TotalProgress}
            }
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
            },
            item =>
            {
                // on purchase failed (not enough cash)
            }
        );
        
        Controller.CloseCurrentWindow();
    }
}