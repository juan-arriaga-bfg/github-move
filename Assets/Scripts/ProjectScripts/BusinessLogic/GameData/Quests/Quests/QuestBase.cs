using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Quests
{
    [Serializable]
    public class QuestBase : IHaveId
    {
        // Predefined
        [JsonProperty] public string Id { get; protected set; }
        [JsonProperty] public string Message { get; protected set; }
        [JsonProperty] public string Ico { get; protected set; }
        [JsonProperty] public List<string> TaskIds { get; protected set; }
        [JsonProperty] public List<IQuestConditionOld> Conditions { get; protected set; }
        [JsonProperty] public List<CurrencyPair> Reward  { get; protected set; }
        
        // Dynamic
        [JsonProperty] public int CurrentTaskIndex { get; protected set; }
        
        public List<TaskBase> Tasks { get; protected set; }
        
        public Action<QuestBase> OnChanged; 
        
        protected virtual void Init()
        {
            foreach (var task in Tasks)
            {
                task.OnChanged += OnTaskChanged;
            }
        }

        protected void OnTaskChanged(TaskBase task)
        {
            OnChanged(this);
        }

        protected QuestState State
        {
            get { return QuestState.InProgress; }
        }
    }
}