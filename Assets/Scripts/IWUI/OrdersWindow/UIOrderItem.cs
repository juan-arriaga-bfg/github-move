using UnityEngine;
using UnityEngine.UI;

public class UIOrderItem : UISimpleScrollItem
{
	[SerializeField] private UIOrdersSelectItem selectItem;
	[SerializeField] private Image indicator;
	
	private Order order;
	private Toggle toggle;
	
	public void Init(Order item)
	{
		order = item;
		
		if (toggle == null) toggle = gameObject.GetComponent<Toggle>();
		
		toggle.isOn = false;
		
		Init(order.Def.Uid, order.Reward.Replace(Order.Separator, "\n"));

		UpdateIndicator();
	}

	public void UpdateIndicator()
	{
		var isShowIndicator = order.State == OrderState.Complete || order.State == OrderState.Enough;
		
		indicator.gameObject.SetActive(isShowIndicator);

		if (isShowIndicator) indicator.sprite = IconService.Current.GetSpriteById($"icon_{(order.State == OrderState.Enough ? "Warning" : "Complete")}");
	}

	public void Select(bool isSelect)
	{
		if(isSelect == false) return;
		
		selectItem.Init(order);
	}
}