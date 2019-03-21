using Debug = IW.Logger;
using System.Collections.Generic;

public static class MigrationUtils
{
    public static void ReplacePieceIdFromTo(UserProfile profile, Dictionary<int, int> fromToDef)
    {
        if (profile.FieldDef != null)
        {
            if (profile.FieldDef.Pieces != null)
            {
                for (int i = 0; i < profile.FieldDef.Pieces.Count; i++)
                {
                    var pieceSaveItem = profile.FieldDef.Pieces[i];

                    int targetId = -1;
                    if (fromToDef.TryGetValue(pieceSaveItem.Id, out targetId))
                    {
                        Debug.LogWarning(string.Format("[MigrationUtils]: replaced PieceType.ID int FieldDef.Pieces {0} => {1}", pieceSaveItem.Id, targetId));
                        pieceSaveItem.Id = targetId;
                    }
                }
            }

            if (profile.FieldDef.Rewards != null)
            {
                for (int i = 0; i < profile.FieldDef.Rewards.Count; i++)
                {
                    var rewardsSaveItem = profile.FieldDef.Rewards[i];

                    foreach (var fromToDefPair in fromToDef)
                    {
                        var fromId = fromToDefPair.Key;
                        var toId = fromToDefPair.Value;

                        if (rewardsSaveItem.Reward.ContainsKey(fromId))
                        {
                            var val = rewardsSaveItem.Reward[fromId];
                            rewardsSaveItem.Reward.Remove(fromId);
                            rewardsSaveItem.Reward.Add(toId, val);
                            
                            Debug.LogWarning(string.Format("[MigrationUtils]: replaced PieceType.ID in FieldDef.Rewards {0} => {1}", fromId, toId));
                        }
                    }
                }
            }
        }

        if (profile.CodexSave != null && profile.CodexSave.Data != null)
        {
            foreach (var fromToDefPair in fromToDef)
            {
                var fromId = fromToDefPair.Key;
                var toId = fromToDefPair.Value;

                if (profile.CodexSave.Data.ContainsKey(fromId))
                {
                    var codexChainState = profile.CodexSave.Data[fromId];
                    ReplaceCodexChainStateFromTo(codexChainState, new Dictionary<int, int>(fromToDef));

                    profile.CodexSave.Data.Remove(fromId);
                    profile.CodexSave.Data.Add(toId, codexChainState);
                }
            }
        }

        if (profile.MarketSave != null && profile.MarketSave.Slots != null)
        {
            for (int i = 0; i < profile.MarketSave.Slots.Count; i++)
            {
                var marketSaveItem = profile.MarketSave.Slots[i];
                
                foreach (var fromToDefPair in fromToDef)
                {
                    var fromId = fromToDefPair.Key;
                    var toId = fromToDefPair.Value;

                    if (marketSaveItem.Piece == fromId)
                    {
                        marketSaveItem.Piece = toId;
                            
                        Debug.LogWarning(string.Format("[MigrationUtils]: replaced PieceType.ID in MarketSave.Slots {0} => {1}", fromId, toId));
                    }
                }
            }
        }
    }

    public static void ReplaceCodexChainStateFromTo(CodexChainState codexChainState, Dictionary<int, int> fromToDef)
    {
        foreach (var fromToDefPair in fromToDef)
        {
            var fromId = fromToDefPair.Key;
            var toId = fromToDefPair.Value;
            
            if (codexChainState.Unlocked.Contains(fromId))
            {
                codexChainState.Unlocked.Remove(fromId);
                codexChainState.Unlocked.Add(toId);

                Debug.LogWarning(string.Format("[MigrationUtils]: replaced PieceType.ID in CodexChainState.Unlocked {0} => {1}", fromId, toId));
            }

            if (codexChainState.PendingReward.Contains(fromId))
            {
                codexChainState.PendingReward.Remove(fromId);
                codexChainState.PendingReward.Add(toId);

                Debug.LogWarning(string.Format("[MigrationUtils]: replaced PieceType.ID in CodexChainState.PendingReward {0} => {1}", fromId, toId));
            }
        }
    }

}