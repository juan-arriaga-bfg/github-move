using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIQuestStartWindowModel : IWWindowModel
{
    public List<QuestEntity> Quests { get; private set; }
    public ConversationScenarioEntity Scenario;

    public void SetQuests(List<string> questsToStart)
    {
        var questManager = GameDataService.Current.QuestsManager;
        
        Quests = new List<QuestEntity>();
        foreach (var id in questsToStart)
        {
            var quest = questManager.InstantiateQuest(id);
            Quests.Add(quest);
        }
    }
    
    public void BuildConversation(string starterId)
    {
        switch (starterId)
        {
             case "1" : CreateStartScenario1(); break;
             case "2" : CreateStartScenario2(); break;
             case "3" : CreateStartScenario3(); break;
             // case "4" : CreateStartScenario4(); break;
             // case "5" : CreateStartScenario5(); break;
             // case "6" : CreateStartScenario6(); break;
             // case "7" : CreateStartScenario7(); break;
             // case "8" : CreateStartScenario8(); break;
             
             default : CreateAbstractStartScenario(); break;
        }
    }

    public void BuildTestConversation()
    {
        string char1Id = UiCharacterData.CharSleepingBeauty;
        string char2Id = UiCharacterData.CharRapunzel;
        string char3Id = UiCharacterData.CharGnomeWorker;
        string char4Id = UiCharacterData.CharPussInBoots;
        
        Scenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.RightOuter, char4Id},
                {CharacterPosition.RightInner, char3Id},
                {CharacterPosition.LeftOuter,  char2Id},
                {CharacterPosition.LeftInner,  char1Id},
            }
        };

        Scenario.RegisterComponent(charsList);
        
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                CharacterId = char1Id,
                Message = "The only thing we remember is a great noise while we cutting down trees. Then terrible fog appeared everywhere, then we run, then we hid..."
            }
        });
               
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                CharacterId = char3Id,
                Message = "Fog scared Gnomes? It's hard to believe this."
            }
        });
        
        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                CharacterId = char2Id,
                Message = "We really need your help right now. Let's clear here everything and clean up."
            }
        };
        actBubble.RegisterComponent(new ConversationActionPayloadShowQuestComponent
        {
            QuestIds = this.Quests.Select(e => e.Id).ToList()
        });
        Scenario.RegisterComponent(actBubble);
        
        Scenario.RegisterComponent(new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                CharacterId = char4Id,
                Message = "We've sweared that we'll help with any requests to the person who saves us. So thank you one more time."
            }
        });
    }
    
    public void BuildQuestCompletedConversation()
    {
        string char1Id = UiCharacterData.CharSleepingBeauty;
        
        var charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  char1Id},
            }
        };

        Scenario = new ConversationScenarioEntity();
        Scenario.RegisterComponent(charsList);

        var actBubble = new ConversationActionBubbleEntity
        {
            BubbleId = R.UICharacterBubbleQuestCompletedView,
            BubbleDef = new UiCharacterBubbleDefQuestCompleted
            {
                CharacterId = char1Id,
                Message = "I knew you would help me. I am indebted to you.\nOnly you can save the Kingdom!",
                QuestId = "1",
                AllowTeleType = false
            }
        };
        actBubble.RegisterComponent(new ConversationActionPayloadStartNewQuestsIfAnyComponent());
        
        Scenario.RegisterComponent(actBubble);
        
        // var externalAction = new ConversationActionExternalActionEntity();
        // externalAction.RegisterComponent(new ConversationActionPayloadStartNewQuestsIfAnyComponent());
        // Scenario.RegisterComponent(externalAction); 
    }

    private ConversationActionBubbleEntity CreateStartBubble(string charId, string message, CharacterEmotion emotion = CharacterEmotion.Normal, List<string> questIds = null)
    {
        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleEntity
        {
            BubbleDef = new UiCharacterBubbleDefMessage
            {
                Emotion = emotion,
                CharacterId = charId,
                Message = message
            }
        };

        if (questIds != null)
        {
            actBubble.RegisterComponent(new ConversationActionPayloadShowQuestComponent
            {
                QuestIds = questIds
            });
        }

        return actBubble;
    }
 
    private void CreateAbstractStartScenario()
    {
        Scenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightOuter, UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharRapunzel},
                // {CharacterPosition.LeftOuter,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.LeftInner,  UiCharacterData.CharPussInBoots},
            }
        };

        Scenario.RegisterComponent(charsList);

        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "[New cool quest start dialog here!]",
            CharacterEmotion.Normal,
            Quests.Select(e => e.Id).ToList()
        ));
    }
    
    private void CreateStartScenario1()
    {
        Scenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightOuter, UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharRapunzel},
                // {CharacterPosition.LeftOuter,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.LeftInner,  UiCharacterData.CharPussInBoots},
            }
        };
        
        Scenario.RegisterComponent(charsList);

        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "What's that noise? Who's here?",
            CharacterEmotion.Sleep1
        ));
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Hey! A new guest, at last! I've been waiting for you for so long. Apparently I fell asleep.",
            CharacterEmotion.Sleep2
        ));

        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Oh, pardon me... I'm not dressed to meet such an honored guest! I'll be right back...",
            CharacterEmotion.Sleep2
        ));        
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "We'll grow a candy tree when I get back.",
            CharacterEmotion.Sleep1,
            Quests.Select(e => e.Id).ToList()
        ));
    }
    
    private void CreateStartScenario2()
    {
        Scenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightOuter, UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharRapunzel},
                // {CharacterPosition.LeftOuter,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.LeftInner,  UiCharacterData.CharPussInBoots},
            }
        };
        
        Scenario.RegisterComponent(charsList);

        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "I've never seen anyone solve this puzzle so fast. You're talented!",
            CharacterEmotion.Normal
        ));
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Oh, sorry, I almost forgot... ",
            CharacterEmotion.Normal
        ));

        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Welcome to the Faitytale Kingdom! I'm Aurora, but some call me Sleeing Beauty. I'll be your guide on this magic journey.",
            CharacterEmotion.Normal
        ));           
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "I can't wait to start making some magic. Let's stop wasting time.",
            CharacterEmotion.Happy
        ));        
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Where did this annoying fog come from? It's ruining my garden! Let's see if we can remove it...",
            CharacterEmotion.Thinking,
            Quests.Select(e => e.Id).ToList()
        ));
    }
    
    private void CreateStartScenario3()
    {
        Scenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.RightOuter, UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharRapunzel},
                // {CharacterPosition.LeftOuter,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.LeftInner,  UiCharacterData.CharPussInBoots},
            }
        };

        Scenario.RegisterComponent(charsList);
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "Thanks a lot! You're our saviour! Do you know that A Gnome's greatest fear is fog? ",
            CharacterEmotion.Happy
        ));
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "What had hapened? Where is my fairytale candy garden? What should I show to our Guest?",
            CharacterEmotion.Shocked
        ));

        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "The only thing we remember is a great noise while we cutting down trees. Then terrible fog appeared everywhere, then we run, then we hid...",
            CharacterEmotion.Normal
        ));           
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Fog scared Gnomes? It's hard to believe this.",
            CharacterEmotion.Thinking
        ));     
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "But it's true. We've sweared that we'll help with any requests to the person who saves us. So thank you one more time.",
            CharacterEmotion.Happy
        ));  
        
        Scenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "We really need your help right now. Let's clear here everything and clean up.",
            CharacterEmotion.Normal,
            Quests.Select(e => e.Id).ToList()
        ));
    }

}
