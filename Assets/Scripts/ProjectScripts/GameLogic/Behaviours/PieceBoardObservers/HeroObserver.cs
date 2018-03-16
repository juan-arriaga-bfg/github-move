public class HeroObserver : IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid
    {
        get { return ComponentGuid; }
    }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(context == null) return;
        
        var hero = GameDataService.Current.HeroesManager.GetHero(context.PieceType == PieceType.H1.Id ? "Robin" : "John");

        hero.HousePosition = position;
    }

    public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }
}