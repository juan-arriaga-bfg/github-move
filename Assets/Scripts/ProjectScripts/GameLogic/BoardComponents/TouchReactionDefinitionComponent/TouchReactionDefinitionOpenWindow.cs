public class TouchReactionDefinitionOpenWindow : TouchReactionDefinitionComponent
{
    public string WindowType { get; set; }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        UIService.Get.ShowWindow(WindowType);
        return true;
    }
}