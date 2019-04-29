using System;

public class UIEventElementEntity : IUIContainerElementEntity
{
    public string Uid { get; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public EventStepDef Step;
}