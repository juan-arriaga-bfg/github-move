using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public partial class DefaultProfileMigration
{
    private static void Migrate604(int clientVersion, UserProfile profile) 
    {
        var questSave = profile.GetComponent<QuestSaveComponent>(QuestSaveComponent.ComponentGuid);
        var codexSave = profile.GetComponent<CodexSaveComponent>(CodexSaveComponent.ComponentGuid);

        if (codexSave?.Data == null)
        {
            return;
        }

        List<int> idsToCheck = new List<int>
        {
            PieceType.NPC_F3.Id,
            PieceType.NPC_F4.Id,
            PieceType.NPC_F5.Id,
            PieceType.NPC_F6.Id,
            PieceType.NPC_F7.Id,
            PieceType.NPC_F8.Id,
            PieceType.NPC_F.Id,
        };
        
        List<string> questsToComplete = new List<string>
        {
            "128_CreatePiece_NPC_F3",
            "129_CreatePiece_NPC_F4",
            "130_CreatePiece_NPC_F",
        };

        if (codexSave.Data.TryGetValue(PieceType.NPC_F1.Id, out CodexChainState state))
        {
            foreach (var questId in questsToComplete)
            {
                if (questSave.FinishedQuests.Contains(questId))
                {
                    continue;
                }

                foreach (var id in idsToCheck)
                {
                    if (state.Unlocked.Contains(id))
                    {
                        questSave.FinishedQuests.Add(questId);

                        for (var activeQuestIndex = questSave.ActiveQuests.Count - 1; activeQuestIndex >= 0; activeQuestIndex--)
                        {
                            var activeQuest = questSave.ActiveQuests[activeQuestIndex];
                            try
                            {
                                var activeQuestId = activeQuest.Data["Quest"]["Id"].ToString();
                                if (activeQuestId == questId)
                                {
                                    questSave.ActiveQuests.Remove(activeQuest);
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                IW.Logger.Log($"[DefaultProfileMigration] => Migrate604: Can't get id from quest data: {e.Message} - {activeQuest?.Data}");
                            }
                        }
                    }
                }
            }
        }
    }
}
