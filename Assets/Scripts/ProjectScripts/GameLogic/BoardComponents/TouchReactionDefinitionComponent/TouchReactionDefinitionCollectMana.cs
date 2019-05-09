using System.Linq;

public class TouchReactionDefinitionCollectMana : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var data = GameDataService.Current.FogsManager;
        var fogs = data.VisibleFogPositions.Values.Select(e => data.GetFogObserver(e.GetCenter())).ToList();

        fogs = fogs.FindAll(fog => fog.CanBeCleared() && fog.CanBeFilled(piece, false));
        fogs.Sort((a, b) => a.GetDeltaFill().CompareTo(b.GetDeltaFill()));
        fogs = fogs.FindAll(fog => fog.GetDeltaFill() == fogs[0].GetDeltaFill());
        fogs.Sort((a, b) => a.Def.Level.CompareTo(b.Def.Level));
        
        var observer = fogs[0];
        var target = observer.Context.CachedPosition;

        observer.CanBeFilled(piece, true); // add

        if (observer.CanBeFilled(null, false) == false) // check
        {
            piece.Context.Manipulator.CameraManipulator.MoveTo(observer.Def.GetCenter(piece.Context));
        }
        
        piece.Context.ActionExecutor.AddAction(new CollapseManaAction
        {
            Mana = piece,
            FogPosition = target,
            OnComplete = () => data.SetMana(piece, target)
        });
        
        return true;
    }
}