using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIMarketWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private GameObject taskPattern;
    [SerializeField] private List<UIMarketTargetItem> targets;
    [SerializeField] private Image icon;
    [SerializeField] private NSText reward;
    [SerializeField] private NSText rewardAll;
    
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
        toggle.isOn = true;
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();

        foreach (var item in taskItems)
        {
            Destroy(item.gameObject);
        }
        
        taskItems = new List<UIMarketTaskItem>();
        
        taskPattern.SetActive(true);
    }
    
    public void OnSelect(Toggle toggle)
    {
        if(toggle.isOn == false) return;
        
        var windowModel = Model as UIMarketWindowModel;
        
        windowModel.SelectIndex = toggle.transform.GetSiblingIndex() - 1;
        
        var selected = windowModel.Selected.Def;
        
        SetMessage(selected.Message);

        for (var i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            var isActive = i < selected.Prices.Count;
            
            target.gameObject.SetActive(isActive);
            
            if(isActive) target.Init(selected.Prices[i]);
        }

        icon.sprite = IconService.Current.GetSpriteById(selected.Result.Currency);
        
        reward.Text = string.Format("x{0}", selected.Result.Amount);
        rewardAll.Text = string.Format("Reward: {0}", taskItems[windowModel.SelectIndex].GetString());
    }

    public void OnClick()
    {
        
    }
}