using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RecursiveLock
{
    private readonly List<string> lockers = new List<string>();

    public string Name;

    public RecursiveLock(string name)
    {
        Name = name;
    }

    public bool Locked => lockers.Count != 0;
    
    public void Lock(string key)
    {
        lockers.Add(key);
        
        Debug.Log($"[RecursiveLock] => {Name} LOCK {key}. State: LOCKED ({lockers.Count})");
    }

    public void Unlock(string key)
    {
        lockers.Remove(key);

        if (Locked)
        {
            Debug.Log($"[RecursiveLock] => {Name} UNLOCK {key}. State: LOCKED ({lockers.Count})");
        }
        else
        {
            Debug.Log($"[RecursiveLock] => {Name} UNLOCK {key}. State: UNLOCKED");
        }
    }
    
    public bool IsLockedBy(string key)
    {
        return lockers.Contains(key);
    }

    public override string ToString()
    {
        if (!Locked)
        {
            return "[RecursiveLock] => Unlocked";
        }
        
        var sb = new StringBuilder("[RecursiveLock] => Locked by: ");
        foreach (var locker in lockers)
        {
            sb.Append("   ");
            sb.AppendLine(locker);
        }

        return sb.ToString();
    }
}