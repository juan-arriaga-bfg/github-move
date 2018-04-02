using UnityEngine;

public class FogTouchRegion : PieceTouchRegion
{
    private TouchReactionConditionFog condition;
    
    public override bool IsTouchableAt(Vector2 pos)
    {
        if (condition == null)
        {
            var reaction = context.Piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);

            if (reaction != null)
            {
                condition = reaction.GetComponent<TouchReactionConditionFog>(TouchReactionConditionFog.ComponentGuid);
            }
        }

        if (condition != null && condition.Check(context.Piece.CachedPosition, context.Piece))
        {
            return base.IsTouchableAt(pos);
        }
        
        return false;
    }
}