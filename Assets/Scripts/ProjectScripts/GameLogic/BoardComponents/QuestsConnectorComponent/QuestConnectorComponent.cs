public class QuestConnectorComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        var manager = GameDataService.Current.QuestsManager;
        manager.CreateStarters();
        manager.ConnectToBoard();
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    } 
}