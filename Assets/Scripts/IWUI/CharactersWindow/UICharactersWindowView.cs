using UnityEngine;
using System.Collections;

public class UICharactersWindowView : IWUIWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICharactersWindowModel windowModel = Model as UICharactersWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICharactersWindowModel windowModel = Model as UICharactersWindowModel;
        
    }
}
