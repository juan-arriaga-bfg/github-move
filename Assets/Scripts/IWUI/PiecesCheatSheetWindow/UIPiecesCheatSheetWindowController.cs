using UnityEngine;
using System.Collections;

public class UIPiecesCheatSheetWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIPiecesCheatSheetWindowModel windowModel = new UIPiecesCheatSheetWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
