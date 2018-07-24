using System.Collections.Generic;

public class UIProductionWindowModel : IWWindowModel 
{
    public List<ProductionComponent> Productions
    {
        get
        {
            var board = BoardService.Current.GetBoardById(0);

            return board == null ? new List<ProductionComponent>() : board.ProductionLogic.Productions;
        }
    }
}
