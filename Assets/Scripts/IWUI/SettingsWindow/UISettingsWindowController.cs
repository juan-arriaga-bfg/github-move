using UnityEngine;
using System.Collections;

public class UISettingsWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UISettingsWindowModel windowModel = new UISettingsWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
