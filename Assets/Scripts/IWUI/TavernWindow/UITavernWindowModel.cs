using System.Collections.Generic;

public class UITavernWindowModel : IWWindowModel 
{
    public int SelectIndex { get; set; }
    
    public string Title
    {
        get { return "Camping Trip"; }
    }
    
    public Task Selected
    {
        get { return GameDataService.Current.TasksManager.Tasks[SelectIndex]; }
    }
    
    public List<Task> Tasks
    {
        get { return GameDataService.Current.TasksManager.Tasks; }
    }
}