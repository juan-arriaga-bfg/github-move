public abstract class DamageablePieceComponent : IECSComponent, IDamageable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid
    {
        get { return ComponentGuid; }
    }

    protected Piece context;

    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as Piece;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public abstract bool PerformHit(int damage, BoardPosition piecePosition, Piece attaker, BoardPosition attakerPosition);
    
    public bool IsDamageable(BoardPosition at)
    {
        int currentLayer = at.Z;
        for (int layer = 0; layer < context.Context.BoardDef.Depth; layer++)
        {
            var point = new BoardPosition(at.X, at.Y, layer);
            var piece = context.Context.BoardLogic.GetPieceAt(point);
            var overrideDestroy = piece == null ? null : piece.GetComponent<OverrideDestroyPieceComponent>(OverrideDestroyPieceComponent.ComponentGuid);
            if (overrideDestroy != null)
            {
                bool state = overrideDestroy.IsDestroyableAtLayer(currentLayer);
                if (state == false)
                {
                    return state;
                }
            }
        }
        return true;
    }
}