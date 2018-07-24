using UnityEngine;
using System.Collections;

public class UIHeroesWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIHeroesWindowModel windowModel = new UIHeroesWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
