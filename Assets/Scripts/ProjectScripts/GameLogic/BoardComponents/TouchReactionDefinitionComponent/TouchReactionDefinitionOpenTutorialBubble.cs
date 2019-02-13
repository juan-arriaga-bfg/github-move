public class TouchReactionDefinitionOpenTutorialBubble : TouchReactionDefinitionOpenBubble
{
    public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
    {
        if (viewDefinition == null) return false;

        var board = BoardService.Current.FirstBoard;
        var id = board.TutorialLogic.CheckLockPR() ? ViewId : ViewType.Bubble;
        
        return viewDefinition.AddView(id).IsShow;
    }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        if (piece.ViewDefinition == null) return false;

        if (piece.Context.TutorialLogic.CheckLockPR()) return base.Make(position, piece);
        
        var view = piece.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;

        view.SetData(
            LocalizationService.Get("common.message.forbidden", "common.message.forbidden"),
            LocalizationService.Get("common.button.ok", "common.button.ok"),
            (p) => { view.Change(false); },
            true,
            false
        );
        
        view.Change(!view.IsShow);
		
        return true;
    }
}