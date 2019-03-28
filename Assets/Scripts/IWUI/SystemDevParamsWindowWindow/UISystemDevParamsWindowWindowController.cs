using UnityEngine;
using System.Collections;

public class UISystemDevParamsWindowWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UISystemDevParamsWindowWindowModel windowModel = new UISystemDevParamsWindowWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
