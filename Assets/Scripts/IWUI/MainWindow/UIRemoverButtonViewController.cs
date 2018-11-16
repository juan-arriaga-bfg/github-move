using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRemoverButtonViewController : IWUIWindowViewController
{
    [SerializeField] private Image icon;

    [SerializeField] private NSText label;

    [SerializeField] private Transform labelPanelView;

    [SerializeField] private Transform shineView;

    [SerializeField] private Transform warningView;

    [SerializeField] private Transform iconView;
    
    public override void OnViewInit(IWUIWindowView context)
    {
        base.OnViewInit(context);
        
        
    }

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);
        
        
    }

    public void OnClick()
    {
        
    }
}
