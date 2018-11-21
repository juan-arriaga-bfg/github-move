using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;

public class UICharacterBubbleQuestCompletedViewController : UICharacterBubbleMessageViewController
{
    [SerializeField] private UiQuestButton questButton;
    [SerializeField] private NSText rewardLabel;
    [SerializeField] private IWTextMeshAnimation textMeshAnimation;

    [SerializeField] private Animator animator;
    
    public override void Show(ConversationActionBubbleEntity def, Action onComplete)
    {
        base.Show(def, onComplete);
        
        ConversationActionBubbleQuestCompletedEntity data = def as ConversationActionBubbleQuestCompletedEntity;

        var quest = GameDataService.Current.QuestsManager.GetActiveQuestById(data.QuestId);
        questButton.Init(quest, false);
        questButton.ToggleCheckMark(true);

        rewardLabel.Text = GetRewardText(quest);

        StartCoroutine(AlignMegaHeaderCoroutine());
    }

    private IEnumerator AlignMegaHeaderCoroutine()
    {
        // Wait for layout
        yield return new WaitForEndOfFrame();
        
        RectTransform labelRect = textMeshAnimation.GetComponent<RectTransform>();
        
        RectTransform backRect = back.GetComponent<RectTransform>();
        RectTransform backParentRect = back.transform.parent.GetComponent<RectTransform>();
        
        var backH = backRect.sizeDelta.y;
        
        var screenTop = new Vector2(0, Screen.height);
        
        Vector2 screenTopAtBubbleBackSpace;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(backRect, screenTop, Camera.current, out screenTopAtBubbleBackSpace);
        Vector2 backTop = new Vector2(0, backRect.localPosition.y + backH / 2f);

        float bubbleScreenTop = screenTopAtBubbleBackSpace.y + backRect.localPosition.y;
        float bubbleTopY = backTop.y;
        float finalY = bubbleTopY + (bubbleScreenTop - bubbleTopY) / 2f;

        // labelRect.localPosition = new Vector2(labelRect.localPosition.x, finalY);
        labelRect.localPosition = new Vector2(labelRect.localPosition.x, finalY - backParentRect.localPosition.y);
    }

    private string GetRewardText(QuestEntity quest)
    {
        var reward = quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        if (reward == null)
        {
            return "";
        }

        List<CurrencyPair> currencyRewards;
        var priceRewards = CurrencyHellper.FiltrationRewards(reward, out currencyRewards);
        
        var str = new StringBuilder("<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00></color></font>");
            
        str.Append(CurrencyHellper.RewardsToString("  ", priceRewards, currencyRewards));
        str.Append("</size>");
            
        return str.ToString();
    }
    
    protected override void ShowAnimation(Action onComplete)
    {
        DOTween.Sequence()
               .AppendInterval(0.3f)
               .AppendCallback(() => { onComplete?.Invoke(); });
        
        // textMeshAnimation.gameObject.SetActive(false);

        // var bubbleScale = bubbleHost.transform.localScale;
        // bubbleScale.x = 0;
        // bubbleHost.transform.localScale = bubbleScale;
        //
        // var rewardScale = rewardLabel.transform.localScale;
        // rewardScale = new Vector3(0, 1.3f, 1);
        // rewardLabel.transform.localScale = rewardScale;
        //
        //
        //
        // DOTween.Sequence()
        //        .InsertCallback(0.5f,    () => { canvasGroup.DOFade(1, 0.4f); })
        //         
        //        .InsertCallback(0.5f,    () => { bubbleHost.transform.DOScale(Vector3.one, 0.5f)
        //                                                             .SetEase(Ease.OutBack); })
        //         
        //        .InsertCallback(1.0f,    () => { textMeshAnimation.gameObject.SetActive(true);
        //                                         textMeshAnimation.Animate();})
        //         
        //        .InsertCallback(1.3f,    () => { rewardLabel.transform.DOScale(Vector3.one, 0.5f)
        //                                                              .SetEase(Ease.OutBack);});

        animator.Play("QuestCompleteBubbleShow");
    }

    protected override void HideAnimation(Action onComplete)
    {
        animator.Play("QuestCompleteBubbleHide");
        
        
        var pool = UIService.Get.PoolContainer;
        
        DOTween.Sequence()
               .InsertCallback(0.1f, () =>
                {
                    StopTeleTypeEffect();
                    onComplete?.Invoke();
                })
                .InsertCallback(2f, () =>
                {
                    pool.Return(gameObject);
                })
            ;
    }

    public void ShowCheer()
    {
        // textMeshAnimation.Animate();
    }
}