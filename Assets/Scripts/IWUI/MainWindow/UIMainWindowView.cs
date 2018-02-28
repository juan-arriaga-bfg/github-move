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
        var model = UIService.Get.GetCachedModel<UIChestWindowModel>(UIWindowType.ChestWindow);

        model.Chest = GameDataService.Current.Chests[0];
        model.CurrentChestState = ChestState.Lock;
        
        UIService.Get.ShowWindow(UIWindowType.ChestWindow);
//        UIMessageWindowController.CreateNotImplementedMessage();
    }
    
    public void StartFight()
    {
        var model = UIService.Get.GetCachedModel<UICharacterWindowModel>(UIWindowType.CharacterWindow);

        model.HeroDamage = 5;
        model.TeamDamage = 12;
        model.CardTupe = CharacterWindowCardTupe.Rare;
        
        UIService.Get.ShowWindow(UIWindowType.CharacterWindow);
//        UIMessageWindowController.CreateNotImplementedMessage();
    }
}
