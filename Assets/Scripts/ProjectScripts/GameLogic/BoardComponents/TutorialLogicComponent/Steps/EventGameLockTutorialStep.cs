public class EventGameLockTutorialStep : BaseTutorialStep
{
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        BoardService.Current?.FirstBoard?.BoardLogic.EventGamesLogic.Locker.Lock(this);
    }
	
    protected override void Complete()
    {
        base.Complete();
        
        var logic = BoardService.Current?.FirstBoard?.BoardLogic.EventGamesLogic;

        logic?.Locker.Unlock(this);
        logic?.Check();
    }
}