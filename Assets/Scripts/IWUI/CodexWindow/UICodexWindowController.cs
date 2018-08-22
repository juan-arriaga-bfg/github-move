public class UICodexWindowController : IWWindowController 
{
    public override IWWindowModel CreateModel()
    {
        UICodexWindowModel windowModel = new UICodexWindowModel();
        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
