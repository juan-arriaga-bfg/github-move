using UnityEngine;

public class UIResourcePanelViewController : UIGenericResourcePanelViewController 
{
    public override int CurrentValueAnimated
    {
        set
        {
            currentValueAnimated = value;
            SetLabelText(currentValueAnimated);
        }
    }

    public override void UpdateView()
    {
        if (storageItem == null) return;

        currentValue = storageItem.Amount;

        SetLabelText(storageItem.Amount);
        if (icon != null) icon.sprite = IconService.Instance.Manager.GetSpriteById(string.Format(IconPattern, storageItem.Currency));
    }

    public void DebugCurrentResources()
    {
        BoardService.Current.GetBoardById(0)?.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        
        var product = new CurrencyPair{Currency = itemUid, Amount = itemUid == Currency.Crystals.Name ? 5 : 100};
        
        UIMessageWindowController.CreateDefaultMessage($"Do you want to cheat and get {product.ToStringIcon(false)} for free?", () =>
        {
            CurrencyHellper.Purchase(product, null, new Vector2(Screen.width/2, Screen.height/2));
        });
    }

    private void SetLabelText(int value)
    {
        if (amountLabel == null) return;
        amountLabel.Text = $"<mspace=2.7em>{value}</mspace>";
    }
}