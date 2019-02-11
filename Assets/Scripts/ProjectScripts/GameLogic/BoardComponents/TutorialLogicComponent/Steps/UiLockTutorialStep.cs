using System.Collections.Generic;

public enum UiLockTutorialItem
{
	Worker,
	Energy,
	Codex,
	Shop,
	Orders,
	Remove,
	Daily,
}

public class UiLockTutorialStep : BaseTutorialStep
{
	public List<UiLockTutorialItem> Targets;

	private UIMainWindowView window;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		base.OnRegisterEntity(entity);

		window = UIService.Get.GetShowedWindowByName(UIWindowType.MainWindow)?.CurrentView as UIMainWindowView;
		
		if(window == null) return;

		foreach (var target in Targets)
		{
			window.ChangeVisibility(target, true, false);
		}
	}
	
	protected override void Complete()
	{
		base.Complete();
		
		if(window == null) return;

		foreach (var target in Targets)
		{
			window.ChangeVisibility(target, false, true);
		}
	}
}