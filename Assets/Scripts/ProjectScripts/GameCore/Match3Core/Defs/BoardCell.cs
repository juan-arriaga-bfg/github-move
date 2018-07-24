using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell
{
	private List<object> lockers = new List<object>();

	public virtual void Lock(object locker)
	{
		lockers.Add(locker);
	}

	public virtual void Unlock(object locker)
	{
		lockers.Remove(locker);
	}

	public virtual bool IsLocked
	{
		get
		{
			return lockers.Count > 0;
		}
	}

	public virtual List<object> Lockers
	{
		get { return lockers; }
	}

	public virtual T GetLockerBy<T>() where T : class
	{
		for (int i = 0; i < lockers.Count; i++)
		{
			if (lockers[i] is T)
			{
				return lockers[i] as T;
			}
		}

		return null;
	}

	public virtual List<object> GetLockers()
	{
		return lockers;
	}

	public virtual bool IsHasLocker<T>() where T : class
	{
		for (int i = 0; i < lockers.Count; i++)
		{
			if (lockers[i] is T)
			{
				return true;
			}
		}
		
		return false;
	}
}
