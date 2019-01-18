using UnityEngine;
using System.Collections;

public class UIWaitWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIWaitWindowModel windowModel = new UIWaitWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
