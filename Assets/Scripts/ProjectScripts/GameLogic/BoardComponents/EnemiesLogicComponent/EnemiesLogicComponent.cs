﻿public class EnemiesLogicComponent : IECSComponent, IECSSystem
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid
	{
		get { return ComponentGuid; }
	}
	
	private Enemy enemy;
	
	public Enemy Enemy
	{
		get { return enemy; }
	}
	
	public void OnRegisterEntity(ECSEntity entity)
	{
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public void Execute()
	{
		enemy = GameDataService.Current.EnemiesManager.GetEnemy();
		
		if(enemy == null) return;
		
		EnemyView.Show(enemy);
	}

	public object GetDependency()
	{
		return null;
	}

	public bool IsExecuteable()
	{
		return GameDataService.Current.PiecesManager.CastlePosition.IsValid
			&& enemy != GameDataService.Current.EnemiesManager.CurrentEnemy
		       && GameDataService.Current.LevelsManager.Level >= GameDataService.Current.EnemiesManager.CurrentEnemy.Def.Level;
	}
}