using UnityEngine;
using System.Collections;

public class UIEventSubscriptionWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIEventSubscriptionWindowModel windowModel = new UIEventSubscriptionWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
