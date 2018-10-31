using System.Collections.Generic;
using System.Linq;

public class Chest
{
    public ChestDef Def;
    
    private int rewardCount;
    private Dictionary<int, int> reward = new Dictionary<int, int>();
    
    public Dictionary<int, int> Reward
    {
        get { return GetRewardPieces(); }
        set { reward = value; }
    }

    public int RewardCount
    {
        get { return rewardCount; }
        set { rewardCount = value; }
    }
    
    public bool CheckStorage()
    {
        var currentRewardCount = Reward.Values.Sum();
        return currentRewardCount < rewardCount;
    }
    
    public Dictionary<int, int> GetRewardPieces()
    {
        if (reward.Count != 0) return reward;
        
        var hard = GameDataService.Current.LevelsManager.GetSequence(Currency.Level.Name);
        var sequence = GameDataService.Current.ChestsManager.GetSequence(Def.Uid);

        var productionAmount = Def.ProductionAmount.Range();
        reward = hard.GetNextDict(productionAmount);
        reward = sequence.GetNextDict(Def.PieceAmount, reward);

        rewardCount = productionAmount + Def.PieceAmount;
        
        return reward;
    }
}