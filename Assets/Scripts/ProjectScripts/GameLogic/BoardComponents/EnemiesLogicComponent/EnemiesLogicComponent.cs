public class EnemiesLogicComponent : IECSComponent, IECSSystem
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid
	{
		get { return ComponentGuid; }
	}

	private int index;
	private Enemy enemy;

	
	public void OnRegisterEntity(ECSEntity entity)
	{
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public void Execute()
	{
		enemy = GameDataService.Current.EnemiesManager.GetEnemy(index);
		index++;

		if (index == GameDataService.Current.EnemiesManager.Enemies.Count)
		{
			index = 0;
		}
		
		EnemyView.Show(enemy);
	}

	public bool IsPersistence
	{
		get { return false; }
	}

	public bool IsExecuteable()
	{
		return GameDataService.Current.LevelsManager.Level >= GameDataService.Current.EnemiesManager.Enemies[index].Level && (enemy == null || enemy.IsComplete);
	}
}
