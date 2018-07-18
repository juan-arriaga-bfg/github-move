using UnityEngine;
using System.Collections.Generic;

public class UICastleWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private GameObject itemPattern;
    [SerializeField] private NSText upgrade;
    
    private List<GameObject> items = new List<GameObject>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UICastleWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        upgrade.Text = windowModel.UpgradeMessage;
        
        var chests = windowModel.Chests;
        
        foreach (var chest in chests)
        {
            var item = Instantiate(itemPattern, itemPattern.transform.parent).GetComponent<UICastleItem>();
            item.Init(chest);
            items.Add(item.gameObject);
        }
        
        itemPattern.SetActive(false);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UICastleWindowModel;
    }
    
    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        itemPattern.SetActive(true);

        foreach (var item in items)
        {
            Destroy(item);
        }
        
        items = new List<GameObject>();
        
        var windowModel = Model as UICastleWindowModel;
        
        windowModel.Spawn();
    }
    
    public void OnClick()
    {
        var windowModel = Model as UICastleWindowModel;

        if (windowModel.Upgrade())
        {
            Controller.CloseCurrentWindow();
        }
    }
}