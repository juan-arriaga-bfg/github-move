using UnityEngine;
using System.Collections;

public class UIShopWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIShopWindowModel windowModel = new UIShopWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
