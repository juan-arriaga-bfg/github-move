using UnityEngine;
using System.Collections;

public class UIStorageWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIStorageWindowModel windowModel = new UIStorageWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
