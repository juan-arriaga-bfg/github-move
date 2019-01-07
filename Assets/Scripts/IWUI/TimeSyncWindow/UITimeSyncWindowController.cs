using UnityEngine;
using System.Collections;

public class UITimeSyncWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UITimeSyncWindowModel windowModel = new UITimeSyncWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
