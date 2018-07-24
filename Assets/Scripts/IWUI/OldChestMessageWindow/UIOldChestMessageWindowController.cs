using UnityEngine;
using System.Collections;

public class UIOldChestMessageWindowController : IWWindowController {

    public override IWWindowModel CreateModel()
    {
        UIOldChestMessageWindowModel windowModel = new UIOldChestMessageWindowModel();



        return windowModel;
    }

    public override void UpdateWindow(IWUIWindow window)
    {
        base.UpdateWindow(window);
    }
}
