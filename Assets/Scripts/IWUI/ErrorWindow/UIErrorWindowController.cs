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
        
        if(model.Messages.ContainsKey(message)) return;

        model.Messages.Add(message, message);
        
        UIService.Get.ShowWindow(UIWindowType.ErrorWindow);
    }
}
