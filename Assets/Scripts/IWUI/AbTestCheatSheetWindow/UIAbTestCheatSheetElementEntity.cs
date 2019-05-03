using System;

public class UIAbTestCheatSheetElementEntity : IUIContainerElementEntity 
{
    public string Uid { get; set; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }
    public IWWindowController WindowController { get; set; }

    public AbTestItem Data;
}