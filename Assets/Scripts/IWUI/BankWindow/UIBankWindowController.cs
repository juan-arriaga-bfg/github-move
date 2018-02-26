using UnityEngine;
using System.Collections;

public class UIBankWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIBankWindowModel windowModel = new UIBankWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
