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
             case "1" : 
             case "2" : 
             case "3" : 
                 QuestStartScenario = QuestStartScenario = GameDataService.Current.ConversationsManager.BuildScenario(StarterId);
                 break;
             
             case "4" : CreateStartScenario4(); break;
             case "5" : CreateStartScenario5(); break;
             case "6" : CreateStartScenario6(); break;
             case "7" : CreateStartScenario7(); break;
             case "8" : CreateStartScenario8(); break;
             case "9" : CreateStartScenario9(); break;
             case "10" : CreateStartScenario10(); break;
             case "11" : CreateStartScenario11(); break;
             case "12" : CreateStartScenario12(); break;
             case "13" : 
             case "14" : 
             case "15" : 
                 CreateStartScenario131415(); break;

             default : CreateAbstractStartScenario(); break;
        }
    }

    private void CreateStartScenario12()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "We should add a roof for this building to make a nice and cozy stone house.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario11()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "We've mined enough stones to start building.",
            CharacterEmotion.Normal
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Do you snow a story about The Three Little Pigs? Stone houses are the best ones, don't you?",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario8()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Look at this! It's a stone mine. We should check it out for useful stuff.",
            CharacterEmotion.Normal
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "It seems like we can start to create a stone building for our Kingdom.",
            CharacterEmotion.Happy,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario7()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "We totally like to set in order everything around. Wanna go on.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario131415()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "It looks like we've drained this source of ingredients.",
            CharacterEmotion.Sad
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Don't mind! We're at fairytale lands so can created new fields and trees right from the air. Look around carefully.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario5()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Great! Now we've got all the necessary ingredients to start cooking.",
            CharacterEmotion.Normal
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Don't waste our time. Let's cook some treats.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario9()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharGnomeWorker, 
            "Let's try to make it even better. We need more resources for this.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario6()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "Hey, look at this field of wheat! And an apple tree... We’re lucky!",
            CharacterEmotion.Happy
        ));

        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "I think we can cook a great treats using these ingredients. Let’s collect them.",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }

    private void CreateStartScenario10()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharGnomeWorker},
                // {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

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

    private void CreateStartScenario4()
    {
        QuestStartScenario = new ConversationScenarioEntity();
        
        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
                {CharacterPosition.RightInner, UiCharacterData.CharGnomeWorker},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);

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

    private void GetQuestCompletedData(string questId, out string characterId, out string message, out CharacterEmotion emotion)
    {
        emotion = CharacterEmotion.Happy;
        
        switch (questId)
        {
            case "1_CreatePiece_PR_C4"   : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="I've never seen anyone make it so fast. You're talented!"                    ; break;
            case "3_KillTree"            : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Normal; message="There are much better without those rusted trees."                           ; break;
            case "2_ClearFog"            : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Normal; message="That fog is everywhere, but it don't stop us!"                               ; break;
            case "4_CreatePiece_A2"      : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="Well done! Let's continue work and build up a house."                        ; break;
            case "10_OpenChest"          : characterId = UiCharacterData.CharGnomeWorker;    emotion = CharacterEmotion.Happy;  message="Would be great if we find another chests like this."                         ; break;
            case "5_CreatePieceA5"       : characterId = UiCharacterData.CharGnomeWorker;    emotion = CharacterEmotion.Happy;  message="A nice building! It remind us about our native village."                     ; break;
            case "7_CreatePiece_PR_A5"   : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="It seems for now we have enough wheat to start cooking. Great job."          ; break;
            case "8_CreatePiece_PR_B5"   : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="These apples looks great and a pie will be even better."                     ; break;
            case "9_CreatePiece_PR_C5"   : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="Aw! One time I've heard that candies don't grow on trees. Funny joke, right?"; break;
            case "17_KillField"          : characterId = UiCharacterData.CharGnomeWorker;    emotion = CharacterEmotion.Normal; message="Cool! We can always find some resources after removing a dead field."        ; break;
            case "6_ClearFog"            : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Normal; message="The less fog around the more space for our Kingdom."                         ; break;
            case "11_UseWorker"          : characterId = UiCharacterData.CharGnomeWorker;    emotion = CharacterEmotion.Happy;  message="We've worked so hard and done so much. Like it!"                             ; break;
            case "12_UseMine"            : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="Fairytale mines gives you chests. Adore this feature."                       ; break;
            case "13_CreatePiece_A6"     : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="So nice! I may even move into this house."                                   ; break;
            case "14_CreatePiece_B3"     : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="Gorgeous! Seems like we'll create a whole Kingdom in a couple of hours."     ; break;
            case "15_CreatePiece_B4"     : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="Great house. You're a good architect."                                       ; break;
            case "16_CompleteOrder"      : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message="We'll make together so many sweets. For the whole Kingdom!"                  ; break;
            default                      : characterId = UiCharacterData.CharSleepingBeauty; emotion = CharacterEmotion.Happy;  message = "Cool quest completed message here!"                                        ; break;
        }
    }
    
    private ConversationActionBubbleEntity CreateStartBubble(string charId, string message, CharacterEmotion emotion = CharacterEmotion.Normal, List<string> questIds = null)
    {
        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleEntity()
        {
             BubbleView = R.UICharacterBubbleMessageView,
             Emotion = emotion,
             CharacterId = charId,
             Message = message
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

        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  characterId},
            }
        };
        
        QuestCompletedScenario.RegisterComponent(characterList);
        
        ConversationActionBubbleEntity actBubble = new ConversationActionBubbleQuestCompletedEntity()
        {
            BubbleView = R.UICharacterBubbleQuestCompletedView,
            Emotion = emotion,
            CharacterId = characterId,
            Message = message,
            AllowTeleType = false,
            QuestId = CompletedQuest.Id
        };
        QuestCompletedScenario.RegisterComponent(actBubble);
        
        ConversationActionExternalActionEntity extAction = new ConversationActionExternalActionEntity();
        extAction.RegisterComponent(new ConversationActionPayloadProvideRewardComponent());
        QuestCompletedScenario.RegisterComponent(extAction);

    }
    
    private void CreateAbstractStartScenario()
    {
        QuestStartScenario = new ConversationScenarioEntity();

        ConversationScenarioCharacterListComponent characterList = new ConversationScenarioCharacterListComponent
        {
            ConversationCharacters = new Dictionary<CharacterPosition, string>
            {
                {CharacterPosition.LeftInner,  UiCharacterData.CharSleepingBeauty},
            }
        };
        
        QuestStartScenario.RegisterComponent(characterList);
        
        QuestStartScenario.RegisterComponent(CreateStartBubble(
            UiCharacterData.CharSleepingBeauty, 
            "New cool quest start dialog here!",
            CharacterEmotion.Normal,
            QuestsToStart.Select(e => e.Id).ToList()
        ));
    }
    
    
}
