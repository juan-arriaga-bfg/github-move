using System;
using Newtonsoft.Json;

namespace Quests
{
    [Serializable]
    public class SubtaskBase : IHaveId
    {
        // Predefined
        [JsonProperty] public string Id { get; protected set; }
        [JsonProperty] public string Message { get; protected set; }
        [JsonProperty] public string Ico { get; protected set; }
        
        public Action<SubtaskBase> OnChanged; 
        
        public virtual SubtaskState State
        {
            get { throw new NotImplementedException("Override me!");}
        }

        public virtual void Init()
        {
            throw new NotImplementedException("Override me!");
        }
    }
}