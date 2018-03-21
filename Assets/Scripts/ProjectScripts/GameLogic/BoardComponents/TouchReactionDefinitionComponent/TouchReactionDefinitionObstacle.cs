﻿using System.Collections.Generic;
using UnityEngine;

public class TouchReactionDefinitionObstacle : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var obstacle = piece.Context.ObstaclesLogic.GetObstacle(position);
        
        if (obstacle == null) return false;
        
        switch (obstacle.State)
        {
            case ObstacleState.None:
                return false;
            case ObstacleState.Lock:
                UIMessageWindowController.CreateDefaultMessage("This Obstacle Lock!");
                break;
            case ObstacleState.Unlock:
                var model = UIService.Get.GetCachedModel<UIQuestStartWindowModel>(UIWindowType.QuestStartWindow);

                model.Obstacle = obstacle;
        
                UIService.Get.ShowWindow(UIWindowType.QuestStartWindow);
                break;
            case ObstacleState.InProgres:
                var modelMessage = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
                modelMessage.Title = "Message";
                modelMessage.Message = string.Format("Are you sure you want to spend {0} crystals to Speed Up the quest?", obstacle.Def.Price.Amount);
                modelMessage.AcceptLabel = "Ok";
                modelMessage.CancelLabel = "Cancel";

                modelMessage.OnCancel = () => {};
                modelMessage.OnAccept = () =>
                {
                    var shopItem = new ShopItem
                    {
                        Uid = string.Format("purchase.test.{0}.10", obstacle.GetUid()), 
                        ItemUid = Currency.Quest.Name, 
                        Amount = 1,
                        CurrentPrices = new List<Price>
                        {
                            new Price{Currency = obstacle.Def.Price.Currency, DefaultPriceAmount = obstacle.Def.Price.Amount}
                        }
                    };
        
                    ShopService.Current.PurchaseItem
                    (
                        shopItem,
                        (item, s) =>
                        {
                            // on purchase success
                            obstacle.State = ObstacleState.Open;
                        },
                        item =>
                        {
                            // on purchase failed (not enough cash)
                            UIMessageWindowController.CreateDefaultMessage("Not enough crystals!");
                        }
                    );
                };
        
                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
                break;
            case ObstacleState.Open:
                obstacle.State = ObstacleState.Finish;
                piece.Context.ObstaclesLogic.RemoveObstacle(position);

                var heroes = GameDataService.Current.HeroesManager.Heroes;

                foreach (var hero in heroes)
                {
                    if(hero.InAdventure != obstacle.GetUid()) continue;

                    hero.InAdventure = -1;
                }
                
                piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                {
                    To = position,
                    Positions = new List<BoardPosition>{position},
                    OnComplete = () =>
                    {
                        piece.Context.ActionExecutor.AddAction(new SpawnPiecesAction()
                        {
                            IsCheckMatch = false,
                            At = position,
                            Pieces = new List<int>{obstacle.GetReward().Piece}
                        });
                    }
                });
                break;
            case ObstacleState.Finish:
                return false;
        }
        
        return true;
    }
}