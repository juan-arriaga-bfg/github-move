using System;

public class UIQuestCheatSheetElementEntity : IUIContainerElementEntity 
{
    public string Uid { get; set; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public TaskEntity Task { get; set; }
    public QuestEntity Quest { get; set; }
    public IWWindowController WindowController { get; set; }
}