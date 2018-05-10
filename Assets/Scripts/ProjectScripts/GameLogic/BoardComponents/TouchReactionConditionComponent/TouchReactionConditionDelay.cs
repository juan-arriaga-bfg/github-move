using System;

public class TouchReactionConditionDelay : TouchReactionConditionComponent
{
    public int Delay;

    private DateTime startTime;
    
    public DateTime StartTime
    {
        get { return startTime; }
    }

    public override void Recharge()
    {
        startTime = DateTime.UtcNow;
        base.Recharge();
    }
    
    public override bool Check(BoardPosition position, Piece piece)
    {
        if (IsDone) return true;
        
        IsDone = (DateTime.UtcNow - startTime).TotalSeconds >= Delay;
        
        return IsDone;
    }
}