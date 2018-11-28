public class UILevelPanelViewController : UIGenericResourcePanelViewController
{
    public override int CurrentValueAnimated
    {
        get
        {
            return currentValueAnimated;
        }
        set
        {
            currentValueAnimated = value;

            SetAmountLabelText(currentValueAnimated);
        }
    }

    public override void UpdateView()
    {
        if (storageItem == null) return;

        currentValue = storageItem.Amount;

        SetAmountLabelText(storageItem.Amount);
    }

    private void SetAmountLabelText(int amount)
    {
        if (amountLabel != null)
        {
            // amountLabel.Text = $"<size=80>{storageItem.Amount}</size>\n<size=30>Level</size>";
            amountLabel.Text = $"<size=75>{storageItem.Amount}</size>";
        }
    }
}