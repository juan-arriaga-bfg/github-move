using UnityEngine;
using System.Collections;

public class UIExchangeWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIExchangeWindowModel windowModel = new UIExchangeWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
