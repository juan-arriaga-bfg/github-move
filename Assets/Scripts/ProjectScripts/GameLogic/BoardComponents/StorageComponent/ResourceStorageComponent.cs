public class ResourceStorageComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }
    
    public CurrencyPair Resources { get; private set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        var piece = entity as Piece;
        
        var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);

        if (def != null) Resources = def.SpawnResources;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
}