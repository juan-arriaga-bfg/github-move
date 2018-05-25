using UnityEngine;

public class TouchReactionDefinitionProduction : TouchReactionDefinitionComponent
{
    public ProductionComponent Production { get; set; }

    private ViewDefinitionComponent viewDef;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, Production);

        if (Production.IsActive == false)
        {
            UIErrorWindowController.AddError(string.Format("This building will be unlocked at Level {0}", Production.Level));
            return false;
        }
        
        switch (Production.State)
        {
            case ProductionState.Waiting:
//                UIMessageWindowController.CreateDefaultMessage("!!!!!!!!!!!");
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