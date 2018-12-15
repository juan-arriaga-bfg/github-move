using UnityEngine;
using UnityEngine.UI;

public class UIOrderElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding] private UIOrdersTabButtonViewController button;
	[IWUIBinding("#Mark")] private Image mark;
	
	public override void Init()
	{
		base.Init();
        
		var contentEntity = entity as UIOrderElementEntity;
		
		button
			.SetActiveScale(1.2f)
			.Init()
			.SetDragDirection(new Vector2(0f, 1f))
			.SetDragThreshold(100f)
			.ToState(GenericButtonState.UnActive).OnClick(Select);
		
		contentEntity.Data.SetMark(mark);
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