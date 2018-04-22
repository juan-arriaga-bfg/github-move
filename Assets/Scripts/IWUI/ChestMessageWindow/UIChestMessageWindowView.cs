using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChestMessageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText btnFastLabel;
    [SerializeField] private NSText btnSlowLabel;
    [SerializeField] private NSText chanceLabel;
    
    [SerializeField] private GameObject btnSlow;
    [SerializeField] private Image item;

    private List<Image> icons = new List<Image>();

    private bool isFast;
    private bool isSlow;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIChestMessageWindowModel;

        isFast = isSlow = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        btnFastLabel.Text = windowModel.FastButtonText;
        btnSlowLabel.Text = windowModel.SlowButtonText;
        chanceLabel.Text = windowModel.ChanceText;
        
        btnSlow.SetActive(windowModel.IsShowSlowButton);

        var sprites = windowModel.Icons();
        
        item.sprite = IconService.Current.GetSpriteById(sprites[0]);
        
        for (var i = 1; i < sprites.Count; i++)
        {
            CreateIcon(sprites[i]);
        }
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIChestMessageWindowModel;
        windowModel.Chest = null;
        
        if (isFast && windowModel.OnBoost != null)
        {
            windowModel.OnBoost();
            return;
        }
        
        if (isSlow && windowModel.OnStart != null) windowModel.OnStart();
    }

    public override void OnViewCloseCompleted()
    {
        foreach (var image in icons)
        {
            Destroy(image.gameObject);
        }
        
        icons = new List<Image>();
    }

    public void FastClick()
    {
        var windowModel = Model as UIChestMessageWindowModel;

        CurrencyHellper.Purchase(windowModel.Chest.Currency, 1, windowModel.Chest.Def.Price, success =>
        {
            if (!success) return;
            isFast = true;
            Controller.CloseCurrentWindow();
        });
    }
    
    public void SlowClick()
    {
        isSlow = true;
        Controller.CloseCurrentWindow();
    }

    private void CreateIcon(string icon)
    {
        var image = Instantiate(item, item.transform.parent).GetComponent<Image>();

        image.sprite = IconService.Current.GetSpriteById(icon);
        icons.Add(image);
    }
}
