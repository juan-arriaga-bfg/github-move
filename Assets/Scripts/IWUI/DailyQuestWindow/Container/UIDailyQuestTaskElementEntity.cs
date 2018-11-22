using System;

public class UIDailyQuestTaskElementEntity : IUITabElementEntity 
{
    public string Uid { get; set; }
    public Action<UITabElementViewController> OnSelectEvent { get; set; }
    public Action<UITabElementViewController> OnDeselectEvent { get; set; }

    public TaskEntity Task { get; set; }

    public override string ToString()
    {
        return $"Task id: '{Task.Id}'";
    }
}