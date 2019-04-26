using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UT
{
    public class UTQuest
    {
        [Test]
        public void UTQuestInstantiatePasses()
        {
            LogAssert.ignoreFailingMessages = false;

            QuestsDataManager questsDataManager = new QuestsDataManager();
            questsDataManager.Reload();

            InstantiateAll<QuestStarterEntity>(questsDataManager);
            InstantiateAll<QuestEntity>(questsDataManager);
            InstantiateAll<TaskEntity>(questsDataManager);
            
            Assert.Pass();
        }

        private void InstantiateAll<T>(QuestsDataManager questsDataManager) where T : IECSComponent
        {
            Dictionary<string, JToken> configs = questsDataManager.Cache[typeof(T)];

            var ids = new List<string>();
            
            foreach (var config in configs)
            {
                T obj = questsDataManager.InstantiateFromJson<T>(config.Value);
                
                if (ids.Contains(config.Key))
                {
                    Assert.Fail($"Duplicate id '{config.Key}' for type: '{typeof(T)}' ");
                }
                else
                {
                    ids.Add(config.Key);
                }
            }
        }

        [Test]
        public void UTQuestStarterConditionsPassed()
        {
            LogAssert.ignoreFailingMessages = true;
            
            QuestsDataManager questsDataManager = new QuestsDataManager();
            questsDataManager.Reload();
            if (!questsDataManager.CreateStarters())
            {
                LogAssert.ignoreFailingMessages = false;
                Assert.Fail("Can't create starters");
                return;
            } 
            
            FogsDataManager fogManager = new FogsDataManager();
            fogManager.Reload();

            var fogIds = fogManager.Fogs.Select(e => e.Uid).ToList();
            
            StringBuilder sb = new StringBuilder();
            
            var starters = questsDataManager.QuestStarters;

            foreach (var starter in starters)
            {
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
                }

                foreach (var condition in starter.Conditions)
                {
                    if (!(condition is QuestStartConditionAlwaysTrueComponent) 
                     && !(condition is QuestStartConditionAlwaysFalseComponent))
                    {
                        if (string.IsNullOrEmpty(condition.Value))
                        {
                            sb.AppendLine($"Starter '{starter.Id}' Condition {condition.GetType()} with id '{condition.Id}' has null or empty Value");
                            continue;
                        }
                    }

                    switch (condition)
                    {
                        case QuestStartConditionQuestCompletedComponent cmp:
                            var quest = questsDataManager.InstantiateQuest(cmp.QuestId);
                            if (quest == null)
                            {
                                sb.AppendLine($"Starter '{starter.Id}' Condition QuestCompleted with id '{condition.Id}' has reference to not existing quest '{cmp.QuestId}'");
                            }
                            break;
                        
                        case QuestStartConditionFogClearedComponent cmp:
                            if (!fogIds.Contains(cmp.FogUid))
                            {
                                sb.AppendLine($"Starter '{starter.Id}' Condition FogCleared with id '{condition.Id}' has reference to not existing fog '{cmp.FogUid}'");
                            }
                            break;
                        
                        case QuestStartConditionPieceUnlockedComponent cmp:
                            if (PieceType.Parse(cmp.Value) == PieceType.None.Id)
                            {
                                sb.AppendLine($"Starter '{starter.Id}' Condition PieceUnlocked with id '{condition.Id}' has reference to not existing piece '{cmp.Value}'");
                            }
                            break;
                    }
                }
            }

            LogAssert.ignoreFailingMessages = false;
            
            var message = sb.ToString();
            if (string.IsNullOrEmpty(message))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail(message);
            }
        }
        
        [Test]
        public void UTQuestIdsPasses()
        {
            LogAssert.ignoreFailingMessages = true;

            QuestsDataManager questsDataManager = new QuestsDataManager();
            questsDataManager.Reload();
            if (!questsDataManager.CreateStarters())
            {
                LogAssert.ignoreFailingMessages = false;
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

            LogAssert.ignoreFailingMessages = false;
            
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

            const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_.";

            foreach (var c in id)
            {
                if (!ALPHABET.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }

        [Test]
        public void UTQuestStartersConflictsPasses()
        {
            LogAssert.ignoreFailingMessages = true;
            
            QuestsDataManager questsDataManager = new QuestsDataManager();
            questsDataManager.Reload();

            questsDataManager.CreateStarters();

            var starters = questsDataManager.QuestStarters;

            HashSet<string> idsInMultiplyStarters = new HashSet<string>();
            
            StringBuilder sb = new StringBuilder();
            
            foreach (var starterToCheck in starters)
            {
                var idsToCheck = starterToCheck.QuestToStartIds;
                foreach (var id in idsToCheck)
                {
                    foreach (var starterToCompare in starters)
                    {
                        if (starterToCheck.Id == starterToCompare.Id)
                        {
                            continue;
                        }

                        var idsToCompare = starterToCompare.QuestToStartIds;
                        if (idsToCompare.Contains(id))
                        {
                            if (!idsInMultiplyStarters.Add(id))
                            {
                                continue;
                            }
                            
                            if (idsToCheck.Count > 1 || idsToCompare.Count > 1)
                            {
                                sb.Append($"Quest '{id}' listed in two starters '{starterToCheck.Id}' and '{starterToCompare.Id}' ");
                                if (idsToCheck.Count > 1 && idsToCompare.Count > 1)
                                {
                                    sb.Append($"but both starters contains more than one id.");
                                }
                                else if (idsToCheck.Count > 1)
                                {
                                    sb.Append($"but starter '{starterToCheck.Id}' contains more than one id.");
                                }
                                else
                                {
                                    sb.Append($"but starter '{starterToCompare.Id}' contains more than one id.");
                                }

                                sb.AppendLine(" It may cause a case when all other quests in pack will never be started");
                            }
                        }
                    }
                }
            }
            
            LogAssert.ignoreFailingMessages = false;
            
            Debug.Log($"Ids listed in multiply starters:\n{string.Join("\n", idsInMultiplyStarters)}\n");
            
            var message = sb.ToString();
            if (string.IsNullOrEmpty(message))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail(message);
            }
        }
        
        [Test]
        public void UTTaskPieceIds()
        {
            LogAssert.ignoreFailingMessages = true;

            QuestsDataManager questsDataManager = new QuestsDataManager();
            questsDataManager.Reload();
            if (!questsDataManager.CreateStarters())
            {
                LogAssert.ignoreFailingMessages = false;
                Assert.Fail("Can't create starters");
                return;
            }

            var sb = new StringBuilder();
            
            var tasks = questsDataManager.Cache[typeof(TaskEntity)];
            foreach (var taskJson in tasks.Values)
            {
                TaskEntity task = questsDataManager.InstantiateFromJson<TaskEntity>(taskJson);
                FieldInfo[] fields = task.GetType().GetFields(BindingFlags.Public | 
                                                              BindingFlags.NonPublic | 
                                                              BindingFlags.Instance);

                FieldInfo pieceUidField = fields.FirstOrDefault(e => e.Name == "PieceUid");
                if (pieceUidField == null)
                {
                    continue;
                }

                string pieceUid = pieceUidField.GetValue(task) as string;
                if (string.IsNullOrEmpty(pieceUid))
                {
                    continue;
                }

                switch (pieceUid)
                {
                    case "None":
                    case "Empty":
                        continue;
                }

                int parsedId = PieceType.Parse(pieceUid);
                if (parsedId == PieceType.None.Id)
                {
                    sb.Append($"Task '{task.Id}' has invalid PieceUid: '{pieceUid}'");
                }
            }
            

            LogAssert.ignoreFailingMessages = false;
            
            var message = sb.ToString();
            if (string.IsNullOrEmpty(message))
            {
                Assert.Pass($"Tasks checked: {tasks.Count}");
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
    }
}