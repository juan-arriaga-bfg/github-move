using UnityEngine;
using UnityEngine.UI;

public class UiCollectionItem : UIGenericResourcePanelViewController
{
    [SerializeField] private Image back;
    
    public void Decoration(string currency)
    {
        itemUid = currency;
        icon.sprite = IconService.Current.GetSpriteById(currency);
        ResourcesViewManager.Instance.RegisterView(this);
    }

    public override void UpdateView()
    {
        var isActive = storageItem.Amount > 0;
        
        icon.color = new Color(1, 1, 1, isActive ? 1 : 0.5f);
        back.sprite = IconService.Current.GetSpriteById(string.Format("ramka_item_{0}active", isActive ? "" : "not_"));
    }

    public void AllActive()
    {
        back.sprite = IconService.Current.GetSpriteById("ramka_item");
    }

    protected override void OnEnable()
    {
    }
    
    public override void UpdateResource(int offset)
    {
        UpdateView();
    }
}