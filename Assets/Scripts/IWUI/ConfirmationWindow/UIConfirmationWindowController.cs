using UnityEngine;
using System.Collections;

public class UIConfirmationWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIConfirmationWindowModel windowModel = new UIConfirmationWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
