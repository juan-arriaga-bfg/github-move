/// <summary>
/// Highlight first existing mine from mines chain 
/// </summary>
public class HighlightTaskFirstMineOfAnyType : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Mine, PieceTypeFilter.Fake);
        foreach (var id in ids)
        {
            if (HighlightTaskMineHelper.Highlight(id))
            {
                return true;
            }
        }

        return false;
    }
}