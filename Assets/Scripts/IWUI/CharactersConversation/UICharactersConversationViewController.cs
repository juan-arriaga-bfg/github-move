using System;
using System.Collections.Generic;
using UnityEngine;

public partial class UICharactersConversationViewController
{
    private interface IScenarioActionExecutor
    {
        void Perform(ScenarioAction action);
    }
    
    private class ScenarioActionExecutorBubble : IScenarioActionExecutor
    {
        public void Perform(ScenarioAction action)
        {
            throw new NotImplementedException();
        }
    }
}

public partial class UICharactersConversationViewController : IWUIWindowView
{
    [SerializeField] private List<UICharacterAnchor> characterAnchors;
    
    [SerializeField] private Transform bubbleAnchorLeft;
    [SerializeField] private Transform bubbleAnchorRight;
    
    private List<UICharacterViewController> characters = new List<UICharacterViewController>();
    private UICharacterBubbleView bubbleView;
    
    private Transform GetAnchorForCharacterPosition(CharacterPosition pos)
    {
        foreach (var item in characterAnchors)
        {
            if (item.Position == pos)
            {
                return item.Anchor;
            }
        }

        return null;
    }
    
    private Transform GetAnchorForBubblePosition(UICharacterBubbleDef def)
    {
        return def.Side == CharacterBubbleSide.Left ? bubbleAnchorLeft : bubbleAnchorRight;
    }
    
    public UICharacterViewController AddCharacter(string characterId, CharacterPosition position, bool active, bool animated, Action onComplete = null)
    {
        var pool = UIService.Get.PoolContainer;

        UICharacterViewController character = pool.Create<UICharacterViewController>(characterId);
        character.transform.SetParent(GetAnchorForCharacterPosition(position), false);

        characters.Add(character);
        
        character.ToggleActive(active, animated, onComplete);

        return character;
    }

    public void RemoveCharacter(int characterId)
    {
        throw new NotImplementedException();
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
    }

    public void NextBubble(string bubbleId, UICharacterBubbleDef data, Action onComplete)
    {
        var pool = UIService.Get.PoolContainer;
        
        Action showNextBubble = () =>
        {
            UICharacterBubbleView bubble = pool.Create<UICharacterBubbleView>(bubbleId);
            bubbleView = bubble;

            bubbleView.transform.SetParent(GetAnchorForBubblePosition(data), false);
            bubbleView.Show(data, onComplete);
        };
        
        if (bubbleView != null)
        {
            bubbleView.Hide(true, () =>
            {
                pool.Return(bubbleView.gameObject);
                showNextBubble();
            });
            
            return;
        }

        showNextBubble();
    }
}