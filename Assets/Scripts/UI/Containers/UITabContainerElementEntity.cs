using System;

public class UITabContainerElementEntity : IUIContainerElementEntity 
{
    public string Uid { get; set; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public string TabLabel { get; set; }

    public override string ToString()
    {
        return string.Format("TabLabel:{0}", TabLabel);
    }
}