using System.Collections.Generic;

public class TouchReactionDefinitionFog : TouchReactionDefinitionComponent
{
    private FogDef def;
    private ViewDefinitionComponent viewDef;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, def);
        
        if (def == null)
        {
            var pos = new BoardPosition(position.X, position.Y);
            def = GameDataService.Current.FogsManager.GetDef(pos);
            
            if (def == null) return false;
        }
        
        if (viewDef == null)
        {
            viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
            
            if (viewDef == null) return false;
        }
        
        var view = viewDef.AddView(ViewType.FogState);
        
        view.Change(!view.IsShow);
        
        if (ProfileService.Current.GetStorageItem(Currency.Coins.Name).Amount < def.Condition.Value) return true;
        
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
}