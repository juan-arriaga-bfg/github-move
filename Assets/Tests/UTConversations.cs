using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        [Test]
        public void UTQuestCharactersPasses()
        {
            LogAssert.ignoreFailingMessages = true;
            
            StringBuilder errors = new StringBuilder();
            
            GameDataManager dataManager = new GameDataManager();
            GameDataService.Instance.SetManager(dataManager);
            
            QuestsDataManager questsDataManager = new QuestsDataManager();
            ConversationsDataManager conversationsDataManager = new ConversationsDataManager();
            
            dataManager.RegisterComponent(questsDataManager);
            dataManager.RegisterComponent(conversationsDataManager);

            questsDataManager.Reload();
            questsDataManager.CreateStarters();

            conversationsDataManager.Reload();
                       
            // Check chars list
            var convManager = GameDataService.Current.ConversationsManager;
            convManager.Reload();
            
            foreach (var pair in convManager.Cache)
            {
                ConversationScenarioEntity scenario = convManager.BuildScenario(pair.Key);
                ConversationScenarioCharacterListComponent charList = scenario.GetComponent<ConversationScenarioCharacterListComponent>(ConversationScenarioCharacterListComponent.ComponentGuid);
                for (var i = 0; i < scenario.Actions.Count; i++)
                {
                    ConversationActionEntity action = scenario.Actions[i];
                    var bubbleAction = action as ConversationActionBubbleEntity;
                    if (bubbleAction == null)
                    {
                        continue;
                    }

                    if (!charList.ConversationCharacters.Values.Contains(bubbleAction.CharacterId))
                    {
                        errors.AppendLine($"Scenario [{pair.Key}] bubble #{i} refers to char {bubbleAction.CharacterId} that not listed in chars list");
                    }
                }
            }

            var starters = questsDataManager.QuestStarters;

            foreach (var starter in starters)
            {
                List<QuestStartConditionComponent> conditions = starter.Conditions.Where(e => e is QuestStartConditionQuestCompletedComponent).ToList();
                if (conditions.Count == 0)
                {
                    continue;
                }

                foreach (var condition in conditions)
                {
                    var dependencyId = condition.Value;
                    var finishScenario = convManager.BuildScenario(dependencyId);
                    var startScenario = convManager.BuildScenario(starter.Id);

                    if (finishScenario == null)
                    {
                        //errors.AppendLine($"No complete dialog for quest '{dependencyId}' defined");
                        continue;
                    }
                    
                    if (startScenario == null)
                    {
                        //errors.AppendLine($"No start dialog for starter' {starter.Id}' defined");
                        continue;
                    }
                    
                    var finishBubble = (finishScenario.GetFirstAction() as ConversationActionBubbleEntity);
                    var finishBubbleChar = finishBubble.CharacterId;
                    var finishBubbleSide = finishBubble.Side;
                    
                    var startBubble = (startScenario.GetFirstAction() as ConversationActionBubbleEntity);
                    var startBubbleChar = startBubble.CharacterId;
                    var startBubbleSide = startBubble.Side;

                    if (finishBubbleChar != startBubbleChar)
                    {
                        errors.AppendLine($"Different chars detected for quest complete dialog [{dependencyId}] and depended starter [{starter.Id}]: '{finishBubbleChar}' <-> '{startBubbleChar}'");
                    }
                    
                    if (finishBubbleSide != startBubbleSide)
                    {
                        errors.AppendLine($"Different chars sides detected for quest complete dialog [{dependencyId}] and depended starter [{starter.Id}]: '{finishBubbleSide}' <-> '{startBubbleSide}'");
                    }
                }
            }
            
            LogAssert.ignoreFailingMessages = false;
            
            var message = errors.ToString();
            if (string.IsNullOrEmpty(message))
            {
                Assert.Pass("ALL OK");
            }
            else
            {
                Assert.Fail(message);
            }
        }
    }
}
