using System;
using Newtonsoft.Json;

namespace Quests
{
    [Serializable]
    public class SubtaskCurrencyCounter : SubtaskBase
    {
        [JsonProperty] public int CurrentValue { get; protected set; }
        [JsonProperty] public CurrencyPair TargetValue { get; protected set; }

        public virtual SubtaskState State
        {
            get
            {
                return CurrentValue >= TargetValue.Amount ? SubtaskState.Completed : SubtaskState.InProgress;
            }
        }
    }
}