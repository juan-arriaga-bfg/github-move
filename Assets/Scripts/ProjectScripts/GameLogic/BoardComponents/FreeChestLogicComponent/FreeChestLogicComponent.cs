public class FreeChestLogicComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	public TimerComponent Timer { get; } = new TimerComponent();

	public override void OnRegisterEntity(ECSEntity entity)
	{
		RegisterComponent(Timer);
		
		Timer.Delay = GameDataService.Current.ConstantsManager.FreeChestDelay;
		Timer.Start();
	}
}