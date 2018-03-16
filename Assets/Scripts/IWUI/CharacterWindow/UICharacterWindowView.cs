using UnityEngine;
using UnityEngine.UI;

public class UICharacterWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText levelLabel;
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private NSText progressLabel;
    [SerializeField] private NSText cardName;
    [SerializeField] private NSText cardLabel;
    
    [SerializeField] private Image bigIcon;
    [SerializeField] private Image smallIcon;

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
        
        cardName.Text = windowModel.HeroName;
        cardLabel.Text = windowModel.CardTupeText;
        
        progress.sizeDelta = new Vector2(windowModel.ProgressLenght, progress.sizeDelta.y);
        
        bigIcon.sprite = windowModel.IconSprite;
        smallIcon.sprite = windowModel.IconSprite;
    }

    public void OnClick()
    {
        var windowModel = Model as UICharacterWindowModel;
        
        windowModel.Hero.LevelUp();
        
        Controller.CloseCurrentWindow();
    }
}