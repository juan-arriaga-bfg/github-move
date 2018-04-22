using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UICollectionWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private Image icon;
    
    [SerializeField] private GameObject button;

    private List<UiCollectionItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UICollectionWindowModel;

        if (items == null)
        {
            items = GetComponentsInChildren<UiCollectionItem>().ToList();
        }
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UICollectionWindowModel;
        
    }
}
