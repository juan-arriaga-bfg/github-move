// Product branch
[TaskHighlight(typeof(HighlightTaskPointToPiece),                             new[] {typeof(HighlightConditionPieceFromProductBranch)})]
[TaskHighlight(typeof(HighlightTaskPointToPredecessor),                       new[] {typeof(HighlightConditionPieceFromProductBranch)})]
[TaskHighlight(typeof(HighlightTaskAnyBag),                                   new[] {typeof(HighlightConditionPieceFromProductBranch)})]
[TaskHighlight(typeof(HighlightTaskPointToRandomChestExcludingFreeAndNpc),    new[] {typeof(HighlightConditionPieceFromProductBranch)})]
[TaskHighlight(typeof(HighlightTaskFindDeadProductionFieldForPieceType),      new[] {typeof(HighlightConditionPieceFromProductBranch)})]
[TaskHighlight(typeof(HighlightTaskFindObstacleOfAnyTypeExcludingProduction), new[] {typeof(HighlightConditionPieceFromProductBranch)})]
[TaskHighlight(typeof(HighlightTaskFirstMineOfAnyType),                       new[] {typeof(HighlightConditionPieceFromProductBranch)})]
[TaskHighlight(typeof(HighlightTaskNextFog),                                  new[] {typeof(HighlightConditionPieceFromProductBranch)})]

// Other
[TaskHighlight(typeof(HighlightTaskPointToPredecessor))]
[TaskHighlight(typeof(HighlightTaskFindObstacleForPieceType))]
[TaskHighlight(typeof(HighlightTaskFindMineForPieceType))]
[TaskHighlight(typeof(HighlightTaskFindChestForPieceType))]
[TaskHighlight(typeof(HighlightTaskNextFog))]

// ReSharper disable once RedundantExtendsListEntry
public class TaskCreatePieceEntity : TaskCounterAboutPiece, IBoardEventListener, IHavePieceId
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public override void ConnectToBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.PieceBuilded);
    }

    public override void DisconnectFromBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, GameEventsCodes.PieceBuilded); 
    }

    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.PieceBuilded)
        {
            return;
        }
        
        if (!IsInProgress())
        {
            return;
        }

        if (PieceId == (int)context)
        {
            CurrentValue += 1;
        }
    }
    
    public override string ToString()
    {
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: {PieceType.GetDefById(PieceId).Abbreviations[0]} - {CurrentValue}/{TargetValue}";
        return ret;
    }
    
    protected override string ValidateDeserializedData()
    {
        if (PieceId == PieceType.Empty.Id || PieceId == PieceType.None.Id)
        {
            return "Abstract 'Empty' or 'None' pieces not supported as target. Use specific piece type";
        }

        return null;
    }
}