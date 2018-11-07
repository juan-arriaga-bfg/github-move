using UnityEngine;
using System.Collections;

public class UIQuestCompleteWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIQuestCompleteWindowModel windowModel = new UIQuestCompleteWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
