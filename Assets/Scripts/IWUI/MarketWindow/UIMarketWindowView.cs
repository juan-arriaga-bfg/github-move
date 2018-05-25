using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
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
        
        SetTitle(windowModel.Title);
        
        foreach (var task in windowModel.Tasks)
        {
            var item = Instantiate(taskPattern, taskPattern.transform.parent).GetComponent<UIMarketTaskItem>();
            
            item.Init(task.Def.Rewards);
            taskItems.Add(item);
        }
        
        taskPattern.SetActive(false);
        var toggle = taskItems[0].gameObject.GetComponent<Toggle>();
        toggle.isOn = true;
        GameDataService.Current.TasksManager.Timer.OnComplete += AddTask;
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
        GameDataService.Current.TasksManager.Timer.OnComplete -= AddTask;
    }

    private void AddTask()
    {
        var windowModel = Model as UIMarketWindowModel;
        
        if(taskItems.Count == windowModel.Tasks.Count) return;
        
        var item = Instantiate(taskPattern, taskPattern.transform.parent).GetComponent<UIMarketTaskItem>();
        
        item.Init(windowModel.Tasks[windowModel.Tasks.Count - 1].Def.Rewards);
        taskItems.Add(item);
        item.gameObject.SetActive(true);
    }
    
    public void OnSelect(Toggle toggle)
    {
        if(toggle.isOn == false) return;
        
        var windowModel = Model as UIMarketWindowModel;
        
        windowModel.SelectIndex = taskItems.FindIndex(item => item.gameObject.GetComponent<Toggle>() == toggle);
        
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
        buttonLabel.Text = windowModel.Selected.IsComplete ? "Get" : string.Format("Get now {0}", windowModel.Selected.GetHardPrice().ToStringIcon(false));
    }

    public void OnClick()
    {
        var windowModel = Model as UIMarketWindowModel;

        if (!windowModel.Selected.Exchange()) return;
        
        var item = taskItems[windowModel.SelectIndex];
        
        windowModel.Tasks.RemoveAt(windowModel.SelectIndex);
        
        GameDataService.Current.TasksManager.Update();

        taskItems.Remove(item);
        Destroy(item.gameObject);
        
        if (taskItems.Count == 0)
        {
            Controller.CloseCurrentWindow();
            return;
        }

        var toggle = taskItems[0].gameObject.GetComponent<Toggle>();
        toggle.isOn = true;
    }
}