using UnityEngine;
using System.Collections;

public class UINextLevelWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UINextLevelWindowModel windowModel = new UINextLevelWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
