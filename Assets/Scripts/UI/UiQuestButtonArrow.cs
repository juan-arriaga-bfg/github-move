using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quests;
using UnityEngine;
using UnityEngine.UI;

public class UiQuestButtonArrow : MonoBehaviour
{
    [SerializeField] private Transform host;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private NSText label;
    [SerializeField] private Image arrow;
    [SerializeField] private Sprite newSprite;
    [SerializeField] private Sprite progressSprite;

    private QuestEntity quest;

    private const string QUEUE_ACTION_ID = "UiQuestButtonArrowAction";

    public void SetQuest(QuestEntity quest)
    {
        this.quest = quest;

        if (this.quest != null)
        {
            var action = new QueueActionComponent {Id = QUEUE_ACTION_ID}
                        .AddCondition(new OpenedWindowsQueueConditionComponent {IgnoredWindows = UIWindowType.IgnoredWindows})
                        .SetAction(() => { AnimateNewArrow(); });

            ProfileService.Current.QueueComponent.AddAction(action, true);
            
            AddListner();
        }
        else
        {
            RemoveListner();
        }
        
        DOTween.Kill(this);
        canvasGroup.alpha = 0;
    }
    
    private void OnDisable()
    {
        RemoveListner();
        DOTween.Kill(this);
        
        ProfileService.Current.QueueComponent.RemoveAction(QUEUE_ACTION_ID);
    }

    private void OnEnable()
    {
        AddListner();
    }
    
    private void AddListner()
    {
        if (quest != null)
        {
            quest.OnChanged += OnQuestChanged;
        }
    }

    private void RemoveListner()
    {
        if (quest != null)
        {
            quest.OnChanged -= OnQuestChanged;
        }
    }

    private void OnQuestChanged(QuestEntity quest, TaskEntity task)
    {
        if (!quest.IsInProgress())
        {
            return;
        }
        
        TaskCounterEntity taskCounter = task as TaskCounterEntity;
        if (taskCounter == null)
        {
            return;
        }

        int current = taskCounter.CurrentValue;
        int target  = taskCounter.TargetValue;

        AnimateProgressArrow(current, target);
    }

    private void AnimateProgressArrow(int current, int target)
    {
        arrow.sprite = newSprite;
        label.Text = $"Progress";
        MovementAnimation();
    }

    private void AnimateNewArrow()
    {
        arrow.sprite = progressSprite;
        label.Text = $"New task";
        MovementAnimation();
    }

    private void MovementAnimation()
    {
        DOTween.Kill(this);

        float fadeTime = 0.3f;
        
        float deltaMoveX = 15f;
        float moveTime = 0.35f;
        int loops = 3;

        canvasGroup.DOFade(1, fadeTime)
                   .SetId(this);
        
        DOTween.Sequence()
               .SetId(this)
               .Append(host.transform.DOLocalMoveX(deltaMoveX, moveTime).SetRelative(true).SetEase(Ease.OutQuad).SetId(this))
               .Append(host.transform.DOLocalMoveX(-deltaMoveX, moveTime).SetRelative(true).SetEase(Ease.InOutQuad).SetId(this))
               .SetLoops(3);

        DOTween.Sequence()
               .SetId(this)
               .AppendInterval(loops * 2 * moveTime - fadeTime)
               .Append(canvasGroup.DOFade(0, fadeTime));
    }
}
