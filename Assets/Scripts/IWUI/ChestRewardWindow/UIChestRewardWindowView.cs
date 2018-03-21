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
    [SerializeField] private Image iconHero;
    
    [SerializeField] private RectTransform progress;
    
    public override void OnViewShow()
    {
        base.OnViewShow();

        foreach (var card in cardAnchors)
        {
            card.gameObject.SetActive(false);
        }
        
        var windowModel = Model as UIChestRewardWindowModel;

        title.Text = windowModel.Title;
        message.Text = windowModel.Message;

        cardHeroLabel.Text = windowModel.CardHeroText;
        cardBuildLabel.Text = windowModel.CardBuildText;
        cardResourceLabel.Text = windowModel.CardResourceText;
        
        var id = windowModel.Chest.GetSkin();
        
        iconOpenTop.sprite = IconService.Current.GetSpriteById(id + "_2");
        iconOpenDown.sprite = IconService.Current.GetSpriteById(id + "_1");

        var reward = windowModel.GetReward();
        
        var hero = GameDataService.Current.HeroesManager.GetHeroByCurrency(reward.Currency);

        if (hero != null)
        {
            cardHeroName.Text = hero.Def.Uid;
            heroAmountLabel.Text = "x" + reward.Amount;
        
            heroProgressLabel.Text = string.Format("{0}/{1}", reward.Amount, hero.TotalProgress);
            progress.sizeDelta = new Vector2(Mathf.Clamp(150*(reward.Amount/(float)hero.TotalProgress), 0, 150), progress.sizeDelta.y);
            iconHero.sprite = IconService.Current.GetSpriteById("face_" + hero.Def.Uid);
            cardAnchors[0].gameObject.SetActive(true);
            return;
        }

        var piece = PieceType.Parse(reward.Currency);

        if (piece != PieceType.None.Id)
        {
            cardBuildName.Text = "House";
            buildAmountLabel.Text = "x" + reward.Amount;
        
            iconPiece.sprite = IconService.Current.GetSpriteById(PieceType.Parse(piece));
            cardAnchors[1].gameObject.SetActive(true);
            return;
        }
        
        cardResourceName.Text = reward.Currency;
        resourceAmountLabel.Text = reward.Amount.ToString();
        cardAnchors[2].gameObject.SetActive(true);
    }
    
    public override void AnimateShow()
    {
        base.AnimateShow();

        for (var i = 0; i < cardAnchors.Count; i++)
        {
            var anchor = cardAnchors[i];
            
            if(anchor.gameObject.activeSelf == false) continue;
            
            var position = new Vector2(0, 130f);
            
            DOTween.Kill(anchor);
            
            anchor.anchoredPosition = new Vector2(0, -Screen.height);
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

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        var windowModel = Model as UIChestRewardWindowModel;
        
        var reward = windowModel.GetReward();
        var board = BoardService.Current.GetBoardById(0);
        
        var hero = GameDataService.Current.HeroesManager.GetHeroByCurrency(reward.Currency);
        
        if (reward.Currency != Currency.Coins.Name && hero == null)
        {
            var pieces = new List<int>();

            for (int i = 0; i < reward.Amount; i++)
            {
                pieces.Add(PieceType.Parse(reward.Currency));
            }
                
            board.ActionExecutor.AddAction(new SpawnPiecesAction
            {
                IsCheckMatch = false,
                At = hero.HousePosition,
                Pieces = pieces
            });
            
            return;
        }

        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", reward.Currency),
            ItemUid = reward.Currency,
            Amount = reward.Amount,
            CurrentPrices = new List<Price>
            {
                new Price {Currency = Currency.Cash.Name, DefaultPriceAmount = 0}
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
        
        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(hero.HousePosition.X, hero.HousePosition.Up.Y, hero.HousePosition.Z);
        
        board.Manipulator.CameraManipulator.ZoomTo(0.3f, worldPos);
    }
}