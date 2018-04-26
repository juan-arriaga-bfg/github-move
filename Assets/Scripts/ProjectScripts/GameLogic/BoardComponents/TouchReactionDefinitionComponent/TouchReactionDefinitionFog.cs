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
            def = GameDataService.Current.FogsManager.GetDef(pos);

            if (def == null) return false;
        }
        
        IsOpen = !IsOpen;

        if (GameDataService.Current.HeroesManager.CurrentPower() >= def.Condition.Value)
        {
            var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
            model.Title = "Clear fog area!";
            model.Message = "Your Band Power are enough for opening new part of Sherwood forest";
            model.AcceptLabel = "Clear";
        
            model.OnAccept = () =>
            {
                piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                {
                    To = position,
                    Positions = new List<BoardPosition>{position}
                });
            };
            
            model.OnCancel = null;
        
            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            
            return true;
        }
        
        return true;
    }
}