using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace UT
{
    public class UTConversations
    {

        [Test]
        public void UTConversationsPasses()
        {
            GameDataManager dataManager = new GameDataManager();
            GameDataService.Instance.SetManager(dataManager);
            
            QuestsDataManager questsDataManager = new QuestsDataManager();
            ConversationsDataManager conversationsDataManager = new ConversationsDataManager();
            
            dataManager.RegisterComponent(questsDataManager);
            dataManager.RegisterComponent(conversationsDataManager);

            
            questsDataManager.Reload();
            questsDataManager.CreateStarters();

            conversationsDataManager.Reload();

            var ids = conversationsDataManager.GetAvailableScenarioIds();
            foreach (var id in ids)
            {
                conversationsDataManager.BuildScenario(id);
            }
            
            Assert.Pass($"{ids.Count} items checked.");
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        // [UnityTest]
        // public IEnumerator UTConversationsWithEnumeratorPasses() {
        //     // Use the Assert class to test conditions.
        //     // yield to skip a frame
        //     yield return null;
        // }
    }
}
