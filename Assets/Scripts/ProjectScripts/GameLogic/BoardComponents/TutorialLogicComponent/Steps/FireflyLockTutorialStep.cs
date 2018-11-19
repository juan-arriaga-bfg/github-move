public class FireflyLockTutorialStep : BaseTutorialStep
{
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        BoardService.Current?.FirstBoard?.BoardLogic.FireflyLogic.Locker.Lock(this);
    }
	
    protected override void Complete()
    {
        base.Complete();
        BoardService.Current?.FirstBoard?.BoardLogic.FireflyLogic.Locker.Unlock(this);
    }
}