using System;

namespace Quests
{
    public class SubtaskBase
    {
        public string Id { get; protected set; }
        public string Message { get; protected set; }
        public string Ico { get; protected set; }
        
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