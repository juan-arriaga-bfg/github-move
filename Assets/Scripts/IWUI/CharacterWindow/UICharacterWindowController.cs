using UnityEngine;
using System.Collections;

public class UICharacterWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UICharacterWindowModel windowModel = new UICharacterWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
