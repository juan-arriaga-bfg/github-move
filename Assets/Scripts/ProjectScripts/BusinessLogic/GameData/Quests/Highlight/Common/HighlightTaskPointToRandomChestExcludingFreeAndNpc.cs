/// <summary>
/// Highlight first existing chest
/// </summary>
public class HighlightTaskPointToRandomChestExcludingFreeAndNpc : HighlightTaskUsingPieceFilter
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        includeFilter = PieceTypeFilter.Chest;
        excludeFilter = PieceTypeFilter.Bag;
        
        return base.ShowArrow(task, delay);
    }
    
    protected override bool Validate(Piece piece)
    {
        int id = piece.PieceType;

        if (id == PieceType.CH_Free.Id
         || id == PieceType.CH_NPC.Id)
        {
            return false;
        }
        
        return true;
    }
}