public class TouchReactionDefinitionOpenBubble : TouchReactionDefinitionComponent
{
	public ViewType ViewId;
	
	private ViewDefinitionComponent viewDef;
	
	public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
	{
		return viewDefinition != null && viewDefinition.AddView(ViewId).IsShow;
	}
    
	public override bool Make(BoardPosition position, Piece piece)
	{
		
		piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, position);
        
		if (viewDef == null)
		{
			viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
            
			if (viewDef == null) return false;
		}
        
		var view = viewDef.AddView(ViewId);
        
		view.Change(!view.IsShow);
        
		return true;
	}
}