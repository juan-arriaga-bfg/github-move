public class TouchReactionDefinitionSpawnRewards : TouchReactionDefinitionComponent
{
    private RewardsStoreComponent storage;
    
    public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
    {
        return viewDefinition != null && viewDefinition.AddView(ViewType.RewardsBubble).IsShow;
    }

    public override bool Make(BoardPosition position, Piece piece)
    {
        if (storage == null)
        {
            storage = piece.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);
            if (storage == null) return false;
        }
        
        if (storage.IsComplete == false)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("message.error.notComplete", "message.error.notComplete"));
            return false;
        }
        
        storage.GetInBubble();
        return true;
    }
}