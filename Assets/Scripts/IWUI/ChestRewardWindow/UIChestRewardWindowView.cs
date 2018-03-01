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
    
    [SerializeField] private Image iconPiece;
    
    [SerializeField] private RectTransform progress;

    private int random;
    
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
        var hero = GameDataService.Current.GetHero("Robin");
        var price = hero.Prices[GameDataService.Current.HeroLevel].Amount;
        
        cardHeroName.Text = heroCard.Currency.Replace("Cards", "");
        heroAmountLabel.Text = "x" + heroCard.Amount;
        
        heroProgressLabel.Text = string.Format("{0}/{1}", heroCard.Amount, price);
        progress.sizeDelta = new Vector2(Mathf.Clamp(150*(heroCard.Amount/(float)price), 0, 150), progress.sizeDelta.y);
        
        random = 2 + Random.Range(0, 2);
        
        var pieceCard = windowModel.Chest.Rewards[random];
        var piece = pieceCard.Currency.Replace("Piece", "");
        
        cardBuildName.Text = piece[0] == 'A' ? "House" : "Tower";
        buildAmountLabel.Text = "x" + pieceCard.Amount;
        
        iconPiece.sprite = IconService.Current.GetSpriteById(piece);
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
        
        var windowModel = Model as UIChestRewardWindowModel;
        var chest = windowModel.Chest;

        foreach (var reward in chest.Rewards)
        {
            if (reward.Currency != Currency.Coins.Name && reward.Currency != Currency.RobinCards.Name)
            {
                var board = BoardService.Current.GetBoardById(0);
        
                var free = new List<BoardPosition>();
        
                board.BoardLogic.EmptyCellsFinder.FindAllWithPointInHome(new BoardPosition(8, 8, board.BoardDef.PieceLayer), 15, 15, free);
        
                if(free.Count == 0) continue;
                
                board.ActionExecutor.AddAction(new SpawnPieceAtAction
                {
                    At = free[Random.Range(0, free.Count)],
                    PieceTypeId = PieceType.Parse(reward.Currency.Replace("Piece", ""))
                });
                
                continue;
            }
            
            var shopItem = new ShopItem
            {
                Uid = string.Format("purchase.test.{0}.10", reward.Currency), 
                ItemUid = reward.Currency, 
                Amount = reward.Amount,
                CurrentPrices = new List<Price>
                {
                    new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}
                }
            };
        
            ShopService.Current.PurchaseItem
            (
                shopItem,
                (item, s) =>
                {
                    // on purchase success
                },
                item =>
                {
                    // on purchase failed (not enough cash)
                }
            );
        }
        
        foreach (var anchor in cardAnchors)
        {
            DOTween.Kill(anchor);
            var sequence = DOTween.Sequence().SetId(anchor);
            sequence.Append(anchor.DOAnchorPos(new Vector2(anchor.anchoredPosition.x, -Screen.height), 0.5f).SetEase(Ease.InBack));
        }
    }
}