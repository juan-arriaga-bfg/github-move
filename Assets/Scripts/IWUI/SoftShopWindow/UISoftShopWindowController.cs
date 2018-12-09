using UnityEngine;
using System.Collections;

public class UISoftShopWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UISoftShopWindowModel windowModel = new UISoftShopWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
