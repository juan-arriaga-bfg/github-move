using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class UIQuestCompleteWindowView : IWUIWindowView
{
    [SerializeField] private Transform characterAnchor;
    [SerializeField] private Transform bubbleAnchor;
    [SerializeField] private Transform tapToContinueAnchor;
    
    [SerializeField] private CanvasGroup rootCanvasGroup;

    private GameObject characterGo;
    private GameObject bubbleGo;
    private TapToContinueTextViewController tapToContinue;
    
    private bool isClickAllowed;

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var model = Model as UIQuestCompleteWindowModel;
        
        InitCharacter(UiCharacterData.CharSleepingBeauty);
        InitBubble();
        InitTapToContinue(3f);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var model = Model as UIQuestCompleteWindowModel;
    }
    
    public void InitCharacter(string characterId)
    {
        var pool = UIService.Get.PoolContainer;

        if (characterGo != null)
        {
            pool.Return(characterGo);
        }
        
        string viewName = $"UICharacter{characterId}View";
        
        UICharacterViewController character = pool.Create<UICharacterViewController>(viewName);
        character.CharacterId = characterId;

        character.transform.SetParent(characterAnchor, false);

        character.Side = CharacterSide.Left;

        characterGo = character.gameObject;
        
        character.ToggleActive(true, true, null);
    }

    public void InitBubble()
    {      
        var pool = UIService.Get.PoolContainer;

        if (bubbleGo != null)
        {
            pool.Return(bubbleGo);
        }
        
        var model = Model as UIQuestCompleteWindowModel;

        var bubbleId = R.UICharacterBubbleQuestCompletedView;
        var bubbleDef = new UiCharacterBubbleDefQuestCompleted
        {
            Message = "Well done! Let's continue work and build up a house.",
            QuestId = model.Quest.Id,
            AllowTeleType = false,
            CharacterId = UiCharacterData.CharSleepingBeauty,
            Side = CharacterSide.Left
        };
        
        UICharacterBubbleView bubble = pool.Create<UICharacterBubbleView>(bubbleId);
        bubbleGo = bubble.gameObject;
        
        bubble.transform.SetParent(bubbleAnchor, false);
        bubble.Show(bubbleDef, () =>
        {
            isClickAllowed = true;
        });
    }

    public void OnClick()
    {
        if (!isClickAllowed)
        {
            return;
        }

        ProvideRewardAndClose();
    }

    private void ProvideRewardAndClose()
    {
        isClickAllowed = false;

        AnimateHide(() =>
        {
            var model = Model as UIQuestCompleteWindowModel;
            var quest = model.Quest;

            quest.SetClaimedState();
            GameDataService.Current.QuestsManager.CompleteQuest(quest.Id);

            List<CurrencyPair> reward = model.Quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

            CurrencyHellper.Purchase(reward, success =>
            {
                if (success == false) return;

                Controller.CloseCurrentWindow();
            },
            new Vector2(Screen.width/2f, Screen.height/2f));
        });
    }

    private void AnimateHide(Action onComplete)
    {
        rootCanvasGroup.DOFade(0, 0.5f)
                       .OnComplete(() =>
                        {
                            onComplete?.Invoke();
                        });
    }
    
    private void InitTapToContinue(float delay)
    {
        if (tapToContinue == null)
        {
            var pool = UIService.Get.PoolContainer;
            tapToContinue = pool.Create<TapToContinueTextViewController>(R.TapToContinueTextView);
            tapToContinue.transform.SetParent(tapToContinueAnchor, false);
            tapToContinue.transform.localPosition = Vector3.zero;
        }

        tapToContinue.Hide(false);
        tapToContinue.Show(true, delay);
    }
}
