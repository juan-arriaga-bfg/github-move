using UnityEngine;
using System.Collections;

public class UICharactersWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UICharactersWindowModel windowModel = new UICharactersWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
