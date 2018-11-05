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
    
    [SerializeField] private Transform tapToContinueAnchor;
    
    private Dictionary<string, UICharacterViewController> characters = new Dictionary<string, UICharacterViewController>();
    private UICharacterBubbleView bubbleView;

    private TapToContinueTextViewController tapToContinue;
    
    private UICharacterViewController GetCharacterById(string charId)
    {
        UICharacterViewController characterViewController;
        characters.TryGetValue(charId, out characterViewController);
        return characterViewController;
    }
    
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
        return def.Side == CharacterSide.Left ? bubbleAnchorLeft : bubbleAnchorRight;
    }

    private CharacterSide GetSideByCharacterPosition(CharacterPosition pos)
    {
        switch (pos)
        {
            case CharacterPosition.LeftOuter:
            case CharacterPosition.LeftInner:
                return CharacterSide.Left;
            
            case CharacterPosition.RightInner:
            case CharacterPosition.RightOuter:
                return CharacterSide.Right;
        }
        
        throw new IndexOutOfRangeException();
    }
    
    public UICharacterViewController AddCharacter(string characterId, CharacterPosition position, bool active, bool animated, Action onComplete = null)
    {
        var pool = UIService.Get.PoolContainer;

        string viewName = $"UICharacter{characterId}View";
        
        UICharacterViewController character = pool.Create<UICharacterViewController>(viewName);
        character.CharacterId = characterId;
        
        character.transform.SetParent(GetAnchorForCharacterPosition(position), false);
        character.Side = GetSideByCharacterPosition(position);

        characters.Add(characterId, character);
        
        character.ToggleActive(active, animated, onComplete);

        return character;
    }

    public void RemoveCharacter(int characterId)
    {
        throw new NotImplementedException();
    }

    private void ToggleTapToContinue(bool enabled)
    {
        InitTapToContinue();
        tapToContinue.gameObject.SetActive(enabled);
    }
    
    private void InitTapToContinue()
    {
        if (tapToContinue != null)
        {
            return;
        }
        
        var pool = UIService.Get.PoolContainer;
        tapToContinue = pool.Create<TapToContinueTextViewController>(R.TapToContinueTextView);
        tapToContinue.transform.SetParent(tapToContinueAnchor, false);
        tapToContinue.transform.localPosition = Vector3.zero;
        
        ToggleTapToContinue(false);
    }

    private void SendToBackgroundAllCharacters(bool animated, string excludedCharId)
    {
        foreach (var character in characters.Values)
        {
            if (character.CharacterId != excludedCharId)
            {
                character.ToBackground(animated);
            }
        }
    }
    
    private void SendCharacterToForeground(string charId, bool animated, Action onComplete = null)
    {
        characters[charId].ToForeground(animated, onComplete);
    }

    public void NextBubble(string bubbleId, UICharacterBubbleDef data, Action onComplete)
    {
        ToggleTapToContinue(false);
        
        if (data.Side == CharacterSide.Unknown)
        {
            data.Side = GetCharacterById(data.CharacterId).Side;
        }
        
        var pool = UIService.Get.PoolContainer;
        
        Action showNextBubble = () =>
        {
            string charId = data.CharacterId;
            SendToBackgroundAllCharacters(true, charId);
            SendCharacterToForeground(charId, true, () =>
            {
                UICharacterBubbleView bubble = pool.Create<UICharacterBubbleView>(bubbleId);
                bubbleView = bubble;

                bubbleView.transform.SetParent(GetAnchorForBubblePosition(data), false);
                bubbleView.Show(data, () =>
                {
                    ToggleTapToContinue(true);
                    onComplete?.Invoke();
                });
            });
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