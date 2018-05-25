using System;
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
    [SerializeField] private Image hero;
    [SerializeField] private NSText reward;
    [SerializeField] private NSText rewardAll;
    
    private List<UIMarketTaskItem> taskItems = new List<UIMarketTaskItem>();
    private Task CompleteTask;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMarketWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        foreach (var task in windowModel.Tasks)
        {
            var item = Instantiate(taskPattern, taskPattern.transform.parent).GetComponent<UIMarketTaskItem>();
            
            item.Init(task.Character, task.Rewards, task.IsComplete);
            taskItems.Add(item);
        }
        
        taskPattern.SetActive(false);
        var toggle = taskItems[0].gameObject.GetComponent<Toggle>();
        toggle.isOn = true;
        GameDataService.Current.TasksManager.Timer.OnComplete += AddTask;
    }

    public override void OnViewCloseCompleted()
    {
        if (CompleteTask != null)
        {
            CompleteTask.Ejection();
            CompleteTask = null;
        }
        
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
        var task = windowModel.Tasks[windowModel.Tasks.Count - 1];
        
        item.Init(task.Character, task.Rewards, task.IsComplete);
        taskItems.Add(item);
        item.gameObject.SetActive(true);
    }
    
    public void OnSelect(Toggle toggle)
    {
        if(toggle.isOn == false) return;
        
        var windowModel = Model as UIMarketWindowModel;
        
        windowModel.SelectIndex = taskItems.FindIndex(item => item.gameObject.GetComponent<Toggle>() == toggle);
        
        var selected = windowModel.Selected;
        
        for (var i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            var isActive = i < selected.Prices.Count;
            
            target.gameObject.SetActive(isActive);
            
            if(isActive) target.Init(selected.Prices[i]);
        }

        hero.sprite = IconService.Current.GetSpriteById(selected.Character);
        icon.sprite = IconService.Current.GetSpriteById(selected.Result.Currency);
        
        reward.Text = string.Format("x{0}", selected.Result.Amount);
        rewardAll.Text = string.Format("Reward: {0}", taskItems[windowModel.SelectIndex].GetString());
        buttonLabel.Text = windowModel.Selected.IsComplete ? "Get" : string.Format("Get now {0}", windowModel.Selected.GetHardPrice().ToStringIcon(false));
    }

    public void OnClick()
    {
        var windowModel = Model as UIMarketWindowModel;

        var isComplete = windowModel.Selected.Exchange();
        
        if (isComplete == false) return;
        
        CompleteTask = windowModel.Selected;
        
        var item = taskItems[windowModel.SelectIndex];
        
        windowModel.Tasks.RemoveAt(windowModel.SelectIndex);
        
        GameDataService.Current.TasksManager.Update();

        var index = 0;

        foreach (var task in windowModel.Tasks)
        {
            taskItems[index].Init(task.Character, task.Rewards, task.IsComplete);
            index++;
        }

        taskItems.Remove(item);
        Destroy(item.gameObject);
        Controller.CloseCurrentWindow();
        
        if (taskItems.Count == 0) return;

        var toggle = taskItems[0].gameObject.GetComponent<Toggle>();
        toggle.isOn = true;
    }

    public void Upgrade()
    {
        var windowModel = Model as UIMarketWindowModel;
        
        if (windowModel.Upgrade())
        {
            Controller.CloseCurrentWindow();
        }
    }
}