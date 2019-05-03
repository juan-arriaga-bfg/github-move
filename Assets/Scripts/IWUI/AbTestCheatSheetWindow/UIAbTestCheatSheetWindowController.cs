using UnityEngine;
using System.Collections;

public class UIAbTestCheatSheetWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIAbTestCheatSheetWindowModel windowModel = new UIAbTestCheatSheetWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
