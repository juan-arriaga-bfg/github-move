using System.Linq;

public class TouchReactionConditionMana : TouchReactionConditionComponent
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        var data = GameDataService.Current.FogsManager;
        var fogs = data.VisibleFogPositions.Values.Select(e => data.GetFogObserver(e.GetCenter())).ToList();

        foreach (var fog in fogs)
        {
            if (fog.CanBeCleared() == false || fog.CanBeFilled() == false) continue;
            
            return true;
        }
        
        UIErrorWindowController.AddError(LocalizationService.Get("message.error.manaClick", "message.error.manaClick"));
        return false;
    }
}