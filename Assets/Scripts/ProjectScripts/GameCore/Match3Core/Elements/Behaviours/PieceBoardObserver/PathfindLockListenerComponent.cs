using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindLockListenerComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    protected Piece piece;
    protected BoardController board;

    protected bool cachedIsHasPath = true;

    public virtual bool IsHasPath()
    {
        return cachedIsHasPath;
    }
 
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        piece = entity as Piece;
        board = piece.Context;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public virtual void UpdatePathState(bool isHasPath)
    {
        cachedIsHasPath = isHasPath;

        if (isHasPath == false)
        {
            var views = piece?.ViewDefinition?.GetViews();
            
            if (views == null) return;

            for (int i = 0; i < views.Count; i++)
            {
                var view = views[i];
                view.UpdateVisibility(false);
            }
        }
    }
}
