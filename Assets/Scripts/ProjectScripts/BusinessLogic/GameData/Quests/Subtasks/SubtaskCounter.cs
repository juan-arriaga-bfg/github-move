namespace Quests
{
    public class SubtaskCounter : SubtaskBase
    {
        protected int currentValue;
        protected CurrencyPair targetValue;

        public virtual SubtaskState State
        {
            get
            {
                return currentValue >= targetValue.Amount ? SubtaskState.Completed : SubtaskState.InProgress;
            }
        }
    }
}