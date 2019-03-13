using UnityEngine;
using System.Collections;

public class UICharacterUnlockedWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UICharacterUnlockedWindowModel windowModel = new UICharacterUnlockedWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
