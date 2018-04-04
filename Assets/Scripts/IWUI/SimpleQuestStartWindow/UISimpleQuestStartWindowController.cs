using UnityEngine;
using System.Collections;

public class UISimpleQuestStartWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UISimpleQuestStartWindowModel windowModel = new UISimpleQuestStartWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
