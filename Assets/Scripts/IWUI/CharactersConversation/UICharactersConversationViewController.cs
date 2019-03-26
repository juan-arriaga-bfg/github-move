using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public partial class UICharactersConversationViewController : IWUIWindowView
{
    [SerializeField] private List<UICharacterAnchor> characterAnchors;
    
    [SerializeField] private Transform bubbleAnchorLeft;
    [SerializeField] private Transform bubbleAnchorRight;
    
    [SerializeField] private Transform tapToContinueAnchor;

    public Transform GetLeftBubbleAnchor()
    {
        return bubbleAnchorLeft;
    }
    
    private ConversationScenarioEntity scenario;

    private bool isFirstBubbleShowed;
    
    private Action onScenarioComplete;
    private Action<ConversationActionEntity> onActionStarted;
    private Action<ConversationActionEntity> onActionEnded;
    private ConversationActionEntity activeAction;
    
    private readonly Dictionary<string, UICharacterViewController> characters = new Dictionary<string, UICharacterViewController>();
    private readonly Dictionary<CharacterPosition, UICharacterViewController> characterPositions = new Dictionary<CharacterPosition, UICharacterViewController>();
    
    private UICharacterBubbleView bubbleView;

    private TapToContinueTextViewController tapToContinue;

    private void CleanUp(bool preserveCharacters)
    {
        isFirstBubbleShowed = false;
        
        var pool = UIService.Get.PoolContainer;

        if (!preserveCharacters)
        {
            foreach (var character in characters.Values)
            {
                pool.Return(character.gameObject);
            }

            characters.Clear();
            characterPositions.Clear();
        }

        bubbleView = null;
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
    
    private Transform GetAnchorForBubblePosition(ConversationActionBubbleEntity data)
    {
        return data.Side == CharacterSide.Left ? bubbleAnchorLeft : bubbleAnchorRight;
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
    
    private void AddCharactersFromScenario(ConversationScenarioEntity scenario, Action onComplete)
    {
        ConversationScenarioCharacterListComponent characterList = scenario.GetComponent<ConversationScenarioCharacterListComponent>(ConversationScenarioCharacterListComponent.ComponentGuid);

        if (scenario.Continuation)
        {
            // Remove chars that not a part of new scenario
            List<string> charsToRemove = characters.Keys.ToList();
            for (var i = charsToRemove.Count - 1; i >= 0; i--)
            {
                var character = charsToRemove[i];
                if (characterList.ConversationCharacters.Values.Contains(character))
                {
                    charsToRemove.RemoveAt(i);
                }
            }

            foreach (var character in charsToRemove)
            {
                RemoveCharacter(character, false);
            }
        }

        foreach (var pair in characterList.ConversationCharacters)
        {
            if (GetCharacterById(pair.Value) != null)
            {
                continue;
            }

            InitCharacter(pair.Value, pair.Key, false);
         }

        if (!scenario.Continuation && characterList.ConversationCharacters.Count == 1)
        {
            SpawnCharacters(true, onComplete);
        }
        else
        {
            SpawnCharacters(false, onComplete);
        }
    }

    private void SpawnCharacters(bool animated, Action onComplete)
    {
        if (animated && (scenario.Continuation || characters.Count != 1))
        {
            Debug.LogError("[UICharactersConversationViewController] => SpawnCharactersAnimated: supported only for ONE char in scenario and not for chained scenario");
            onComplete();
            return;
        }

        ConversationActionEntity action = scenario.GetFirstAction();
        ConversationActionBubbleEntity bubbleAction = action as ConversationActionBubbleEntity;

        var character = characters[bubbleAction.CharacterId];
        character.ToForeground(false, bubbleAction.Emotion);

        if (animated)
        {
            float dx = 170;
            float moveTime = 0.4f;
            float fadeTime = 0.35f;
            float totalTime = 0.1f;

            var initialPos = character.transform.localPosition;
            var newPos = initialPos;
            newPos.x -= dx;
            character.transform.localPosition = newPos;

            var canvasGroup = character.GetCanvasGroup();
            canvasGroup.alpha = 0;

            canvasGroup.DOFade(1, fadeTime);

            character.transform.DOLocalMoveX(initialPos.x, moveTime)
                      .SetEase(Ease.OutBack)
                     // .SetEase(Ease.OutSine)
                     ;

            DOTween.Sequence()
                   .InsertCallback(totalTime, () =>
                    {
                        onComplete();
                    });
        }
        else
        {
            onComplete();
        }
    }

    public void InitCharacter(string characterId, CharacterPosition position, bool active)
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
        
        character.ToggleActive(active, CharacterEmotion.Normal, false);
    }

    public void RemoveCharacter(string characterId, bool animated)
    {
        var pool = UIService.Get.PoolContainer;

        var character = characters.Values.FirstOrDefault(e => e.CharacterId == characterId);
        if (character == null)
        {
            return;
        }

        character.ToBackground(animated, character.Emotion, () =>
        {
            pool.Return(character.gameObject);
        });

        var pos = GetPositionForCharacter(character);
        characters.Remove(characterId);
        characterPositions.Remove(pos);
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
        else
        {
            tapToContinue.Hide(false);  
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
    
    private void NextBubble(ConversationActionBubbleEntity data, Action onComplete)
    {
        ToggleTapToContinue(false);
        
        if (data.Side == CharacterSide.Unknown)
        {
            data.Side = GetCharacterById(data.CharacterId).Side;
        }

        if (bubbleView != null)
        {
            HideBubble(() =>
            {
                ReorderCharsAndShowBubble(data, onComplete);
            });
        }
        else
        {
            ReorderCharsAndShowBubble(data, onComplete);
        }
    }

    private void HideBubble(Action onComplete)
    {
        if (bubbleView == null)
        {
            onComplete?.Invoke();
            return;
        }
        
        //var pool = UIService.Get.PoolContainer;
        bubbleView.Hide(true, () =>
        {
            bubbleView = null;
            
            // do not return bubble to the pool here!
            onComplete?.Invoke();
        });
    }

    private void ReorderCharsAndShowBubble(ConversationActionBubbleEntity data, Action onComplete)
    {
        bool animated = isFirstBubbleShowed;
        
        string charId = data.CharacterId;
        SendToBackgroundAllCharacters(animated, charId);
        SendCharacterToForeground(charId, data.Emotion, animated, () =>
        {
            var pool = UIService.Get.PoolContainer;
            UICharacterBubbleView bubble = pool.Create<UICharacterBubbleView>(data.BubbleView);
            bubbleView = bubble;

            bubbleView.transform.SetParent(GetAnchorForBubblePosition(data), false);
            bubbleView.Show(data, () =>
            {
                isFirstBubbleShowed = true;
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
        CleanUp(scenario.Continuation);
        
        this.scenario = scenario;
        this.onScenarioComplete = onScenarioComplete;
        this.onActionStarted = onActionStarted;
        this.onActionEnded = onActionEnded;

        AddCharactersFromScenario(scenario, () =>
        {
            NextScenarioAction(); 
        });
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
            HideBubble(() =>
            {
                onScenarioComplete?.Invoke();
            });
            return;
        }
        
        if (activeAction is ConversationActionBubbleEntity)
        {
            PerformActionBubble();
            return;
        }
        
        if (activeAction is ConversationActionExternalActionEntity)
        {
            PerformActionExternal();
            return;
        }

        Debug.LogError($"[UICharactersConversationViewController] => Unknown action type: {activeAction.GetType()} ");
        NextScenarioAction();
    }

    private void PerformActionExternal()
    {
        HideBubble(() =>
        {
            onActionStarted?.Invoke(activeAction);
            onActionEnded?.Invoke(activeAction);
        });
    }

    private void PerformActionBubble()
    {
        ConversationActionBubbleEntity act = activeAction as ConversationActionBubbleEntity;
        NextBubble(act, () => { onActionEnded?.Invoke(act); });

        onActionStarted?.Invoke(act);
    }

    public void OnClick()
    {
        Debug.Log("[UICharactersConversationViewController] => OnClick");

        var teletype = bubbleView as ITeleTypedText;
        if (teletype != null && teletype.IsPlayingTeleTypeEffect())
        {
            teletype.StopTeleTypeEffect();
            return;
        }   
        
        ToggleTapToContinue(false);
        
        NextScenarioAction();
    }
}