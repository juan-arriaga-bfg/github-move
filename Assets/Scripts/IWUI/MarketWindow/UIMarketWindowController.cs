using UnityEngine;
using System.Collections;

public class UIMarketWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIMarketWindowModel windowModel = new UIMarketWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
