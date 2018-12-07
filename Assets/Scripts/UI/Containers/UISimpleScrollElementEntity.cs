using System;

public class UISimpleScrollElementEntity : IUIContainerElementEntity
{
    public string Uid { get; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public string ContentId;
    public string LabelText;
}