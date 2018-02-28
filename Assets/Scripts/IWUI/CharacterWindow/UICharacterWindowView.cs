using UnityEngine;
using DG.Tweening;

public class UICharacterWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText levelLabel;
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private NSText progressLabel;
    [SerializeField] private NSText cardName;
    [SerializeField] private NSText cardLabel;
    [SerializeField] private NSText damageLabel;
    
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
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICharacterWindowModel windowModel = Model as UICharacterWindowModel;
        
    }
}