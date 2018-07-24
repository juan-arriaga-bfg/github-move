using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionBoardStateComponent : GenericBoardStateComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    public SessionBoardStateComponent(int startStateId)
    {
        stateId = startStateId;
    }

    private List<object> lockers = new List<object>();

    public List<object> Lockers { get { return lockers; } }

    public virtual bool IsProcessing { get { return StateId > 0 && lockers.Count <= 0; } }

    public virtual void Pause(bool state, object affector)
    {
        if (state)
        {
            lockers.Add(affector);
        }
        else
        {
            lockers.Remove(affector);
        }
    }

}
