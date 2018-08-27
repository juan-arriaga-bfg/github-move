using System.Collections.Generic;

public class LockerComponent : IECSComponent 
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid => ComponentGuid;

	private readonly List<object> lockers = new List<object>();
	public virtual bool IsLocked => lockers.Count > 0;

	public virtual void Lock(object locker, bool multiple = true)
	{
		if (multiple == false && lockers.FindAll(item => item == locker).Count > 0) return;
		
		lockers.Add(locker);
	}

	public virtual void Unlock(object locker, bool all = false)
	{
		if (all)
		{
			var items = lockers.FindAll(item => item == locker);

			foreach (var item in items)
			{
				lockers.Remove(item);
			}
			
			return;
		}
		
		lockers.Remove(locker);
	}
	
	public virtual void OnRegisterEntity(ECSEntity entity)
	{
	}

	public virtual void OnUnRegisterEntity(ECSEntity entity)
	{
	}	
}