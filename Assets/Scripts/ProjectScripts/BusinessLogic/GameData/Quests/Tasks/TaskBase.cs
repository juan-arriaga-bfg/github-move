using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Quests
{
    [Serializable]
    public class TaskBase : IHaveId
    {
        // Predefined
        [JsonProperty] public string Id { get; protected set; }
        [JsonProperty] public string Message { get; protected set; }
        [JsonProperty] public string Ico { get; protected set; }
        [JsonProperty] public List<CurrencyPair> Reward  { get; protected set; }
        [JsonProperty] public List<string> SubtaskIds  { get; protected set; }
        
        public List<SubtaskBase> Subtasks  { get; protected set; }

        public Action<TaskBase> OnChanged; 

        public virtual TaskState State
        {
            get
            {
                foreach (var subtask in Subtasks)
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
            foreach (var subtask in Subtasks)
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