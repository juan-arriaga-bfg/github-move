using System;

public class UIDailyQuestTaskElementEntity : IUIContainerElementEntity 
{
    public string Uid { get; set; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public TaskEntity Task { get; set; }
    public IWWindowController WindowController { get; set; }

    public override string ToString()
    {
        return $"Task id: '{Task.Id}'";
    }
}