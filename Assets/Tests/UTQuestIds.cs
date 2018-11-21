using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

public class UTQuestIds 
{

    [Test]
    public void UTQuestIdsPasses()
    {
        LogAssert.ignoreFailingMessages = true;
        
        QuestsDataManager questsDataManager = new QuestsDataManager();
        questsDataManager.Reload();
        if (!questsDataManager.CreateStarters())
        {
            Assert.Fail("Can't create starters");
            return;
        }
        
        StringBuilder sb = new StringBuilder();

        int startersCnt = 0;
        int questsCnt = 0;
        int tasksCnt = 0;
        
        var starters = questsDataManager.QuestStarters;
        
        foreach (var starter in starters)
        {
            startersCnt++;
            
            if (!ValidateId(starter.Id))
            {
                sb.AppendLine($"Wrong starter id '{starter.Id}'"); 
            }
            
            var questIds = starter.QuestToStartIds;
            foreach (var questId in questIds)
            {               
                if (!ValidateId(questId))
                {
                    sb.AppendLine($"Wrong quest id '{questId}' in starter '{starter.Id}'"); 
                    continue;
                }
                
                var quest = questsDataManager.InstantiateQuest(questId);
                if (quest == null)
                {
                    sb.AppendLine($"Starter '{starter.Id}' refers to not existed quest id '{questId}'"); 
                    continue;
                }
                
                questsCnt++;

                foreach (var def in quest.TaskDefs)
                {
                    if (!ValidateId(def.TaskId))
                    {
                        sb.AppendLine($"Wrong task id '{def.TaskId}' for quest '{quest.Id}'"); 
                    }
                    
                    var task = questsDataManager.InstantiateTask(def.TaskId);
                    if (task == null)
                    {
                        sb.AppendLine($"Quest '{quest.Id}' refers to not existed task id '{def.TaskId}'"); 
                    }

                    tasksCnt++;
                }
            }
        }

        // Can't handle reuse, commented out
        // int processedCount = startersCnt + questsCnt + tasksCnt;
        // int cacheCount = questsDataManager.GetCountOfItemsInCache();
        // if (processedCount != cacheCount)
        // {
        //     sb.AppendLine($"Only {processedCount} items used but {cacheCount} defined. Please ensure that all quests added to starters and all tasks added to quests."); 
        // }

        var message = sb.ToString();
        if (string.IsNullOrEmpty(message))
        {
            Assert.Pass($"Starters: {startersCnt}, quests: {questsCnt}, tasks: {tasksCnt}");
        }
        else
        {
            Assert.Fail(message);
        }
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    // [UnityTest]
    // public IEnumerator UTQuestIdsWithEnumeratorPasses() {
    //     // Use the Assert class to test conditions.
    //     // yield to skip a frame
    //     yield return null;
    // }

    private bool ValidateId(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_.";
        
        foreach (var c in id)
        {
            if (!alphabet.Contains(c))
            {
                return false;
            }
        }
        
        return true;
    }
}
