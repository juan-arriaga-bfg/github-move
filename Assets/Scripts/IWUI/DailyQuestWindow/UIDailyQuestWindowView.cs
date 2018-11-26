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

        SortTasks(tasks);
        
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

    private void SortTasks(List<TaskEntity> tasks)
    {
        tasks.Sort((item1, item2) =>
        {
            const int CLAIMED_WEIGHT = 10000;
            const int COMPLETED_WEIGHT = -1000;
            
            int w1 = (int)item1.Group;
            int w2 = (int)item2.Group;

            if (item1.IsClaimed())
            {
                w1 += CLAIMED_WEIGHT;
            }
            else if (item1.IsCompleted())
            {
                w1 += COMPLETED_WEIGHT;
            }

            if (item2.IsClaimed())
            {
                w2 += CLAIMED_WEIGHT;
            }
            else if (item2.IsCompleted())
            {
                w2 += COMPLETED_WEIGHT;
            }
            
            return w1 - w2;
        });
    }
}
