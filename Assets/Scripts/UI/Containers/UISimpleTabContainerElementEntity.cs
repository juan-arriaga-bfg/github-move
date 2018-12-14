using System;

public class UISimpleTabContainerElementEntity : IUIContainerElementEntity
{
    public string Uid { get; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public string LabelText;
    public string CheckmarkText;
}