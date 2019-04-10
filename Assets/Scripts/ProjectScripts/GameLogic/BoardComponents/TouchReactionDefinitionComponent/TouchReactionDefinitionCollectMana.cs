using System.Linq;

public class TouchReactionDefinitionCollectMana : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var data = GameDataService.Current.FogsManager;
        var fogs = data.VisibleFogPositions.Values.Select(e => data.GetFogObserver(e.GetCenter())).ToList();

        fogs = fogs.FindAll(fog => fog.CanBeCleared() && fog.CanBeFilled());
        fogs.Sort((a, b) => a.AlreadyPaid.Amount.CompareTo(b.AlreadyPaid.Amount));
        fogs = fogs.FindAll(fog => fog.AlreadyPaid.Amount == fogs[0].AlreadyPaid.Amount);
        fogs.Sort((a, b) => a.Def.Level.CompareTo(b.Def.Level));

        var target = fogs[0].Context.CachedPosition;
        
        piece.Context.ActionExecutor.AddAction(new CollapseManaAction
        {
            Mana = piece,
            FogPosition = target,
            OnComplete = () => data.SetMana(piece, target)
        });
        
        return true;
    }
}