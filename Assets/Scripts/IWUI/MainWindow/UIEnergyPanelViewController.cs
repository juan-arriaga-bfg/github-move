using UnityEngine;
using UnityEngine.UI;

public class UIEnergyPanelViewController : UIGenericResourcePanelViewController
{
    [SerializeField] private Image progress;
    
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
    
    private void UpdateProgress(float value)
    {
        var manager = GameDataService.Current.LevelsManager;
        
        progress.fillAmount = Mathf.Clamp(value/manager.Price, 0f, 1f);
        
        if(CurrencyHellper.IsCanPurchase(itemUid, manager.Price) == false) return;

        CurrencyHellper.Purchase(Currency.Level.Name, 1, itemUid, manager.Price);
    }
}