using System.Collections.Generic;

public class TouchReactionDefinitionFog : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        UIMessageWindowController.CreateDefaultMessage("Fog go home!", () =>
        {
            piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = position,
                Positions = new List<BoardPosition>{position}
            });
        });
        return true;
    }
}