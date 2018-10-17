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
		
		toggle.isOn = UIService.Get.GetCachedModel<UIOrdersWindowModel>(UIWindowType.OrdersWindow).Select == order;
		
		Init(order.Def.Uid, order.Reward.Replace(Order.Separator, "\n"));

		UpdateIndicator();
	}
	
	public void UpdateIndicator()
	{
		order.SetMark(indicator);
	}

	public override void Select(bool isSelect)
	{
		base.Select(isSelect);
		
		if(isSelect == false) return;
		
		selectItem.Init(order);
	}
}