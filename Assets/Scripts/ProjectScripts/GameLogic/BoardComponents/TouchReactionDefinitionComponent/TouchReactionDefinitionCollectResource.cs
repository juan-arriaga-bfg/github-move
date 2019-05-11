using System.Collections.Generic;
using BfgAnalytics;

public class TouchReactionDefinitionCollectResource : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<ResourceStorageComponent>(ResourceStorageComponent.ComponentGuid);
        var isPiece = storage?.Resources == null;
        var resources = isPiece ? new CurrencyPair {Currency = PieceType.Parse(piece.PieceType), Amount = 1} : storage.Resources;

        if (isPiece)
        {
            CurrencyHelper.Purchase(resources);
            piece.Context.BoardLogic.PieceFlyer.FlyToTarget(piece, position, Currency.Order.Name);
            GameDataService.Current.OrdersManager.UpdateOrders();

            NSAudioService.Current.Play(SoundId.GetIngredients);
        }
        else
        {
            AddResourceView.Show(position, resources, -0.3f);
            Analytics.SendPurchase("board_main", $"item{piece.IndexInChain}", null, new List<CurrencyPair>{resources}, false, false);

            if (resources.Currency == Currency.Coins.Name || 
                resources.Currency == Currency.Crystals.Name ||
                resources.Currency == Currency.Token.Name)
            {
                NSAudioService.Current.Play(SoundId.GetToken);
            }
        }
        
        piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position},
            AnimationResourceSearch = pieceType => AnimationOverrideDataService.Current.FindAnimation(pieceType, def => def.OnDestroyFromBoard)
        });
        
        return true;
    }
}