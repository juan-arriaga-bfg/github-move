using System.Collections.Generic;

public class OverrideMatchPieceComponent : IECSComponent, IOverrideMatchCopy
{
	private Dictionary<int, bool> filter;

	private bool defaultState;
	
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid
	{
		get { return ComponentGuid; }
	}

	public Dictionary<int, bool> Filter
	{
		get { return filter; }
		set { filter = value; }
	}

	public bool DefaultState
	{
		get { return defaultState; }
		set { defaultState = value; }
	}

	protected Piece context;

	public virtual void OnRegisterEntity(ECSEntity entity)
	{
		this.context = entity as Piece;
	}

	public virtual void OnUnRegisterEntity(ECSEntity entity)
	{
	}
	
	public bool IsMatchableAtLayer(int layer)
	{
		if (filter == null) return defaultState;

		bool state = defaultState;
		
		return filter.TryGetValue(layer, out state) ? state : defaultState;
	}
}
