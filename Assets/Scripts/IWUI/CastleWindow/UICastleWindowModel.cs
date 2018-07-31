using System.Collections.Generic;

public class UICastleWindowModel : IWWindowModel 
{
    public int ChestReward = -1;
    
    public string Title
    {
        get { return "Shop of Chests"; }
    }
    
    public string Message
    {
        get { return "Upgrade your castle to get new chests!"; }
    }
    
    public string UpgradeMessage
    {
        get
        {
            var board = BoardService.Current.GetBoardById(0);
            var piece = board.BoardLogic.GetPieceAt(GameDataService.Current.PiecesManager.CastlePosition);
            var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);
            
            return string.Format("Upgrade\n{0}", def.UpgradePrices[0].ToStringIcon(false));
        }
    }
    
    public List<ChestDef> Chests
    {
        get
        {
            var board = BoardService.Current.GetBoardById(0);
            var piece = board.BoardLogic.GetPieceAt(GameDataService.Current.PiecesManager.CastlePosition);
            var definition = board.BoardLogic.MatchDefinition;
            
            var current = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType).SpawnPieceType;
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
    
    public StorageComponent Storage
    {
        get
        {
            var board = BoardService.Current.GetBoardById(0);
            var piece = board.BoardLogic.GetPieceAt(GameDataService.Current.PiecesManager.CastlePosition);
            var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
            return storage;
        }
    }
    
    public bool Upgrade()
    {
        var board = BoardService.Current.GetBoardById(0);
        var piece = board.BoardLogic.GetPieceAt(GameDataService.Current.PiecesManager.CastlePosition);
        var reaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
        if(reaction == null) return false;
        
        var menu = reaction.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
        
        if(menu == null) return false;

        var upgrade = menu.GetDefinition<TouchReactionDefinitionUpgrade>();
        
        if(upgrade == null) return false;
        
        return upgrade.Make(piece.CachedPosition, piece);
    }
    
    public bool Spawn()
    {
        if (ChestReward == -1) return false;
        
        var board = BoardService.Current.GetBoardById(0);
        var piece = board.BoardLogic.GetPieceAt(GameDataService.Current.PiecesManager.CastlePosition);
        var reaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
        if(reaction == null) return false;
        
        var menu = reaction.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
        
        if(menu == null) return false;

        var spawn = menu.GetDefinition<TouchReactionDefinitionSpawnCastle>();
        
        if(spawn == null) return false;

        spawn.Reward = ChestReward;
        ChestReward = -1;
        return spawn.Make(piece.CachedPosition, piece);
    }
}
