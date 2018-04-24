using UnityEngine;
using System.Collections;

public class UIQuestWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIQuestWindowModel windowModel = new UIQuestWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
