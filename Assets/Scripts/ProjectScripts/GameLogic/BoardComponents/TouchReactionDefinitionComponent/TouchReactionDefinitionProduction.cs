public class TouchReactionDefinitionProduction : TouchReactionDefinitionComponent
{
    public ProductionComponent Production { get; set; }

    private ViewDefinitionComponent viewDef;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, Production);
        
        switch (Production.State)
        {
            case ProductionState.Waiting:
                UIMessageWindowController.CreateDefaultMessage("!!!!!!!!!!!");
                break;
            case ProductionState.Completed:
                Production.Complete();
                break;
            default:
                Production.Change();
                break;
        }
        
        return true;
    }
}