using System;
using System.Collections.Generic;

public abstract class AsyncInitItemBase
{
    public List<Type> Dependencies { get; private set; } = new List<Type>();

    public int WeightForProgressbar { get; private set; } = 1;
    
    protected bool isCompleted;

    public virtual float Progress => isCompleted ? 1 : 0;

    public virtual bool IsCompleted => isCompleted;
    
    public Action<AsyncInitItemBase> OnComplete { get; set; }

    public AsyncInitItemBase SetDependencies(List<Type> dependencies)
    {
        Dependencies.AddRange(dependencies);
        return this;
    }
    
    public AsyncInitItemBase SetDependency(Type dependency)
    {
        Dependencies.Add(dependency);
        return this;
    }
    
    public AsyncInitItemBase SetWeightForProgressbar(int weightForProgressbar)
    {
        WeightForProgressbar = weightForProgressbar;
        return this;
    }

    public abstract void Execute();
}