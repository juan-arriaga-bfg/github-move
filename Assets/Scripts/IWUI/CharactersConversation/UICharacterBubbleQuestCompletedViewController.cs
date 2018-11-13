using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;

public class UICharacterBubbleQuestCompletedViewController : UICharacterBubbleMessageViewController
{
    [SerializeField] private UiQuestButton questButton;
    [SerializeField] private NSText rewardLabel;
    [SerializeField] private IWTextMeshAnimation textMeshAnimation;
    
    public override void Show(UICharacterBubbleDef def, Action onComplete)
    {
        base.Show(def, onComplete);
        
        UiCharacterBubbleDefQuestCompleted data = def as UiCharacterBubbleDefQuestCompleted;

        var quest = GameDataService.Current.QuestsManager.GetActiveQuestById(data.QuestId);
        questButton.Init(quest, false);
        questButton.ToggleCheckMark(true);

        rewardLabel.Text = GetRewardText(quest);
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

        var bubbleScale = bubbleHost.transform.localScale;
        bubbleScale.x = 0;
        bubbleHost.transform.localScale = bubbleScale;
        
        var rewardScale = rewardLabel.transform.localScale;
        rewardScale = new Vector3(0, 1.3f, 1);
        rewardLabel.transform.localScale = rewardScale;
        
        textMeshAnimation.gameObject.SetActive(false);
        
        DOTween.Sequence()
               .InsertCallback(0.5f,    () => { canvasGroup.DOFade(1, 0.4f); })
                
               .InsertCallback(0.5f,    () => { bubbleHost.transform.DOScale(Vector3.one, 0.5f)
                                                                    .SetEase(Ease.OutBack); })
                
               .InsertCallback(1.0f,    () => { textMeshAnimation.gameObject.SetActive(true);
                                                textMeshAnimation.Animate();})
                
               .InsertCallback(1.3f,    () => { rewardLabel.transform.DOScale(Vector3.one, 0.5f)
                                                                     .SetEase(Ease.OutBack);});

    }
}