using UnityEngine;
using System.Collections;

public class UIQuestCheatSheetWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIQuestCheatSheetWindowModel windowModel = new UIQuestCheatSheetWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
