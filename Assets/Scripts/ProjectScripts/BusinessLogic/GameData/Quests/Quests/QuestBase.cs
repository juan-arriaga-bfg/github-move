using System;
using System.Collections.Generic;

namespace Quests
{
    public class QuestBase
    {
        protected List<TaskBase> tasks;
        protected int currentTaskIndex;

        public Action<QuestBase> OnChanged; 
        
        protected virtual void Init()
        {
            foreach (var task in tasks)
            {
                task.OnChanged += OnTaskChaged;
            }
        }

        protected void OnTaskChaged(TaskBase task)
        {
            OnChanged(this);
        }

    }
}