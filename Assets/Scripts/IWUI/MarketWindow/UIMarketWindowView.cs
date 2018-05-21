using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIMarketWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private GameObject taskPattern;
    [SerializeField] private List<UIMarketTargetItem> targets;
    
    private List<UIMarketTaskItem> taskItems = new List<UIMarketTaskItem>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMarketWindowModel;
        Toggle toggle = null;
        
        SetTitle(windowModel.Title);
        
        foreach (var task in windowModel.Tasks)
        {
            var item = Instantiate(taskPattern, taskPattern.transform.parent).GetComponent<UIMarketTaskItem>();

            if (toggle == null) toggle = item.gameObject.GetComponent<Toggle>();
            
            item.Init(task.Def.Rewards);
            taskItems.Add(item);
        }
        
        taskPattern.SetActive(false);
        toggle.Select();
    }
    
    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIMarketWindowModel;
        
        taskPattern.SetActive(true);
    }
    
    public void OnSelect(Transform toggle)
    {
        var windowModel = Model as UIMarketWindowModel;
        
        windowModel.SelectIndex = toggle.GetSiblingIndex();

        var selected = windowModel.Selected.Def;
        
        SetMessage(selected.Message);

        for (var i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            var isActive = i < selected.Prices.Count;
            
            target.gameObject.SetActive(isActive);
            
            if(isActive) target.Init(selected.Prices[i]);
        }
    }

    public void OnClick()
    {
        
    }
}