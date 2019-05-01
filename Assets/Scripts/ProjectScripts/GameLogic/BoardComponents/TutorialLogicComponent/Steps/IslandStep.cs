using DG.Tweening;

public class IslandStep : BaseTutorialStep
{
    public BoardPosition Target;
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        BoardService.Current.FirstBoard.BoardLogic.VIPIslandLogic.SpawnPieces();
        BoardService.Current.FirstBoard.BoardLogic.VIPIslandLogic.UpdateView(VIPIslandState.Broken, true);
        
        base.Perform();

        if (BoardService.Current.FirstBoard.BoardLogic.VIPIslandLogic.State != VIPIslandState.Fog) return;
        
        if (Context.Context.Manipulator.CameraManipulator.CameraMove.IsLocked) return;
        
        var position = Context.Context.BoardDef.GetSectorCenterWorldPosition(Target.X, Target.Y, 0);
        Context.Context.Manipulator.CameraManipulator.MoveTo(position);
        
        DOTween.Sequence().InsertCallback(1f, () => BoardService.Current.FirstBoard.BoardLogic.VIPIslandLogic.UpdateView(VIPIslandState.Broken, true));
    }
}