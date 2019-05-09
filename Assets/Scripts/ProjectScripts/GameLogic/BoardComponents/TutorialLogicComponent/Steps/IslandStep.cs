using DG.Tweening;

public class IslandStep : BaseTutorialStep
{
    public BoardPosition Target;
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();

        var logic = BoardService.Current.FirstBoard.BoardLogic.VIPIslandLogic;

        if (logic.State != VIPIslandState.Fog) return;

        logic.SpawnPieces();
        logic.State = VIPIslandState.Broken;
        
        if (Context.Context.Manipulator.CameraManipulator.CameraMove.IsLocked == false)
        {
            var position = Context.Context.BoardDef.GetSectorCenterWorldPosition(Target.X, Target.Y, 0);
            Context.Context.Manipulator.CameraManipulator.MoveTo(position);
        }
        
        DOTween.Sequence().InsertCallback(1f, () => logic.UpdateView(VIPIslandState.Broken, true));
    }
}