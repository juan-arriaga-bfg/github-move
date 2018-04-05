using System.Collections.Generic;

public class TouchReactionDefinitionFog : TouchReactionDefinitionComponent
{
    private FogDef def;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        if (def == null)
        {
            var pos = new BoardPosition(position.X, position.Y);
            
            if (GameDataService.Current.FogsManager.Fogs.TryGetValue(pos, out def) == false) return false;
        }

        if (GameDataService.Current.HeroesManager.CurrentPower() >= def.Condition.Value)
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

        UIMessageWindowController.CreateDefaultMessage("Not enough power!");
        
        return true;
    }
}