using System.Collections.Generic;

public class TouchReactionDefinitionCollectResource : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<ResourceStorageComponent>(ResourceStorageComponent.ComponentGuid);
        var isPiece = storage?.Resources == null;
        var resources = isPiece ? new CurrencyPair {Currency = PieceType.Parse(piece.PieceType), Amount = 1} : storage.Resources;

        if (isPiece)
        {
            CurrencyHellper.Purchase(resources);
            piece.Context.BoardLogic.PieceFlyer.FlyToTarget(piece, position, Currency.Order.Name);
            GameDataService.Current.OrdersManager.UpdateOrders();
        }
        else AddResourceView.Show(position, resources);
        
        piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position}
        });
        
        return true;
    }
}