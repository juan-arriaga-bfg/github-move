using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDailyQuestWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#TaskList")] private UIContainerViewController taskList;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIDailyQuestWindowModel model = Model as UIDailyQuestWindowModel;
       
        SetTitle(model.Title);

        CreateTaskList(model);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIDailyQuestWindowModel windowModel = Model as UIDailyQuestWindowModel;
        
    }
    
    private void CreateTaskList(UIDailyQuestWindowModel model)
    {
        var tasks = model.Tasks;
        
        if (tasks == null || tasks.Count <= 0)
        {
            taskList.Clear();
            return;
        }

        var tabViews = new List<IUIContainerElementEntity>(tasks.Count);
        for (int i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];

            var tabEntity = new UIDailyQuestTaskElementEntity
            {
                Task = task,
                WindowController = Controller,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            tabViews.Add(tabEntity);
        }
        
        taskList.Create(tabViews);
        taskList.Select(0);
    }
}
