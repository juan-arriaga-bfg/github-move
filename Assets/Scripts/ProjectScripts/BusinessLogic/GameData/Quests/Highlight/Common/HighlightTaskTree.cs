using UnityEngine;

public class HighlightTaskTree : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        if (task is TaskKillTreeEntity || task is TaskHitTreeEntity)
        {
            includeFilter = PieceTypeFilter.Tree;
        }
        else
        {
            Debug.LogError($"[HighlightTaskTree] => Unsupported task type: {task.GetType()}");
            return false;
        }
        
        return base.ShowArrow(task, delay);
    }
}