using System;

public class TouchReactionDefinitionOpenBubble : TouchReactionDefinitionComponent
{
	public ViewType ViewId;
	public Action<bool> OnChange;
	
	public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
	{
		return viewDefinition != null && viewDefinition.AddView(ViewId).IsShow;
	}
    
	public override bool Make(BoardPosition position, Piece piece)
	{
		if (piece.ViewDefinition == null) return false;
        
		var view = piece.ViewDefinition.AddView(ViewId);
        
		OnChange?.Invoke(!view.IsShow);
		view.Change(!view.IsShow);
		
		return true;
	}
}