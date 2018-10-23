using System.Collections.Generic;
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
}
