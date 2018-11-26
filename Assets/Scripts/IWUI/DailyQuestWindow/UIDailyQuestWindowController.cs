using UnityEngine;
using System.Collections;

public class UIDailyQuestWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIDailyQuestWindowModel windowModel = new UIDailyQuestWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
