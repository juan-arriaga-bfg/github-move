public class TouchReactionDefinitionMine : TouchReactionDefinitionComponent
{
	private ViewDefinitionComponent viewDef;
	
	public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
	{
		return viewDefinition != null && viewDefinition.AddView(ViewType.MineState).IsShow;
	}
    
	public override bool Make(BoardPosition position, Piece piece)
	{
		piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, position);
        
		if (viewDef == null)
		{
			viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
            
			if (viewDef == null) return false;
		}
        
		var view = viewDef.AddView(ViewType.MineState);
        
		view.Change(!view.IsShow);
        
		return true;
	}
}