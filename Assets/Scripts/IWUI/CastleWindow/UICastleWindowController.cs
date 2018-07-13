using UnityEngine;
using System.Collections;

public class UICastleWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UICastleWindowModel windowModel = new UICastleWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
