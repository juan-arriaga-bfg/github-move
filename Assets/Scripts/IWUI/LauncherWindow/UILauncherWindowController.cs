using UnityEngine;
using System.Collections;

public class UILauncherWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UILauncherWindowModel windowModel = new UILauncherWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
