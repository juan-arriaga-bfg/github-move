using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UIQuestStartWindowModel : IWWindowModel
{
    public List<QuestEntity> QuestsToStart { get; private set; }
    public ConversationScenarioEntity QuestCompletedScenario { get;  private set; }
    public ConversationScenarioEntity QuestStartScenario { get;  private set; }
    public QuestEntity CompletedQuest { get;  private set; }
    public string StarterId { get; private set; }

    public void Init(QuestEntity completedQuest, List<string> questsToStart, string starterId)
    {
        CompletedQuest = completedQuest;
        StarterId = starterId;
        
        var questManager = GameDataService.Current.QuestsManager;

        if (completedQuest != null)
        {
            BuildQuestCompletedScenario();
        }
        else
        {
            QuestCompletedScenario = null;
        }
        
        if (questsToStart == null)
        {
            QuestsToStart = null;
            QuestStartScenario = null;
        }
        else
        {
            QuestsToStart = new List<QuestEntity>();
            foreach (var id in questsToStart)
            {
                var quest = questManager.InstantiateQuest(id);
                QuestsToStart.Add(quest);
            }

            BuildStartConversation();
        }
    }

    private void BuildStartConversation()
    {
        switch (StarterId)
        {
             case "1" : CreateStartScenario1(); break;
             case "2" : CreateStartScenario2(); break;
             case "3" : CreateStartScenario3(); break;
             // case "4" : CreateStartScenario4(); break;
             // case "5" : CreateStartScenario5(); break;
             case "6" : CreateStartScenario6(); break;
             // case "7" : CreateStartScenario7(); break;
             case "8" : CreateStartScenario8(); break;
             // case "9" : CreateStartScenario9(); break;
             // case "10" : CreateStartScenario10(); break;
             // case "11" : CreateStartScenario11(); break;
             // case "12" : CreateStartScenario12(); break;
             case "13" : CreateStartScenario13(); break;
             // case "14" : CreateStartScenario14(); break;
             // case "15" : CreateStartScenario15(); break;
             
             default : CreateAbstractStartScenario(); break;
        }
    }

    private void GetQuestCompletedData(string questId, out string characterId, out string message, out CharacterEmotion emotion)
    {
        emotion = CharacterEmotion.Happy;
        
        switch (questId)
        {
            case "1" :
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "What a beautiful tree!";
                break;
            
            case "2" :
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "Great! That fog is really annoying.";
                break;
            
            case "3" :
                characterId = UiCharacterData.CharSleepingBeauty;
                emotion = CharacterEmotion.Normal;
                message = "There are much better without those rusted trees.";
                break;
            
            case "4" :
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "Well done! Let's continue work and build up a house.";
                break;
            
            case "5" :
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "It's much better without this fog.";
                break;
            
            case "6" :
                characterId = UiCharacterData.CharGnomeWorker;
                message = "A nice building! It remind us about our native village.";
                break;
            
            case "8" :
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "It seems for now we have enough wheat to start cooking. Great job.";
                break;
            
            case "9" :
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "These apples looks great and a pie will be even better.";
                break;
            
            case "10" :
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "Aw! One time I've heard that candies don't grow on trees. Funny joke, right?";
                break;
            
            case "11" :
                characterId = UiCharacterData.CharGnomeWorker;
                message = "We're chest hunters from now!";
                break;
            
            case "15" :
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "Great! That fog is really annoying.";
                break;

            default:
                characterId = UiCharacterData.CharSleepingBeauty;
                message = "Cool quest completed message here!";
                break;
        }
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
    
    private void BuildQuestCompletedScenario()
    {
        QuestCompletedScenario = new ConversationScenarioEntity();
        
        string characterId;
        string message;
        CharacterEmotion emotion;
        GetQuestCompletedData(CompletedQuest.Id, out characterId, out message, out emotion);

        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  characterId},
            }
        };
        
        QuestCompletedScenario.RegisterComponent(charsList);
        
        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleEntity
        {
            BubbleId = R.UICharacterBubbleQuestCompletedView,
            BubbleDef = new UiCharacterBubbleDefQuestCompleted
            {
                Emotion = emotion,
                CharacterId = characterId,
                Message = message,
                AllowTeleType = false,
                QuestId = CompletedQuest.Id
            }
        };
        QuestCompletedScenario.RegisterComponent(actBubble);
        
        ConversationActionExternalActionEntity extAction = new ConversationActionExternalActionEntity();
        extAction.RegisterComponent(new ConversationActionPayloadProvideRewardComponent());
        QuestCompletedScenario.RegisterComponent(extAction);

    }
    
    private void CreateAbstractStartScenario()
    {
        QuestStartScenario = new ConversationScenarioEntity();

        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
            }
        };
        
        QuestStartScenario.RegisterComponent(charsList);
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "New cool quest start dialog here!",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }
    
    private void CreateStartScenario1()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
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
        
        QuestStartScenario.RegisterComponent(charsList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "What's that noise? Who's here?",
            CharacterEmotion.Sleep1
        ));
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Hey! A new guest, at last! I've been waiting for you for so long. Apparently I fell asleep.",
            CharacterEmotion.Sleep2
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Oh, pardon me... I'm not dressed to meet such an honored guest! I'll be right back...",
            CharacterEmotion.Sleep2
        ));        
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "We'll grow a candy tree when I get back.",
            CharacterEmotion.Sleep1,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }
    
    private void CreateStartScenario2()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
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
        
        QuestStartScenario.RegisterComponent(charsList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "I've never seen anyone solve this puzzle so fast. You're talented!",
            CharacterEmotion.Normal
        ));
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Oh, sorry, I almost forgot... ",
            CharacterEmotion.Normal
        ));
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Welcome to the Faitytale Kingdom! I'm Aurora, but some call me Sleeing Beauty. I'll be your guide on this magic journey.",
            CharacterEmotion.Normal
        ));           
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "I can't wait to start making some magic. Let's stop wasting time.",
            CharacterEmotion.Happy
        ));        
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Where did this annoying fog come from? It's ruining my garden! Let's see if we can remove it...",
            CharacterEmotion.Thinking,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }
    
    private void CreateStartScenario3()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
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

        QuestStartScenario.RegisterComponent(charsList);
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "Thanks a lot! You're our saviour! Do you know that A Gnome's greatest fear is fog? ",
            CharacterEmotion.Happy
        ));
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "What had hapened? Where is my fairytale candy garden? What should I show to our Guest?",
            CharacterEmotion.Shocked
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "The only thing we remember is a great noise while we cutting down trees. Then terrible fog appeared everywhere, then we run, then we hid...",
            CharacterEmotion.Normal
        ));           
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Fog scared Gnomes? It's hard to believe this.",
            CharacterEmotion.Thinking
        ));     
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "But it's true. We've sweared that we'll help with any requests to the person who saves us. So thank you one more time.",
            CharacterEmotion.Happy
        ));  
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "We really need your help right now. Let's clear here everything and clean up.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario6()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
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

        QuestStartScenario.RegisterComponent(charsList);
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Hey, look at this. Someone have left almost finished houses.",
            CharacterEmotion.Happy
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "Let's complete these buildings. There are many suitable resources nearby.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }
    
    private void CreateStartScenario8()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner, UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightOuter, UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharRapunzel},
                // {CharacterPosition.LeftOuter,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.LeftInner,  UiCharacterData.CharPussInBoots},
            }
        };

        QuestStartScenario.RegisterComponent(charsList);
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Hey, look at this field of wheat. And an apple tree. We’re lucky!",
            CharacterEmotion.Happy
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "I think we can cook a great treats using these ingredients. Let’s collect them.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }
    
    private void CreateStartScenario13()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharsListComponent charsList = new ConversationScenarioCharsListComponent
        {
            Characters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner, UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.RightOuter, UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharRapunzel},
                // {CharacterPosition.LeftOuter,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.LeftInner,  UiCharacterData.CharPussInBoots},
            }
        };

        QuestStartScenario.RegisterComponent(charsList);
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "Wow! We've found a chest during wood cutting. You're totally lucky!",
            CharacterEmotion.Happy
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "You can open this chest and take a look at his content.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }
}
