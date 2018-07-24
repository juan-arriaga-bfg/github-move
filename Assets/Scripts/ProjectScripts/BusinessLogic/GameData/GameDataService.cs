public class GameDataService : IWService<GameDataService, GameDataManager>
{
	public static GameDataManager Current
	{
		get { return Instance.Manager; }
	}
}