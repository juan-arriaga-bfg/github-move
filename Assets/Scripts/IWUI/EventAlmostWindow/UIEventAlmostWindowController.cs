using UnityEngine;
using System.Collections;

public class UIEventAlmostWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIEventAlmostWindowModel windowModel = new UIEventAlmostWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
