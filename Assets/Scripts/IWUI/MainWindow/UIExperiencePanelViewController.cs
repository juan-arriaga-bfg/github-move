﻿using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIExperiencePanelViewController : UIGenericResourcePanelViewController
{
    [SerializeField] private Image progress;

    private bool isLevelUp;
    
    public override int CurrentValueAnimated
    {
        get { return currentValueAnimated; }
        set
        {
            currentValueAnimated = value;
            UpdateProgress(currentValueAnimated);
        }
    }
    
    public override void UpdateView()
    {
        if (storageItem == null) return;
        
        currentValue = storageItem.Amount;
        
        UpdateProgress(currentValue);
    }

    public override void UpdateResource(int offset)
    {
        if (offset == 0) return;
        
        currentValueAnimated = offset < 0 ? 0 : currentValue;

        currentValue += offset;

        UpdateLabel(currentValue);
    }
    
    private void UpdateProgress(float value)
    {
        var manager = GameDataService.Current.LevelsManager;
        
        progress.fillAmount = Mathf.Clamp(value/manager.Price, 0f, 1f);
        
#if UNITY_EDITOR
        amountLabel.Text = $"{Mathf.Max(0, value)}/{manager.Price}";
#endif
        
        if(isLevelUp || CurrencyHellper.IsCanPurchase(itemUid, manager.Price) == false) return;

        isLevelUp = true;
        
        var rewards = new StringBuilder("Rewards:");
        var data = GameDataService.Current.LevelsManager.Rewards;

        foreach (var pair in data)
        {
            rewards.Append(" ");
            rewards.Append(pair.ToStringIcon());
        }

        UIMessageWindowController.CreateMessage("New level", rewards.ToString(), () =>
        {
            isLevelUp = false;
            CurrencyHellper.Purchase(Currency.Level.Name, 1, itemUid, manager.Price);
            CurrencyHellper.Purchase(Currency.EnergyLimit.Name, 1);

            GameDataService.Current.QuestsManager.UpdateActiveQuest();
            GameDataService.Current.TasksManager.NextLevel();
        }, null, true);
    }
}