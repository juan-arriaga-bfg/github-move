using UnityEngine;
using System.Collections;

public class UIQuestStartWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIQuestStartWindowModel windowModel = new UIQuestStartWindowModel();

        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
    
    public override void OnBackButtonPress()
    {
        var view = WindowView as UIQuestStartWindowView;
        view.OnClick();
    }
}
