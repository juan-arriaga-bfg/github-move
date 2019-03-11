public class MarketLogicComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public TimerComponent ResetMarketTimer { get; } = new TimerComponent();
	public TimerComponent ResetEnergyTimer { get; } = new TimerComponent();

	public override void OnRegisterEntity(ECSEntity entity)
	{
		RegisterComponent(ResetMarketTimer, true);
		RegisterComponent(ResetEnergyTimer, true);
		
		LocalNotificationsService.Current.RegisterNotifier(new Notifier(ResetMarketTimer, NotifyType.MarketRefresh));
		LocalNotificationsService.Current.RegisterNotifier(new Notifier(ResetEnergyTimer, NotifyType.FreeEnergyRefill));
		
		ResetMarketTimer.Delay = GameDataService.Current.ConstantsManager.MarketUpdateDelay;
		ResetEnergyTimer.Delay = GameDataService.Current.ConstantsManager.FreeEnergyDelay;
		
		ResetMarketTimer.OnComplete += () =>
		{
			GameDataService.Current.MarketManager.UpdateSlots(true);
			ResetMarketTimer.Start();
		};
		
		var save = ProfileService.Current.GetComponent<MarketSaveComponent>(MarketSaveComponent.ComponentGuid);

		if (save != null && string.IsNullOrEmpty(save.ResetMarketStartTime) == false) ResetMarketTimer.Start(long.Parse(save.ResetMarketStartTime));
		else ResetMarketTimer.Start();
		
		if (save != null && string.IsNullOrEmpty(save.ResetEnergyStartTime) == false) ResetEnergyTimer.Start(long.Parse(save.ResetEnergyStartTime));
	}

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        ResetMarketTimer.OnComplete = null;
        ResetEnergyTimer.OnComplete = null;
        
        base.OnUnRegisterEntity(entity);
    }
}