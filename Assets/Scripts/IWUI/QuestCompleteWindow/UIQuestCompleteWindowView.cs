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

    public override void AnimateShow()
    {
        base.AnimateShow();

        rootCanvasGroup.alpha = 0;

        DOTween.Sequence()
               .AppendInterval(1.5f)
               .AppendCallback(() =>
                {
                    InitCharacter();
                    InitBubble();
                    InitTapToContinue(3f);
                    rootCanvasGroup.DOFade(1, 0.5f);
                });
    }

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var model = Model as UIQuestCompleteWindowModel;

        CleanUp();
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var model = Model as UIQuestCompleteWindowModel;
    }

    public void CleanUp()
    {
        var pool = UIService.Get.PoolContainer;

        if (characterGo != null)
        {
            pool.Return(characterGo);
        }
        
        if (bubbleGo != null)
        {
            pool.Return(bubbleGo);
        }
    }
    
    public void InitCharacter()
    {
        var pool = UIService.Get.PoolContainer;
        
        var model = Model as UIQuestCompleteWindowModel;
               
        string viewName = $"UICharacter{model.CharacterId}View";
        
        UICharacterViewController character = pool.Create<UICharacterViewController>(viewName);
        character.CharacterId = model.CharacterId;
        character.Emotion = model.Emotion;

        character.transform.SetParent(characterAnchor, false);

        character.Side = CharacterSide.Left;

        characterGo = character.gameObject;
        
        character.ToggleActive(true, CharacterEmotion.Normal, true, null);
    }

    public void InitBubble()
    {      
        var pool = UIService.Get.PoolContainer;
       
        var model = Model as UIQuestCompleteWindowModel;

        var bubbleId = R.UICharacterBubbleQuestCompletedView;
        var bubbleDef = new UiCharacterBubbleDefQuestCompleted
        {
            Message = model.Message,
            QuestId = model.Quest.Id,
            AllowTeleType = false,
            CharacterId = model.CharacterId,
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
