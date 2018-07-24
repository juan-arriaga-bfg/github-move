using UnityEngine;
using System.Collections;

public class UIChestMessageWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIChestMessageWindowModel windowModel = new UIChestMessageWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
