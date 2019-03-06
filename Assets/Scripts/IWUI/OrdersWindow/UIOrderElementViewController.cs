using UnityEngine;
using UnityEngine.UI;

public class UIOrderElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding] private UIOrdersTabButtonViewController button;
	[IWUIBinding("#Mark")] private Image mark;
	[IWUIBinding("#Clock")] private GameObject clock;
	
	public override void Init()
	{
		base.Init();
        
		button
			.SetActiveScale(1.2f)
			.ToState(GenericButtonState.UnActive);
		
		var contentEntity = entity as UIOrderElementEntity;
		
		contentEntity.Data.OnStateChange += UpdateMark;

		UpdateMark();
	}

	private void OnDisable()
	{
		if(!(entity is UIOrderElementEntity contentEntity)) return;
		
		contentEntity.Data.OnStateChange -= UpdateMark;
	}

	private void UpdateMark()
	{
		var contentEntity = entity as UIOrderElementEntity;
		
		contentEntity.Data.SetMark(mark, clock);
	}

	public override void OnViewShowCompleted()
	{
		base.OnViewShowCompleted();
		
		button.OnClick(Select);
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