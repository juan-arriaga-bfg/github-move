using UnityEngine;
using UnityEngine.UI;

public class OrdersButton : UIFakePanelViewController
{
	[SerializeField] private Image markIcon;
	[SerializeField] private GameObject shine;
	[SerializeField] private GameObject exclamationMark;
	[SerializeField] private NSText label;

	public override void OnViewShow(IWUIWindowView context)
	{
		base.OnViewShow(context);
		
		label.Text = LocalizationService.Get("window.main.orders", "window.main.orders");
		UpdateState();
	}

	public override void UpdateResource(int offset)
	{
		base.UpdateResource(offset);
		UpdateState();
	}

	private void UpdateState()
	{
		var isWarning = false;
		var isComplete = false;

		var orders = GameDataService.Current.OrdersManager.Orders;

		foreach (var order in orders)
		{
			if (order.State == OrderState.Complete)
			{
				isComplete = true;
				break;
			}
			
			if (order.State == OrderState.Enough) isWarning = true;
		}

		var isShowIndicator = isWarning || isComplete;
		
		shine.SetActive(isShowIndicator);
		exclamationMark.SetActive(isShowIndicator);
		
		if(isShowIndicator) markIcon.sprite = IconService.Current.GetSpriteById($"icon_{(isComplete ? "Complete" : "Warning")}");
	}
}