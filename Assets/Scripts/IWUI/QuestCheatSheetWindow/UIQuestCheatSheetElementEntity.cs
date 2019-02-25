using System;

public class UIQuestCheatSheetElementEntity : IUIContainerElementEntity 
{
    public string Uid { get; set; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }
    public IWWindowController WindowController { get; set; }

    public string QuestId;
    
    public UIQuestCheatSheetWindowModel MainModel;
    public QuestEntity Quest => MainModel.GetQuestById(QuestId);
}