/// <summary>
/// Highlight first existing mine from mines chain 
/// </summary>
public class HighlightTaskFirstMineOfAnyType : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        //var board = BoardService.Current.GetBoardById(0);
        // var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);
        
        var mineIds = PieceType.GetIdsByFilter(PieceTypeFilter.Mine);
        foreach (var mineId in mineIds)
        {
            if (HighlightTaskMineHelper.Highlight(mineId))
            {
                return true;
            }
        }

        return false;
    }
}