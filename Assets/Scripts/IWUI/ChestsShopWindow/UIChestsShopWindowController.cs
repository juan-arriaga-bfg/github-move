using UnityEngine;
using System.Collections;

public class UIChestsShopWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIChestsShopWindowModel windowModel = new UIChestsShopWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
