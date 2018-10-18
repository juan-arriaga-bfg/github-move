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
            
            if (amountLabel != null) amountLabel.Text = $"<size=80>{currentValueAnimated}</size>\n<size=30>Level</size>";
        }
    }

    public override void UpdateView()
    {
        if (storageItem == null) return;

        currentValue = storageItem.Amount;

        if (amountLabel != null) amountLabel.Text = $"<size=80>{storageItem.Amount}</size>\n<size=30>Level</size>";
    }
}