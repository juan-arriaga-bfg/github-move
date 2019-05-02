public class CheckPieceChangeTutorialCondition : BaseTutorialCondition
{
    public int Target;
    public int Amount = 1;
    private int? thenValue;
    
    private int then
    {
        get
        {
            if (thenValue == null) thenValue = context.Context.Context.BoardLogic.PositionsCache.GetCountByType(Target);
            
            return thenValue.Value;
        }
    }

    public override bool Check()
    {
        if (ConditionType == TutorialConditionType.Start)
        {
            return Сount(context.Context.Context.BoardLogic.PositionsCache.GetCountByType(Target));
        }

        return context.IsPerform && Сount(context.Context.Context.BoardLogic.PositionsCache.GetCountByType(Target));
    }

    private bool Сount(int current)
    {
        var value = (then - current);

        if (Amount > 0)
        {
            if (value < 0) return (Amount + value) <= 0;
            
            thenValue = context.Context.Context.BoardLogic.PositionsCache.GetCountByType(Target);
            return false;
        }

        if (Amount < 0)
        {
            if(value > 0) return (Amount + value) >= 0;
            
            thenValue = context.Context.Context.BoardLogic.PositionsCache.GetCountByType(Target);
            return false;
        }
        
        return false;
    }
}
