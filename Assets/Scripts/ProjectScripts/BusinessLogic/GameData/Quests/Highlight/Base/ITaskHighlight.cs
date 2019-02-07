using System;

public interface ITaskHighlight
{
    bool Highlight(TaskEntity task);
}

public interface ITaskHighlightCondition
{
    bool Check(TaskEntity task);
}