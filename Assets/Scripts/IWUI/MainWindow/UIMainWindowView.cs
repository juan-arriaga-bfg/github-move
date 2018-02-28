using UnityEngine;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private NSText settingsLabel;
    [SerializeField] private NSText fightLabel;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;

        settingsLabel.Text = windowModel.SettingsText;
        fightLabel.Text = windowModel.FightText;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIMainWindowModel windowModel = Model as UIMainWindowModel;
    }

    public void ShowSettings()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
    }
    
    public void StartFight()
    {
        UIMessageWindowController.CreateNotImplementedMessage();
    }
    
    public void ShowSample()
    {
//        // get model for window
//        var model = UIService.Get.GetCachedModel<UISampleWindowModel>(UIWindowType.SampleWindow);
//        // modify model properties
//        model.RandomNumber = UnityEngine.Random.Range(0, 100);
//        // show window
//        UIService.Get.ShowWindow(UIWindowType.SampleWindow);
        
        UIService.Get.ShowWindow(UIWindowType.CharacterWindow);
    }
}
