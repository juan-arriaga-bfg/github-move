public class HighlightTaskCompleteOrderCharacter : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var orders = GameDataService.Current.OrdersManager.Orders;
        if (orders == null || orders.Count == 0)
        {
            return false;
        }

        var order = orders[UnityEngine.Random.Range(0, orders.Count)];
        var board = BoardService.Current.FirstBoard;
        var position = board.BoardLogic.PositionsCache.GetRandomPositions(order.Customer, 1)[0];
        // var character = board.BoardLogic.GetPieceAt(position);

        HintArrowView.Show(position, 0, 0.5f);
        
        return true;
    }
}