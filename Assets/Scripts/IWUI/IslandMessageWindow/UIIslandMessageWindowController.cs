using UnityEngine;
using System.Collections;

public class UIIslandMessageWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIIslandMessageWindowModel windowModel = new UIIslandMessageWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
