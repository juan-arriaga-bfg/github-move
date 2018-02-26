using System;

public class TouchReactonConditionDelayComponent : TouchReactionConditionComponent
{
    public int Delay;

    private DateTime startTime;
    
    public override void Recharge()
    {
        startTime = DateTime.Now;
        base.Recharge();
    }

    public override bool Check(BoardPosition position, Piece piece)
    {
        if (IsDone) return true;
        
        IsDone = (DateTime.Now - startTime).TotalSeconds >= Delay;
        
        return IsDone;
    }
}