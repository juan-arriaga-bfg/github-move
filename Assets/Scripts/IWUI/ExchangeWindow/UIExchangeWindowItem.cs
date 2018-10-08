using UnityEngine;
using UnityEngine.UI;

public class UIExchangeWindowItem : IWUIWindowViewController
{
	[SerializeField] private Image icon;
	[SerializeField] private NSText label;

	public void Init(CurrencyPair value)
	{
		icon.sprite = value.GetIcon();
		label.Text = value.Amount.ToString();
	}
}