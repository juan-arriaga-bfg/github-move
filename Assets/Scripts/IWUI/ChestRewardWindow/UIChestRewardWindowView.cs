using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIChestRewardWindowView : UIGenericWindowView 
{
    [SerializeField] private List<RectTransform> cardAnchors;
    
    [SerializeField] private NSText title;
    [SerializeField] private NSText message;
    
    [SerializeField] private NSText cardHeroLabel;
    [SerializeField] private NSText cardBuildLabel;
    [SerializeField] private NSText cardResourceLabel;
    
    [SerializeField] private NSText cardHeroName;
    [SerializeField] private NSText cardBuildName;
    [SerializeField] private NSText cardResourceName;
    
    [SerializeField] private NSText buildAmountLabel;
    [SerializeField] private NSText heroAmountLabel;
    [SerializeField] private NSText resourceAmountLabel;
    
    [SerializeField] private NSText heroProgressLabel;
    
    [SerializeField] private Image iconOpenTop;
    [SerializeField] private Image iconOpenDown;
    
    [SerializeField] private RectTransform progress;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIChestRewardWindowModel;

        title.Text = windowModel.Title;
        message.Text = windowModel.Message;

        cardHeroLabel.Text = windowModel.CardHeroText;
        cardBuildLabel.Text = windowModel.CardBuildText;
        cardResourceLabel.Text = windowModel.CardResourceText;
        
        var id = windowModel.Chest.GetSkin();
        
        iconOpenTop.sprite = IconService.Current.GetSpriteById(id + "_3");
        iconOpenDown.sprite = IconService.Current.GetSpriteById(id + "_1");

        var resCard = windowModel.Chest.Rewards.Find(pair => pair.Currency == Currency.Coins.Name);

        cardResourceName.Text = resCard.Currency;
        resourceAmountLabel.Text = resCard.Amount.ToString();
        
        var heroCard = windowModel.Chest.Rewards.Find(pair => pair.Currency == Currency.RobinCards.Name);

        cardHeroName.Text = heroCard.Currency.Replace("Cards", "");
        heroAmountLabel.Text = "x" + heroCard.Amount;
        
        heroProgressLabel.Text = string.Format("{0}/{1}", heroCard.Amount, 50);
        progress.sizeDelta = new Vector2(150*(heroCard.Amount/50f), progress.sizeDelta.y);
        
        var index = 2 + Random.Range(0, 2);
        var amount = windowModel.Chest.Rewards[2 + Random.Range(0, 2)].Amount;
        var pieceCard = windowModel.Chest.Rewards[index];
        
        cardBuildName.Text = pieceCard.Currency.Replace("Piece", "")[0] == 'A' ? "House" : "Tower";
        buildAmountLabel.Text = "x" + amount;
    }
    
    public override void AnimateShow()
    {
        base.AnimateShow();

        for (var i = 0; i < cardAnchors.Count; i++)
        {
            var anchor = cardAnchors[i];
            var position = new Vector2(anchor.anchoredPosition.x, 130f);
            
            DOTween.Kill(anchor);
            
            anchor.anchoredPosition = new Vector2(position.x, -Screen.height);
            var sequence = DOTween.Sequence().SetId(anchor);
            sequence.Append(anchor.DOAnchorPos(position, (i * 0.1f) + 0.5f).SetEase(Ease.OutBack));
        }
    }

    public override void AnimateClose()
    {
        base.AnimateClose();

        foreach (var anchor in cardAnchors)
        {
            DOTween.Kill(anchor);
            var sequence = DOTween.Sequence().SetId(anchor);
            sequence.Append(anchor.DOAnchorPos(new Vector2(anchor.anchoredPosition.x, -Screen.height), 0.5f).SetEase(Ease.InBack));
        }
    }
}