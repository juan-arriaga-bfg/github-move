public class AnimationStartAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	public BoardAnimation Animation;
    
	public bool PerformAction(BoardController gameBoardController)
	{
		gameBoardController.RendererContext.AddAnimationToQueue(Animation);
		return true;
	}
}