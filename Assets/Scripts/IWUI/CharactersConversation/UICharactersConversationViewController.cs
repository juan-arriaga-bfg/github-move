using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class UICharactersConversationViewController : IWUIWindowView
{
    [SerializeField] private List<UICharacterAnchor> characterAnchors;
    
    [SerializeField] private Transform bubbleAnchorLeft;
    [SerializeField] private Transform bubbleAnchorRight;
    
    [SerializeField] private Transform tapToContinueAnchor;

    private ConversationScenarioEntity scenario;

    private Action onScenarioComplete;
    private Action<ConversationActionEntity> onActionStarted;
    private Action<ConversationActionEntity> onActionEnded;
    private ConversationActionEntity activeAction;
    
    private readonly Dictionary<string, UICharacterViewController> characters = new Dictionary<string, UICharacterViewController>();
    private readonly Dictionary<CharacterPosition, UICharacterViewController> characterPositions = new Dictionary<CharacterPosition, UICharacterViewController>();
    
    private UICharacterBubbleView bubbleView;

    private TapToContinueTextViewController tapToContinue;

    private void CleanUp()
    {
        var pool = UIService.Get.PoolContainer;
        
        foreach (var character in characters.Values)
        {
            pool.Return(character.gameObject);
        }
        
        characters.Clear();
        characterPositions.Clear();

        if (bubbleView != null)
        {
            bubbleView.Hide(false, null);
        }
    }
    
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
    
    private Vector3 GetAnchorCoordinatesForCharacterPosition(CharacterPosition pos)
    {
        var anchor = GetAnchorForCharacterPosition(pos);
        return anchor.position;
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
    
    private void AddCharactersFromScenario(ConversationScenarioEntity scenario)
    {
        ConversationScenarioCharsListComponent charsList = scenario.GetComponent<ConversationScenarioCharsListComponent>(ConversationScenarioCharsListComponent.ComponentGuid);
        foreach (var pair in charsList.Characters)
        {
            AddCharacter(pair.Value, pair.Key, false, false);
        }
    }
    
    public void AddCharacter(string characterId, CharacterPosition position, bool active, bool animated, Action onComplete = null)
    {
        var pool = UIService.Get.PoolContainer;

        string viewName = $"UICharacter{characterId}View";
        
        UICharacterViewController character = pool.Create<UICharacterViewController>(viewName);
        character.CharacterId = characterId;
       
        Transform anchor = GetAnchorForCharacterPosition(position);
        Transform host = anchor.parent;
        
        character.transform.SetParent(host, false);
        character.transform.position = anchor.position;
                  
        character.Side = GetSideByCharacterPosition(position);

        characters.Add(characterId, character);
        characterPositions.Add(position, character);
        
        character.ToggleActive(active, CharacterEmotion.Normal, animated, onComplete);
    }

    public void RemoveCharacter(int characterId)
    {
        throw new NotImplementedException();
    }

    private void ToggleTapToContinue(bool enabled)
    {
        InitTapToContinue();
        tapToContinue.gameObject.SetActive(enabled);

        // Show with some delay
        if (enabled)
        {
            tapToContinue.Hide(false);
            tapToContinue.Show(true, 1f);
        }
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
                character.ToBackground(animated, character.Emotion);
            }
        }
    }
    
    private void SendCharacterToForeground(string charId, CharacterEmotion emotion, bool animated, Action onComplete = null)
    {
        do
        {
            var character = characters[charId];       
            character.ToForeground(animated, emotion, null);
            
            if (IsCharacterAtFront(character))
            {
                break;
            }

            UICharacterViewController neighbor = GetNeighborFor(character);
            if (neighbor == null)
            {
                break;
            }

            character.transform.SetAsLastSibling();
            CharacterPosition charToFrontPos = GetPositionForCharacter(character);
            CharacterPosition charToBackPos  = GetPositionForCharacter(neighbor);

            characterPositions[charToBackPos] = character;
            characterPositions[charToFrontPos] = neighbor;

            Vector3 frontCoords = GetAnchorCoordinatesForCharacterPosition(charToBackPos);
            Vector3 backCoords  = GetAnchorCoordinatesForCharacterPosition(charToFrontPos);

            if (!animated)
            {
                character.transform.position = frontCoords;
                neighbor.transform.position = backCoords;
                break;
            }

            const float TIME = UICharacterViewController.FADE_TIME;

            character.transform.DOMove(frontCoords, TIME)
                     .SetEase(Ease.OutSine);

            neighbor.transform.DOMove(backCoords, TIME)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() =>
                     {
                         onComplete?.Invoke();
                     });
            return;

        } while (false);
        
        onComplete?.Invoke();
    }

    private CharacterPosition GetPositionForCharacter(UICharacterViewController character)
    {
        foreach (var pair in characterPositions)
        {
            if (pair.Value == character)
            {
                return pair.Key;
            }
        }

        return CharacterPosition.Unknown;
    }
    
    private UICharacterViewController GetCharacterForPosition(CharacterPosition pos)
    {
        UICharacterViewController character;
        characterPositions.TryGetValue(pos, out character);
        return character;
    }
    
    private UICharacterViewController GetNeighborFor(UICharacterViewController character)
    {
        CharacterPosition pos = GetPositionForCharacter(character);
        
        CharacterPosition neighborPos = CharacterPosition.Unknown;
        
        if (pos == CharacterPosition.LeftInner) { neighborPos = CharacterPosition.LeftOuter;}
        if (pos == CharacterPosition.LeftOuter) { neighborPos = CharacterPosition.LeftInner;}        
        if (pos == CharacterPosition.RightInner) { neighborPos = CharacterPosition.RightOuter;}
        if (pos == CharacterPosition.RightOuter) { neighborPos = CharacterPosition.RightInner;}

        UICharacterViewController neighbor = GetCharacterForPosition(neighborPos);
        return neighbor;
    }

    private bool IsCharacterAtFront(UICharacterViewController character)
    {
        CharacterPosition pos = GetPositionForCharacter(character);
        return pos == CharacterPosition.LeftInner || pos == CharacterPosition.RightInner;
    }
    
    private void NextBubble(string bubbleId, UICharacterBubbleDef data, Action onComplete)
    {
        ToggleTapToContinue(false);
        
        if (data.Side == CharacterSide.Unknown)
        {
            data.Side = GetCharacterById(data.CharacterId).Side;
        }

        if (bubbleView != null)
        {
            var pool = UIService.Get.PoolContainer;
            bubbleView.Hide(true, () =>
            {
                pool.Return(bubbleView.gameObject);
                ReorderCharsAndShowBubble(bubbleId, data, onComplete);
            });
        }
        else
        {
            ReorderCharsAndShowBubble(bubbleId, data, onComplete);
        }
    }

    private void ReorderCharsAndShowBubble(string bubbleId, UICharacterBubbleDef data, Action onComplete)
    {
        string charId = data.CharacterId;
        SendToBackgroundAllCharacters(true, charId);
        SendCharacterToForeground(charId, data.Emotion, true, () =>
        {
            var pool = UIService.Get.PoolContainer;
            UICharacterBubbleView bubble = pool.Create<UICharacterBubbleView>(bubbleId);
            bubbleView = bubble;

            bubbleView.transform.SetParent(GetAnchorForBubblePosition(data), false);
            bubbleView.Show(data, () =>
            {
                ToggleTapToContinue(true);
                onComplete?.Invoke();
            });
        });
    }
    
    public void PlayScenario(ConversationScenarioEntity scenario, 
                             Action<ConversationActionEntity> onActionStarted, 
                             Action<ConversationActionEntity> onActionEnded, 
                             Action onScenarioComplete)
    {
        CleanUp();
        
        this.scenario = scenario;
        this.onScenarioComplete = onScenarioComplete;
        this.onActionStarted = onActionStarted;
        this.onActionEnded = onActionEnded;

        AddCharactersFromScenario(scenario);
        
        NextScenarioAction();
    }

    public void NextScenarioAction()
    {
        if (activeAction != null)
        {
            onActionStarted?.Invoke(activeAction);
        }
        
        activeAction = scenario.GetNextAction();

        if (activeAction == null)
        {
            onScenarioComplete?.Invoke();
            return;
        }
        
        if (activeAction is ConversationActionBubbleEntity)
        {
            ConversationActionBubbleEntity act = activeAction as ConversationActionBubbleEntity;
            NextBubble(act.BubbleId, act.BubbleDef, () =>
            {
                 onActionEnded?.Invoke(act);
            });
            
            onActionStarted?.Invoke(act);
            return;
        }
        else if (activeAction is ConversationActionExternalActionEntity)
        {            
            onActionStarted?.Invoke(activeAction);
            onActionEnded?.Invoke(activeAction);
            return;
        }

        Debug.LogError($"[UICharactersConversationViewController] => Unknown action type: {activeAction.GetType()} ");
        NextScenarioAction();
    }

    public void OnClick()
    {
        NextScenarioAction();
    }
}