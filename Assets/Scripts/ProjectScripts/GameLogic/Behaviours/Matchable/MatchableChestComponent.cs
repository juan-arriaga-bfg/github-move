public class MatchableChestComponent : MatchablePieceComponent
{
    private RewardsStoreComponent reward;
    
    public override bool IsMatchable()
    {
        if (base.IsMatchable() == false) return false;
        if (reward != null) return reward.IsComplete == false;
        
        var component = context.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
        
        reward = component?.Rewards;
        
        return reward != null && reward.IsComplete == false;
    }
}