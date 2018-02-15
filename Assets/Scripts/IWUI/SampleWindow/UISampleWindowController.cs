using UnityEngine;
using System.Collections;

public class UISampleWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UISampleWindowModel windowModel = new UISampleWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
