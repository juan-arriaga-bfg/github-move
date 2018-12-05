using UnityEngine;

public class TouchReactionDefinitionSpawnRewards : TouchReactionDefinitionComponent
{
    public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
    {
        return viewDefinition != null && viewDefinition.AddView(ViewType.RewardsBubble).IsShow;
    }

    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);
        
        if (storage == null) return false;
        
        if (storage.IsComplete == false)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("message.error.notComplete", "message.error.notComplete"));
            return false;
        }

        storage.Get();
        return true;
    }
}