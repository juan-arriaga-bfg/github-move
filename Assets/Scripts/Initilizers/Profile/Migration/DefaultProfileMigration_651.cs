using System.Collections.Generic;
using System.Linq;

public partial class DefaultProfileMigration
{
    private static void Migrate651(int clientVersion, UserProfile profile)
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

        const string QUEST_ID = "128_CreatePiece_NPC_F3";

        if (questSave.FinishedQuests.Contains(QUEST_ID))
        {
            return;
        }

        if (codexSave.Data.TryGetValue(PieceType.NPC_F1.Id, out CodexChainState state))
        {
            foreach (var id in idsToCheck)
            {
                if (state.Unlocked.Contains(id))
                {
                    questSave.FinishedQuests.Add(QUEST_ID);

                    var active = questSave.ActiveQuests.FirstOrDefault(e => e.Quest.Id == QUEST_ID);
                    if (active != null)
                    {
                        questSave.ActiveQuests.Remove(active);
                        return;
                    }
                }
            }
        }
    }
}
