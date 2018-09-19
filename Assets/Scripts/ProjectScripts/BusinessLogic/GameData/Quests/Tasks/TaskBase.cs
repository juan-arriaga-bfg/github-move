using System;
using System.Collections.Generic;

namespace Quests
{
    public class TaskBase
    {
        protected string message;
        protected List<CurrencyPair> reward;
        
        protected List<SubtaskBase> subtasks;
        
        public Action<TaskBase> OnChanged; 

        public virtual TaskState State
        {
            get
            {
                foreach (var subtask in subtasks)
                {
                    if (subtask.State != SubtaskState.Completed)
                    {
                        return TaskState.InProgress;
                    }
                }

                return TaskState.Completed;
            }
        }
        
        protected virtual void Init()
        {
            foreach (var subtask in subtasks)
            {
                subtask.OnChanged += OnSubtaskChanged;
            }
        }
        
        protected void OnSubtaskChanged(SubtaskBase subtask)
        {
            OnChanged(this);
        }
    }
}