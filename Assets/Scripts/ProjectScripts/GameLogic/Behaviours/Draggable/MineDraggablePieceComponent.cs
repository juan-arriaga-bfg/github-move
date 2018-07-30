using YamlDotNet.Core.Tokens;

public class MineDraggablePieceComponent : DraggablePieceComponent
{
    private Piece context;
    private MulticellularPieceBoardObserver multicellular;

    public MulticellularPieceBoardObserver Multicellular
    {
        get
        {
            return multicellular ?? (multicellular =
                       context.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver
                           .ComponentGuid));
        }
        set { multicellular = value; }
    }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        context = entity as Piece;
    }

    public override bool IsDraggable(BoardPosition at)
    {
        if(!base.IsDraggable(at))
            return false;

        foreach (var maskPos in Multicellular.Mask)
        {
            if (context.Context.BoardLogic.IsLockedCell(multicellular.GetPointInMask(at, maskPos)))
                return false;
        }
        //return Pathfinder.CanPathToCastle(context);
        return true;
    }
}