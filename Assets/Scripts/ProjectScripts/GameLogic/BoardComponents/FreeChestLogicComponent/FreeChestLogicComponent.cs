public class FreeChestLogicComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public TimerComponent Timer { get; } = new TimerComponent();

	public override void OnRegisterEntity(ECSEntity entity)
	{
		RegisterComponent(Timer);
		
		Timer.Delay = GameDataService.Current.ConstantsManager.FreeChestDelay;
		
		var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

		if (save != null && string.IsNullOrEmpty(save.FreeChestStartTime) == false)
		{
			Timer.Start(long.Parse(save.FreeChestStartTime));
			return;
		}
		
		Timer.Start();
	}
}