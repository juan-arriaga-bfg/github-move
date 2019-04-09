[TaskHighlight(typeof(HighlightTaskTreeBranch))]
[TaskHighlight(typeof(HighlightTaskAnyTree))]
[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskKillTreeEntity : TaskHitTreeEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.ObstacleKilled;
}