using System.Collections.Generic;

public class UIChestsShopWindowModel : IWWindowModel 
{
    public string Title => "Shop of Chests";
    public string Message => "Upgrade your castle to get new chests!";
    public string Button => "Show";
    
    public FreeChestLogicComponent FreeChestLogic => BoardService.Current.GetBoardById(0).FreeChestLogic;
    
    public List<ChestDef> Chests
    {
        get
        {
            var board = BoardService.Current.GetBoardById(0);
            var definition = board.BoardLogic.MatchDefinition;
            
            var current = GameDataService.Current.LevelsManager.Chest;
            var ignore = definition.GetLast(current);
            
            var last = GameDataService.Current.ChestsManager.Chests.FindAll(def =>
            {
                var chest = PieceType.Parse(def.Uid);
                var next = definition.GetNext(chest);
                
                return next == PieceType.None.Id && chest != ignore;
            });
            
            last.Add(GameDataService.Current.ChestsManager.Chests.Find(def => PieceType.Parse(def.Uid) == current));
            
            return last;
        }
    }
}