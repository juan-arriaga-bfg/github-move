using System.Collections.Generic;
using UnityEngine;

public class BoardStatesComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public virtual int Guid { get { return ComponentGuid; } }
    
    private Dictionary<int, IECSComponent> registeredStateComponents = new Dictionary<int, IECSComponent>();
    
    private List<int> activeStateGuids = new List<int>();
    

    private ECSEntity context;

    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
        this.context = null;
    }


    public virtual BoardStatesComponent RegisterState(IECSComponent state)
    {
        if (registeredStateComponents.ContainsKey(state.Guid)) return this;

        registeredStateComponents.Add(state.Guid, state);

        return this;
    }

    protected virtual IECSComponent GetStateInternal(int guid)
    {
        IECSComponent component;
        if (registeredStateComponents.TryGetValue(guid, out component))
        {
            return component;
        }
        
        return null;
    }

    public virtual bool AddState(int guid)
    {
        if (context == null) return false;
        
        var stateComponent = GetStateInternal(guid);

        if (stateComponent != null)
        {
            activeStateGuids.Add(guid);
            
            context.RegisterComponent(stateComponent);

            return true;
        }

		Debug.LogError("[BoardStatesComponent]: try to add unregistered state => " + guid);
        return false;
    }

    public virtual bool RemoveState(int guid)
    {
        if (context == null) return false;
        
        var stateComponent = GetStateInternal(guid);

        if (stateComponent != null)
        {

            activeStateGuids.Remove(guid);

            context.UnRegisterComponent(stateComponent);

            return true;

        }

        return false;
    }

    public virtual bool IsHasState(int guid)
    {
        if (context == null) return false;
        
        return context.IsHasComponent(guid);
    }

    public virtual IECSComponent GetState(int guid)
    {
        if (context == null) return null;
        
        return context.GetComponent(guid);
    }

    public virtual T GetState<T>(int guid) where T : class, IECSComponent
    {
        if (context == null) return null;
        
        return context.GetComponent(guid) as T;
    }

    public virtual void ClearStates()
    {
        for (var i = 0; i < activeStateGuids.Count; i++)
        {
            var guid = activeStateGuids[i];
            RemoveState(guid);
        }
    }

}
