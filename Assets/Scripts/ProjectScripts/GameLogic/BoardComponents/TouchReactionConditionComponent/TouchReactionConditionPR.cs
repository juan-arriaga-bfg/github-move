using System.Collections.Generic;

public class TouchReactionConditionPR : TouchReactionConditionStorage
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        var save = ProfileService.Current.GetComponent<TutorialSaveComponent>(TutorialSaveComponent.ComponentGuid)?.Complete ?? new List<int>();

        return save.Contains(8) && base.Check(position, piece);
    }
}