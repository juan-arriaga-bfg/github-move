using UnityEngine;
using System.Collections;

public class UIEventWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIEventWindowModel windowModel = new UIEventWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
