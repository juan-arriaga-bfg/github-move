using UnityEngine;

public class MarketButton : UIGenericResourcePanelViewController
{
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private NSText label;
    
    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);
        
        label.Text = LocalizationService.Get("window.main.market", "window.main.market");

        GameDataService.Current.MarketManager.UpdateState += UpdateButtonState;
        UpdateButtonState();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GameDataService.Current.MarketManager.UpdateSlots();
        UpdateButtonState();
    }

    private void OnDestroy()
    {
        GameDataService.Current.MarketManager.UpdateState -= UpdateButtonState;
    }

    public override void UpdateResource(int offset)
    {
        base.UpdateResource(offset);
        UpdateButtonState();
    }
    
    private void UpdateButtonState()
    {
        var isActive = GameDataService.Current.MarketManager.Defs.Find(item => item.State == MarketItemState.Saved) != null;
        
        shine.SetActive(isActive);
        exclamationMark.SetActive(isActive);
    }
}