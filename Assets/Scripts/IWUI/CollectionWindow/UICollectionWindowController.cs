using UnityEngine;
using System.Collections;

public class UICollectionWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UICollectionWindowModel windowModel = new UICollectionWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
