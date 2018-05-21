public class Task
{
    private TaskDef def;

    public TaskDef Def
    {
        get { return def; }
    }
    
    public Task(TaskDef def)
    {
        this.def = def;
    }
}