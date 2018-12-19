using UnityEngine;
using System.Collections;

public class UIOptionsWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIOptionsWindowModel windowModel = new UIOptionsWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
