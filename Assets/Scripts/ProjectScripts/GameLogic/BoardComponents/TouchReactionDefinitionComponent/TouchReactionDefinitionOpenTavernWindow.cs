public class TouchReactionDefinitionOpenTavernWindow : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var model = UIService.Get.GetCachedModel<UITavernWindowModel>(UIWindowType.TavernWindow);

        model.Obstacle = null;
        
        UIService.Get.ShowWindow(UIWindowType.TavernWindow);
        return true;
    }
}