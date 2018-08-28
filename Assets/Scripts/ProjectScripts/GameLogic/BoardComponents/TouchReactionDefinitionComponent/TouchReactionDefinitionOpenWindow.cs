public class TouchReactionDefinitionOpenWindow : TouchReactionDefinitionComponent
{
    public string WindowType;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        UIService.Get.ShowWindow(WindowType);
        return true;
    }
}