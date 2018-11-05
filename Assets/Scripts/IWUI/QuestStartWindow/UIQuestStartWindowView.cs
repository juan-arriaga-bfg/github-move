using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class UIQuestStartWindowView : IWUIWindowView
{
    [SerializeField] private GameObject questCardPrefab;
    [SerializeField] private Transform characterConversationAnchor;
    
    private readonly List<UIQuestCard> questCards = new List<UIQuestCard>();
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);

        WarmUpPool();
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        CreateQuestCards(windowModel);
        CreateConversation(windowModel);
    }

    private void CreateQuestCards(UIQuestStartWindowModel model)
    {
        var pool = UIService.Get.PoolContainer;
        
        // Cleanup
        foreach (var card in questCards)
        {
            pool.Return(card.gameObject);
        }
        questCards.Clear();

        if (model.Quests != null)
        {
            // Create cards
            foreach (var quest in model.Quests)
            {
                UIQuestCard card = pool.Create<UIQuestCard>(questCardPrefab);
                card.gameObject.SetActive(true);
                card.transform.SetParent(questCardPrefab.transform.parent, false);
                card.Init(quest);

                questCards.Add(card);
            }
        }
    }

    public override void OnViewClose()
    {
        StartQuests();
        
        base.OnViewClose();
        
        UIQuestStartWindowModel windowModel = Model as UIQuestStartWindowModel;
        
    }

    private void StartQuests()
    {
        List<string> ids = (Model as UIQuestStartWindowModel).Quests.Select(e => e.Id).ToList();
        GameDataService.Current.QuestsManager.StartQuests(ids);
    }

    private void WarmUpPool()
    {
        var pool = UIService.Get.PoolContainer;

        for (int i = 1; i <= 3; i++)
        {
            UIQuestCard tabInstance = pool.Create<UIQuestCard>(questCardPrefab);
            pool.Return(tabInstance.gameObject);
        }
        
        questCardPrefab.SetActive(false);
    }

    private void CreateConversation(UIQuestStartWindowModel windowModel)
    {
        UICharactersConversationViewController conversation = UIService.Get.GetCachedObject<UICharactersConversationViewController>(R.UICharacterConversationView);
        conversation.transform.SetParent(characterConversationAnchor, false);

        string char1Id = UiCharacterData.CharSleepingBeauty;
        string char2Id = UiCharacterData.CharRapunzel;
        string char3Id = UiCharacterData.CharGnomeWorker;
        string char4Id = UiCharacterData.CharPussInBoots;
        
        conversation.AddCharacter(char4Id, CharacterPosition.RightOuter, false, false);
        conversation.AddCharacter(char3Id, CharacterPosition.RightInner, false, false);
        conversation.AddCharacter(char2Id, CharacterPosition.LeftOuter,  false, false);
        conversation.AddCharacter(char1Id, CharacterPosition.LeftInner,  false, false);


        ConversationScenario scenario = new ConversationScenario();
        scenario.RegisterComponent(new ConversationActionBubble
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char1Id,
                Message = "The only thing we remember is a great noise while we cutting down trees. Then terrible fog appeared everywhere, then we run, then we hid..."
            }
        });
        scenario.RegisterComponent(new ConversationActionBubble
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char2Id,
                Message = "2222"
            }
        });
        scenario.RegisterComponent(new ConversationActionBubble
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char3Id,
                Message = "33333"
            }
        });
        scenario.RegisterComponent(new ConversationActionBubble
        {
            Def = new UiCharacterBubbleDefMessage
            {
                CharacterId = char4Id,
                Message = "444444"
            }
        });
            
        
        conversation.PlayScenario(scenario);
    }
}

public class ConversationAction : IECSComponent, IECSSerializeable
{    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public  int Guid => ComponentGuid;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
}

public class ConversationActionBubble : ConversationAction
{
    public string BubbleId = R.UICharacterBubbleMessageView;
    public UiCharacterBubbleDefMessage Def;
}

public class ConversationScenario : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
   
    private readonly List<ConversationAction> actions = new List<ConversationAction>();

    private int index;

    public override ECSEntity RegisterComponent(IECSComponent component, bool isCollection = false)
    {
        return base.RegisterComponent(component, isCollection);
        
        ConversationAction action = component as ConversationAction;
        if (action != null)
        {
            actions.Add(action);
        }
    }

    public ConversationAction GetNextAction()
    {
        if (actions == null || index >= actions.Count - 1)
        {
            return null;
        }

        var ret = actions[index];
        index++;

        return ret;
    }
}