using IW;

public class HighlightTaskAnyTree : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var pieceId = ((IHavePieceId) task).PieceId;
        if (pieceId == PieceType.None.Id || pieceId == PieceType.Empty.Id)
        {
            if (task is TaskKillTreeEntity || task is TaskHitTreeEntity)
            {
                includeFilter = PieceTypeFilter.Tree;
            }
            else
            {
                Logger.LogError($"[HighlightTaskTree] => Unsupported task type: {task.GetType()}");
                return false;
            }

            return base.ShowArrow(task, delay);
        }

        return false;
    }
}