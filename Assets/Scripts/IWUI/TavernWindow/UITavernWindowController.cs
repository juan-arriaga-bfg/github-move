using UnityEngine;
using System.Collections;

public class UITavernWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UITavernWindowModel windowModel = new UITavernWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
