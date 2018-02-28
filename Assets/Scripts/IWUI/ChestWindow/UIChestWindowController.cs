using UnityEngine;
using System.Collections;

public class UIChestWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIChestWindowModel windowModel = new UIChestWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
