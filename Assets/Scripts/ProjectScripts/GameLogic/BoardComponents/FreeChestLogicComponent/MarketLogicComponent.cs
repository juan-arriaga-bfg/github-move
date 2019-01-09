public class MarketLogicComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public TimerComponent Timer { get; } = new TimerComponent();

	public override void OnRegisterEntity(ECSEntity entity)
	{
		RegisterComponent(Timer);
		
		Timer.Delay = GameDataService.Current.ConstantsManager.MarketUpdateDelay;
		
		var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

		if (save != null && string.IsNullOrEmpty(save.ResetMarketStartTime) == false)
		{
			Timer.Start(long.Parse(save.ResetMarketStartTime));
			return;
		}
		
		Timer.Start();
	}
}