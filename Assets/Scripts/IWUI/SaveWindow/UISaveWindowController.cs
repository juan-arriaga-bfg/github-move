using UnityEngine;
using System.Collections;

public class UISaveWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UISaveWindowModel windowModel = new UISaveWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
