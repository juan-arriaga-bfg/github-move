using UnityEngine;
using System.Collections;

public class UIHardShopWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIHardShopWindowModel windowModel = new UIHardShopWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
