using System.Collections.Generic;
using UnityEngine;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private NSText settingsLabel;
    [SerializeField] private NSText fightLabel;
    [SerializeField] private List<UIChestSlot> slots;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;

        settingsLabel.Text = windowModel.SettingsText;
        fightLabel.Text = windowModel.FightText;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (i < slots.Count - 1) slot.Initialize(GameDataService.Current.Chests[i]);
            else slot.Initialize();
        }
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
        /*var model = UIService.Get.GetCachedModel<UICharacterWindowModel>(UIWindowType.CharacterWindow);

        model.HeroDamage = 5;
        model.TeamDamage = 12;
        model.CardTupe = CharacterWindowCardTupe.Rare;
        
        UIService.Get.ShowWindow(UIWindowType.CharacterWindow);*/

        GameDataService.Current.GetEnemy();
    }
}
