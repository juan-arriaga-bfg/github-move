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
            
            if (amountLabel != null) amountLabel.Text = string.Format("<size=80>{0}</size>\n<size=30>Level</size>", currentValueAnimated);
        }
    }

    public override void UpdateView()
    {
        if (storageItem == null) return;

        currentValue = storageItem.Amount;

        if (amountLabel != null) amountLabel.Text = string.Format("<size=80>{0}</size>\n<size=30>Level</size>", storageItem.Amount);
    }
}