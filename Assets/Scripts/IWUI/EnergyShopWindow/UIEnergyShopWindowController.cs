using UnityEngine;
using System.Collections;

public class UIEnergyShopWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIEnergyShopWindowModel windowModel = new UIEnergyShopWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
