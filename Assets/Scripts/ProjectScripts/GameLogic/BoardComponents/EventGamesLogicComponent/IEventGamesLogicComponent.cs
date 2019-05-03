public partial class BoardLogicComponent : IEventGamesLogicComponent
{
    protected EventGamesLogicComponent eventGamesLogic;
    public EventGamesLogicComponent EventGamesLogic => eventGamesLogic ?? (eventGamesLogic = GetComponent<EventGamesLogicComponent>(EventGamesLogicComponent.ComponentGuid));
}

public interface IEventGamesLogicComponent
{
    EventGamesLogicComponent EventGamesLogic{ get; }
}