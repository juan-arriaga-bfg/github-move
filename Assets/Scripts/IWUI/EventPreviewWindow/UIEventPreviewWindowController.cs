using UnityEngine;
using System.Collections;

public class UIEventPreviewWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIEventPreviewWindowModel windowModel = new UIEventPreviewWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
