using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStorageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText capacity;
    [SerializeField] private RectTransform line;

    [SerializeField] private GameObject tabPattern;
    [SerializeField] private GameObject itemPattern;
    
    private List<GameObject> items = new List<GameObject>();

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        var windowModel = Model as UIStorageWindowModel;
        
        SetTabValue(tabPattern, windowModel.Tabs[0]);
        
        for (var i = 1; i < windowModel.Tabs.Count; i++)
        {
            var tab = Instantiate(tabPattern, tabPattern.transform.parent);

            SetTabValue(tab, windowModel.Tabs[i]);
        }
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIStorageWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        capacity.Text = windowModel.Capacity;
        line.sizeDelta = new Vector2(537*windowModel.Progress, line.sizeDelta.y);
        tabPattern.GetComponent<Toggle>().isOn = true;

        var defs = Currency.GetCurrencyDefs(CurrencyTag.All);
        
        foreach (var def in defs)
        {
            var storageItem = ProfileService.Current.GetStorageItem(def.Name);
            if(storageItem.Amount == 0) continue;
            
            var item = Instantiate(itemPattern, itemPattern.transform.parent).GetComponent<UIStorageItem>();
            item.Init(storageItem);
            items.Add(item.gameObject);
        }
        
        itemPattern.SetActive(false);
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        itemPattern.SetActive(true);

        foreach (var item in items)
        {
            Destroy(item);
        }
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIStorageWindowModel;
    }

    public void OnClick()
    {
        var windowModel = Model as UIStorageWindowModel;

        if (windowModel.Upgrade())
        {
            Controller.CloseCurrentWindow();
        }
    }

    private void SetTabValue(GameObject target, string value)
    {
        var labels = target.GetComponentsInChildren<NSText>();

        foreach (var label in labels)
        {
            label.Text = value;
        }
    }
}
