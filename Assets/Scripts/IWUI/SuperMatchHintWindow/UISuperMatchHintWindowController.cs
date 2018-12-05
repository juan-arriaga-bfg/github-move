using UnityEngine;
using System.Collections;

public class UISuperMatchHintWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UISuperMatchHintWindowModel windowModel = new UISuperMatchHintWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
