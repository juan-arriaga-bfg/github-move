using UnityEngine;
using System.Collections;

public class UIFireflyHintWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIFireflyHintWindowModel windowModel = new UIFireflyHintWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
