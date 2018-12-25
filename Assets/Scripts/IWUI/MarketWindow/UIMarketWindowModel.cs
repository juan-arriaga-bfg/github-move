using System.Collections.Generic;

public class UIMarketWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.market.title", "window.market.title");
    public string Message => LocalizationService.Get("window.market.message", "window.market.message");

    public string ButtonReset
    {
        get
        {
            var price = BoardService.Current.FirstBoard.MarketLogic.Timer.GetPrise();
            
            return string.Format(LocalizationService.Get("window.market.button.reset", "window.market.button.reset {0}"), price.ToStringIcon());
        }
    }

    public List<ChestDef> Chests
    {
        get
        {
            var board = BoardService.Current.FirstBoard;
            var definition = board.BoardLogic.MatchDefinition;
            
            var last = GameDataService.Current.ChestsManager.Chests.FindAll(def =>
            {
                var chest = PieceType.Parse(def.Uid);
                var next = definition.GetNext(chest);
                var index = definition.GetIndexInChain(chest);
                
                return index != 1
                       && next == PieceType.None.Id
                       && chest != PieceType.CH_Free.Id
                       && chest != PieceType.CH3_C.Id
                       && chest != PieceType.CH3_D.Id;
            });
            
            last.Add(GameDataService.Current.ChestsManager.Chests.Find(def => PieceType.Parse(def.Uid) == PieceType.CH_Free.Id));
            
            return last;
        }
    }
}