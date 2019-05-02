public class TouchReactionDefinitionClaimOrderWindow : TouchReactionDefinitionComponent
{
	private OrderPieceComponent orderPieceComponent;
	
	public override bool Make(BoardPosition position, Piece piece)
	{
		if (orderPieceComponent == null) orderPieceComponent = piece.GetComponent<OrderPieceComponent>(OrderPieceComponent.ComponentGuid);
	    if (orderPieceComponent == null
	        || orderPieceComponent.Rewards.IsHighlight
	        || orderPieceComponent?.Rewards?.CheckOutOfCellsDropFullReward() == true) return false;
	    
	    orderPieceComponent?.Rewards.FullDrop();
		
		return true;
	}
}