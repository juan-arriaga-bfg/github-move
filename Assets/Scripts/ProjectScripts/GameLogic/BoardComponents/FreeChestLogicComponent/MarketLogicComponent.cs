public class MarketLogicComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public TimerComponent Timer { get; } = new TimerComponent();

	public override void OnRegisterEntity(ECSEntity entity)
	{
		RegisterComponent(Timer);
		
		LocalNotificationsService.Current.RegisterNotifier(new Notifier(Timer, NotifyType.MarketRefresh));
		
		Timer.Delay = GameDataService.Current.ConstantsManager.MarketUpdateDelay;
		
		Timer.OnComplete += () =>
		{
			GameDataService.Current.MarketManager.UpdateSlots(true);
			Timer.Start();
		};
		
		var save = ProfileService.Current.GetComponent<MarketSaveComponent>(MarketSaveComponent.ComponentGuid);

		if (save != null && string.IsNullOrEmpty(save.ResetMarketStartTime) == false)
		{
			Timer.Start(long.Parse(save.ResetMarketStartTime));
			return;
		}
		
		Timer.Start();
	}
}