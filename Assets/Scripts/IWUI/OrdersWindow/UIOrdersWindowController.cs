using UnityEngine;
using System.Collections;

public class UIOrdersWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIOrdersWindowModel windowModel = new UIOrdersWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
