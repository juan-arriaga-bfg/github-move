using System.Linq;

public class TouchReactionConditionCharacter : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		var customer = piece.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);

		if (customer?.Order == null) return false;

		if (customer.Order.State != OrderState.Complete) return true;
		
		var amount = customer.Order.PiecesReward.Sum(e => e.Value);
			
		if(piece.Context.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, amount) == false)
		{
			UIErrorWindowController.AddError(LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"));
			return false;
		}
			
		customer.GetReward();
		
		return false;
	}
}