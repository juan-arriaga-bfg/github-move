using System.Collections.Generic;

public class TouchReactionDefinitionFog : TouchReactionDefinitionComponent
{
    private FogDef def;

    public bool IsOpen;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
        
        if (def == null)
        {
            var pos = new BoardPosition(position.X, position.Y);
            
            if (GameDataService.Current.FogsManager.Fogs.TryGetValue(pos, out def) == false) return false;
        }
        
        IsOpen = !IsOpen;

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
        
        return true;
    }
}