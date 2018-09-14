using UnityEngine;
using System.Collections;

public class UICurrencyCheatSheetWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UICurrencyCheatSheetWindowModel windowModel = new UICurrencyCheatSheetWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
