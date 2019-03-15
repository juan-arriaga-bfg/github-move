using Debug = IW.Logger;
using UnityEngine;

public class HighlightTaskClearFog : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        TaskClearFogEntity fogTask = task as TaskClearFogEntity;
        if (fogTask == null)
        {
            Debug.LogError("[HighlightTaskClearFog] => task is not TaskClearFogEntity");
            return false;
        }
        
        string fogUid = fogTask.FogId;

        if (string.IsNullOrEmpty(fogUid))
        {
            return HighlightFogHelper.HighlightNextFog(delay);
        }
        
        return HighlightFogHelper.HighlightByUid(fogUid, delay);
    }
}