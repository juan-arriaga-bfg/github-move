using UnityEngine;

public class HighlightTaskUsingObstaclePieceFilter : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        if (task is TaskKillTreeEntity || task is TaskHitTreeEntity)
        {
            filter = PieceTypeFilter.Tree;
        }
        else if (task is TaskKillFieldEntity)
        {
            filter = PieceTypeFilter.ProductionField;
        }
        else
        {
            Debug.LogError($"[HighlightTaskUsingPieceFilter] => Unsupported task type: {task.GetType()}");
            return false;
        }
        
        return base.ShowArrow(task, delay);
    }
}