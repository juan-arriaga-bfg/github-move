public class CheckPieceTutorialCondition : BaseTutorialCondition
{
	public int Target;
	public int Amount = 1;
	public bool MoreThan;

	public override bool Check()
	{
		var value = context.Context.Context.BoardLogic.PositionsCache.GetUnlockedPiecePositionsByType(Target).Count;

		return MoreThan ? value > Amount : value == Amount;
	}
}