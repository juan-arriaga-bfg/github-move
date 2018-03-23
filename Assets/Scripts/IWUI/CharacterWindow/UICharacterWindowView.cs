using UnityEngine;
using UnityEngine.UI;

public class UICharacterWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText abilityLabel;
    [SerializeField] private NSText levelLabel;
    [SerializeField] private NSText cardLabel;
    [SerializeField] private NSText progressLabel;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private Image bigIcon;
    [SerializeField] private Image smallIcon;
    [SerializeField] private Image skillIcon;

    [SerializeField] private RectTransform progress;
    [SerializeField] private Button button;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UICharacterWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        abilityLabel.Text = windowModel.AbilityValue;
        levelLabel.Text = windowModel.LevelText;
        cardLabel.Text = windowModel.CardTupeText;
        progressLabel.Text = windowModel.ProgressText;
        buttonLabel.Text = windowModel.ButtonText;
        
        bigIcon.sprite = windowModel.IconSprite;
        smallIcon.sprite = windowModel.IconSprite;
        skillIcon.sprite = windowModel.SkillSprite;
        
        progress.sizeDelta = new Vector2(windowModel.ProgressLenght, progress.sizeDelta.y);
        
        button.interactable = windowModel.IsDone;
    }

    public void OnClick()
    {
        var windowModel = Model as UICharacterWindowModel;
        
        Controller.CloseCurrentWindow();
        
        if (windowModel.Hero.IsLevelMax())
        {
            var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
            model.Title = "Message";
            model.Message = "Tavern level is not high enough!";
            model.AcceptLabel = "Go to Tavern";
        
            model.OnAccept = () => { HintArrowView.AddHint(GameDataService.Current.PiecesManager.TavernPosition); };
            model.OnCancel = null;
        
            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            IWUIManager.Instance.CloseWindow(UIWindowType.TavernWindow);
            return;
        }
        
        windowModel.Hero.LevelUp();
    }
}