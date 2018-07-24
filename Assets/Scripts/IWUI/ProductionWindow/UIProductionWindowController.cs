using UnityEngine;
using System.Collections;

public class UIProductionWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIProductionWindowModel windowModel = new UIProductionWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
