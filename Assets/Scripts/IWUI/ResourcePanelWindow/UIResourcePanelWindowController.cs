using UnityEngine;
using System.Collections;

public class UIResourcePanelWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIResourcePanelWindowModel windowModel = new UIResourcePanelWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
