using UnityEngine;
using System.Collections;

public class UIConfirmRemoverMessageWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIConfirmRemoverMessageWindowModel windowModel = new UIConfirmRemoverMessageWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
