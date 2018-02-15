using UnityEngine;
using System.Collections;

public class UIMainWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIMainWindowModel windowModel = new UIMainWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
