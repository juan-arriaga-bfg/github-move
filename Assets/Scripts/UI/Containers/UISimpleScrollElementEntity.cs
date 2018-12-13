using System;

public class UISimpleScrollElementEntity : IUIContainerElementEntity
{
    public string Uid { get; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public string ContentId;
    public virtual string LabelText { get; set; }
    public float Alpha = 1;
}