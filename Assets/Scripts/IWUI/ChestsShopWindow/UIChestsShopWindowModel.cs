using System.Collections.Generic;

public class UIChestsShopWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Instance.Manager.GetTextByUid("window.shop.chest.title", "Shop of Chests");
    public string Message => LocalizationService.Instance.Manager.GetTextByUid("window.shop.chest.message", "Upgrade your castle to get new chests!");
    public string Button => LocalizationService.Instance.Manager.GetTextByUid("window.shop.chest.button", "Show");
    
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