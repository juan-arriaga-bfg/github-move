using UnityEngine;
using System.Collections;

public class UICreditsWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UICreditsWindowModel windowModel = new UICreditsWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
