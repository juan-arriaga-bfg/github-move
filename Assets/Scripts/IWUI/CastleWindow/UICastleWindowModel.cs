using System.Collections.Generic;

public class UICastleWindowModel : IWWindowModel 
{
    public int ChestReward = -1;
    
    public string Title => "Shop of Chests";
    public string Message => "Upgrade your castle to get new chests!";
    public string Button => "Show";

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
                
                return next == PieceType.None.Id && chest != ignore && chest != PieceType.Basket3.Id;
            });
            
            last.Add(GameDataService.Current.ChestsManager.Chests.Find(def => PieceType.Parse(def.Uid) == current));
            
            return last;
        }
    }

    public FreeChestLogicComponent FreeChestLogic => BoardService.Current.GetBoardById(0).FreeChestLogic;

    public bool Spawn()
    {
        if (ChestReward == -1) return false;
        
        var board = BoardService.Current.GetBoardById(0);
        var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
        var piece = board.BoardLogic.GetPieceAt(position);
        var reaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);

        var menu = reaction?.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
        var spawn = menu?.GetDefinition<TouchReactionDefinitionSpawnShop>();
        
        if(spawn == null) return false;

        spawn.Reward = ChestReward;
        ChestReward = -1;
        FreeChestLogic.Timer.Start();
        return spawn.Make(piece.CachedPosition, piece);
    }
}
