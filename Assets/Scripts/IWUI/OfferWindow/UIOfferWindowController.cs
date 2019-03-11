using UnityEngine;
using System.Collections;

public class UIOfferWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIOfferWindowModel windowModel = new UIOfferWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
