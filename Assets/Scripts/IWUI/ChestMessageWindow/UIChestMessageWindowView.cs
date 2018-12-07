using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChestMessageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText btnOpenLabel;
    [SerializeField] private NSText chanceLabel;
    
    [SerializeField] private Image chest;
    [SerializeField] private Image item;
    
    [SerializeField] private ScrollRect scroll;

    private List<Image> icons = new List<Image>();

    private bool isOpen;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIChestMessageWindowModel;

        isOpen = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        btnOpenLabel.Text = windowModel.ButtonText;
        chanceLabel.Text = windowModel.ChanceText;
        
        chest.sprite = IconService.Current.GetSpriteById(windowModel.ChestComponent.Def.Uid);
        chest.SetNativeSize();
        
        var sprites = windowModel.Icons();
        item.sprite = IconService.Current.GetSpriteById(sprites[0]);
        
        for (var i = 1; i < sprites.Count; i++)
        {
            CreateIcon(sprites[i]);
        }

        scroll.horizontalNormalizedPosition = 0;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIChestMessageWindowModel;

        if (isOpen) windowModel.ChestComponent.Rewards.GetInWindow();
        
        windowModel.ChestComponent = null;
    }
    
    public override void OnViewCloseCompleted()
    {
        foreach (var image in icons)
        {
            Destroy(image.gameObject);
        }
        
        icons = new List<Image>();
    }
    
    public void OnOpenClick()
    {
        isOpen = true;
        Controller.CloseCurrentWindow();
    }
    
    private void CreateIcon(string icon)
    {
        var image = Instantiate(item, item.transform.parent).GetComponent<Image>();

        image.sprite = IconService.Current.GetSpriteById(icon);
        icons.Add(image);
    }
}
