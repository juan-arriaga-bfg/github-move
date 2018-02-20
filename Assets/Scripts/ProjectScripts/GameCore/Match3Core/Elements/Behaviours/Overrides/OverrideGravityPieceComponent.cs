﻿using System.Collections.Generic;

public class OverrideGravityPieceComponent : IECSComponent, IOverrideGravity
{
	private Dictionary<int, bool> filter = null;

	private bool defaultState = false;
	
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

	public virtual bool IsGravitableAtLayer(int layer)
	{
		if (filter == null) return defaultState;

		bool state = defaultState;
		
		return filter.TryGetValue(layer, out state) ? state : defaultState;
	}
}