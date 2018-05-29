public class UIErrorWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIErrorWindowModel windowModel = new UIErrorWindowModel();
        
        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }

    public static void AddError(string message)
    {
        var model = UIService.Get.GetCachedModel<UIErrorWindowModel>(UIWindowType.ErrorWindow);
        
        model.Messages.Add(message);

        var window = UIService.Get.GetShowedWindowByName(UIWindowType.ErrorWindow);

        if (window == null)
        {
            UIService.Get.ShowWindow(UIWindowType.ErrorWindow);
            return;
        }
        
        var view = window.CurrentView as UIErrorWindowView;
        view.Next();
    }
}
