using System;

public class TouchReactonConditionDelay : TouchReactionConditionComponent
{
    public int Delay;

    private DateTime startTime;
    
    public DateTime StartTime
    {
        get { return startTime; }
    }

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