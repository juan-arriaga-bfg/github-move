using UnityEngine;
using System.Collections;

public class UIProfileCheatSheetWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIProfileCheatSheetWindowModel windowModel = new UIProfileCheatSheetWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
