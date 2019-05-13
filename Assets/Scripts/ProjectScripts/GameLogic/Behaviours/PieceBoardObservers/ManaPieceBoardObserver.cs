public class ManaPieceBoardObserver : IECSComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private Piece contextPiece;
    private FieldControllerComponent locker;
    private CurrencyPair mana;

    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
        locker = contextPiece.Context.GetComponent<FieldControllerComponent>(FieldControllerComponent.ComponentGuid);
        
        var storage = contextPiece.GetComponent<ResourceStorageComponent>(ResourceStorageComponent.ComponentGuid);
        
        mana = new CurrencyPair{Currency = Currency.ManaFake.Name, Amount = storage.Resources.Amount};
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if (locker.IsCreateComplete == false) return;

        CurrencyHelper.Purchase(mana);
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if (locker.IsCreateComplete == false) return;
        
        CurrencyHelper.Purchase(Currency.Cash.Name, 0, mana);
    }
}