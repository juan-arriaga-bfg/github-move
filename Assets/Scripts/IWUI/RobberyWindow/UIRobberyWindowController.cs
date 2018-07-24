using UnityEngine;
using System.Collections;

public class UIRobberyWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIRobberyWindowModel windowModel = new UIRobberyWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
