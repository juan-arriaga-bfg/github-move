[TaskHighlight(typeof(HighlightTaskAboutPiece))]
[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskCollectIngredientEntity : TaskCurrencyCollectEntity
{
    public new static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
}