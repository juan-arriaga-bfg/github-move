public class TouchReactionConditionCharacter : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		var customer = piece.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);

		return customer?.Order != null;
	}
}