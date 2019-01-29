using System;
using System.Collections.Generic;

public abstract class AsyncInitComponentBase
{
    public List<Type> Dependencies { get; private set; } = new List<Type>();

    public int WeightForProgressbar { get; private set; } = 1;
    
    protected bool isCompleted;

    public virtual float Progress => isCompleted ? 1 : 0;

    public virtual bool IsCompleted => isCompleted;
    
    public Action<AsyncInitComponentBase> OnComplete { get; set; }

    public AsyncInitComponentBase SetDependencies(List<Type> dependencies)
    {
        Dependencies.AddRange(dependencies);
        return this;
    }
    
    public AsyncInitComponentBase SetDependency(Type dependency)
    {
        Dependencies.Add(dependency);
        return this;
    }
    
    public AsyncInitComponentBase SetWeightForProgressbar(int weightForProgressbar)
    {
        WeightForProgressbar = weightForProgressbar;
        return this;
    }

    public abstract void Execute();

    public override string ToString()
    {
        return $"{GetType()}, Progress: {Progress}, IsCompleted: {IsCompleted}";
    }
}