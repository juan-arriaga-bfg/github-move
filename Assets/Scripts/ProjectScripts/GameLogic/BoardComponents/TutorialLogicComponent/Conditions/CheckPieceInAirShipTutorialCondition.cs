public class CheckPieceInAirShipTutorialCondition : BaseTutorialCondition
{
    public int Target;
    
    public override bool Check()
    {
        return context.Context.Context.BoardLogic.AirShipLogic.CheckPayload(Target);
    }
}