using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIExperiencePanelViewController : UIGenericResourcePanelViewController
{
    [SerializeField] private Image progress;

    private bool isLevelUp;
    public Action<int> OnLevelUp;

    private string kiloAbbreviation;
    
    public override int CurrentValueAnimated
    {
        get { return currentValueAnimated; }
        set
        {
            currentValueAnimated = value;
            UpdateProgress(currentValueAnimated);
        }
    }

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);
        kiloAbbreviation = LocalizationService.Get("common.abbreviation.kilo", "common.abbreviation.kilo");
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

        SetText((int) Mathf.Max(0, value), manager.Price);
        
        if(isLevelUp || CurrencyHellper.IsCanPurchase(itemUid, manager.Price) == false) return;

        isLevelUp = true;

        DefaultSafeQueueBuilder.BuildAndRun(() =>
        {
            UIService.Get.ShowWindow(UIWindowType.NextLevelWindow);
            UIService.Get.OnCloseWindowEvent += OnCloseNextLevelWindow;
        });
    }

    private void OnCloseNextLevelWindow(IWUIWindow window)
    {
        UIService.Get.OnCloseWindowEvent -= OnCloseNextLevelWindow;
        isLevelUp = false;
    }

    private void SetText(int from, int to)
    {
        const int MAX_LEN = 9;
        
        const string DELIMITER = "/";
        string fromStr = from.ToString();
        string toStr = to.ToString();

        if (fromStr.Length + DELIMITER.Length + toStr.Length > MAX_LEN)
        {
            if (to < 1000)
            {
                Debug.LogError($"[UIExperiencePanelViewController] => SetText: Text size exceeded maximum limit of {MAX_LEN} chars but can't be compressed using K");
            }
            else if (to >= 1000)
            {
                int rounded = Mathf.CeilToInt(to / 1000f);
                toStr = $"{rounded}{kiloAbbreviation}";
            }
        }
        
        amountLabel.Text = string.Concat(fromStr, DELIMITER, toStr);
    }
}