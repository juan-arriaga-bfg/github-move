public class TouchReactionDefinitionObstacle : TouchReactionDefinitionComponent
{
    private ViewDefinitionComponent viewDef;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, position);
        
        if (viewDef == null)
        {
            viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
            
            if (viewDef == null) return false;
        }
        
        var view = viewDef.AddView(ViewType.Obstacle);
        
        view.Change(!view.IsShow);
        
        var hint = piece.Context.GetComponent<HintCooldownComponent>(HintCooldownComponent.ComponentGuid);
        
        if(hint == null) return true;
        
        hint.Step(HintType.Obstacle);
        
        return true;
    }
}