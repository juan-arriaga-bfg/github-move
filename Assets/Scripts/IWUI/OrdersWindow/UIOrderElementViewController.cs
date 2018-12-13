using UnityEngine;
using UnityEngine.UI;

public class UIOrderElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding] private UIOrdersTabButtonViewController button;
	
	[SerializeField] private Image indicator;
	
	public override void Init()
	{
		base.Init();
        
		var contentEntity = entity as UIOrderElementEntity;
		
//		toggle.isOn = UIService.Get.GetCachedModel<UIOrdersWindowModel>(UIWindowType.OrdersWindow).Select == order;
		
		button
			.SetActiveScale(1.2f)
			.Init()
			.SetDragDirection(new Vector2(0f, 1f))
			.SetDragThreshold(100f)
			.ToState(GenericButtonState.UnActive).OnClick(Select);
		
		UpdateIndicator();
	}
	
	public void UpdateIndicator()
	{
//		order.SetMark(indicator);
	}
	
	public override void OnSelect()
	{
		base.OnSelect();
        
		button.ToState(GenericButtonState.Active);
	}
	
	public override void OnDeselect()
	{
		base.OnDeselect();
        
		button.ToState(GenericButtonState.UnActive);
	}
}