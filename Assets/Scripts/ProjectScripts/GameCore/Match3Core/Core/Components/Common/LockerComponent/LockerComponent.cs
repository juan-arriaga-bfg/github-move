using System.Collections.Generic;

public class LockerComponent : IECSComponent 
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid { get { return ComponentGuid; } }
	
	private List<object> lockers = new List<object>();

	public virtual bool IsLocked
	{
		get { return lockers.Count > 0; }
	}

	public virtual void Lock(object locker)
	{
		lockers.Add(locker);
	}

	public virtual void Unlock(object locker)
	{
		lockers.Remove(locker);
	}
	
	public virtual void OnRegisterEntity(ECSEntity entity)
	{
		
	}

	public virtual void OnUnRegisterEntity(ECSEntity entity)
	{
		
	}	
}
