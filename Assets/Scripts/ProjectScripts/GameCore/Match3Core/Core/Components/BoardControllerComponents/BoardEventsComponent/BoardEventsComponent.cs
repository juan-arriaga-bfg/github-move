using System.Collections.Generic;

public class BoardEventsComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid
    {
        get { return ComponentGuid; }
    }
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    private Dictionary<int, List<IBoardEventListener>> registeredListeners = new Dictionary<int, List<IBoardEventListener>>();

    public Dictionary<int, List<IBoardEventListener>> Listeners
    {
        get { return registeredListeners; }
    }

    public void AddListener(IBoardEventListener listener, int eventCode)
    {
        if (registeredListeners.ContainsKey(eventCode) == false)
        {
            registeredListeners.Add(eventCode, new List<IBoardEventListener>());
        }
        
        registeredListeners[eventCode].Add(listener);
    }

    public void RemoveListener(IBoardEventListener listener, int eventCode)
    {
        List<IBoardEventListener> listeners;

        if (registeredListeners.TryGetValue(eventCode, out listeners))
        {
            listeners.Remove(listener);
        }
    }

    public virtual void RaiseEvent(int eventCode, object context)
    {
        List<IBoardEventListener> listeners;

        if (registeredListeners.TryGetValue(eventCode, out listeners))
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnBoardEvent(eventCode, context);
            }
        }
    }
}