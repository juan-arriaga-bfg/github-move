using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }
    
    private Dictionary<int, BoardController> cachedBoardControllers = new Dictionary<int, BoardController>();
    
    public virtual void RegisterBoard(BoardController boardController, int id)
    {
        if (cachedBoardControllers.ContainsKey(id))
        {
            Debug.LogWarning(string.Format("[BoardManager]: board with tag:{0} already registered", id));
            return;
        }
        
        cachedBoardControllers.Add(id, boardController);
    }
    
    public virtual void UnRegisterBoard(BoardController boardController)
    {
        List<int> targetTags = new List<int>();
        foreach (var cachedBoardController in cachedBoardControllers)
        {
            if (cachedBoardController.Value == boardController)
            {
                targetTags.Add(cachedBoardController.Key);
            }
        }

        for (int i = 0; i < targetTags.Count; i++)
        {
            cachedBoardControllers.Remove(targetTags[i]);
        }
        
        CleanupRecursive(boardController, boardController);
    }

    public virtual BoardController GetBoardById(int id)
    {
        BoardController boardController;
        if (cachedBoardControllers.TryGetValue(id, out boardController))
        {
            return boardController;
        }

        return null;
    }
    
    public virtual BoardController FirstBoard => GetBoardById(0);

    public static void CleanupRecursive(IECSComponent itemToCleanup, ECSEntity context = null)
    {
        Debug.Log("======== CleanupRecursive: " + itemToCleanup.GetType());
        
        switch (itemToCleanup)
        {
            case ECSEntity entity:
                var ecsComponents = entity.ComponentsCache.Values.ToList();
                for (var i = ecsComponents.Count - 1; i >= 0; i--)
                {
                    var item = ecsComponents.ToList()[i];
                    CleanupRecursive(item, entity);
                }

                break;
            
            case ECSComponentCollection collection:
                foreach (var item in collection.Components)
                {
                    CleanupRecursive(item);
                }
                break;
            
            case IECSSystem system:
                ECSService.Current.SystemProcessor.UnRegisterSystem(system);
                break;
        }
        
        // Debug.Log($"== Unregister: {itemToCleanup.GetType()} with context: {context?.GetType()}");
        context?.UnRegisterComponent(itemToCleanup);
    }
    
    public void Cleanup()
    {
        var boardControllers = cachedBoardControllers.Values.ToList();

        for (var i = boardControllers.Count - 1; i >= 0; i--)
        {
            var boardController = boardControllers[i];
            UnRegisterBoard(boardController);
        }
    }
}
