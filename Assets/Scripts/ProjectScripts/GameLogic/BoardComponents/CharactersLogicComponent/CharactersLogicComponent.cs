using System.Collections.Generic;

public class CharactersLogicComponent : IECSComponent
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	private BoardLogicComponent context;

	private List<int> ids;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
		ids = PieceType.GetIdsByFilter(PieceTypeFilter.Character);
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}

	public void Step()
	{
		foreach (var id in ids)
		{
			var positions = context.PositionsCache.GetPiecePositionsByType(id);

			foreach (var position in positions)
			{
				context.Context.ActionExecutor.AddAction(new MoveCharacterAction{From = position});
			}
		}
	}
}