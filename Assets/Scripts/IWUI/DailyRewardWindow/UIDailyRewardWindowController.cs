using UnityEngine;
using System.Collections;

public class UIDailyRewardWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIDailyRewardWindowModel windowModel = new UIDailyRewardWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
