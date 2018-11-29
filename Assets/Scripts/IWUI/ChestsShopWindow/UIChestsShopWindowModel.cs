using System.Collections.Generic;

public class UIChestsShopWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.shop.chest.title", "window.shop.chest.title");
    public string Message => LocalizationService.Get("window.shop.chest.message", "window.shop.chest.message");
    public string Button => LocalizationService.Get("window.shop.chest.button", "window.shop.chest.button");
    
    public FreeChestLogicComponent FreeChestLogic => BoardService.Current.GetBoardById(0).FreeChestLogic;
    
    public List<ChestDef> Chests
    {
        get
        {
            var board = BoardService.Current.GetBoardById(0);
            var definition = board.BoardLogic.MatchDefinition;
            
            var last = GameDataService.Current.ChestsManager.Chests.FindAll(def =>
            {
                var chest = PieceType.Parse(def.Uid);
                var next = definition.GetNext(chest);
                var index = definition.GetIndexInChain(chest);
                
                return index != 1 && next == PieceType.None.Id && chest != PieceType.CH_Free.Id;
            });
            
            last.Add(GameDataService.Current.ChestsManager.Chests.Find(def => PieceType.Parse(def.Uid) == PieceType.CH_Free.Id));
            
            return last;
        }
    }
}