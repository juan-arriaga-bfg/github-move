using UnityEngine;
using System.Collections;

public class UIChestRewardWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIChestRewardWindowModel windowModel = new UIChestRewardWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
