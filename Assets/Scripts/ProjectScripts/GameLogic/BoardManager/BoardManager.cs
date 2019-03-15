using Debug = IW.Logger;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }
    
    private readonly Dictionary<int, BoardController> cachedBoardControllers = new Dictionary<int, BoardController>();
    
    public virtual void RegisterBoard(BoardController boardController, int id)
    {
        if (cachedBoardControllers.ContainsKey(id))
        {
            Debug.LogWarning($"[BoardManager]: board with tag:{id} already registered");
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
        
        boardController.UnregisterRecursive();
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
