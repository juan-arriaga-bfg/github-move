using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel
{
    private List<int> selectedPieces;
    private List<ChestDef> products;
    
    public int ChestReward = -1;
    
    public string Title
    {
        get { return "Out of Energy"; }
    }
    
    public string Message
    {
        get { return "Need more energy? Make it yourself or buy more."; }
    }
    
    public string SecondMessage
    {
        get { return "Get energy for free from:"; }
    }

    public string ButtonText
    {
        get { return "Show"; }
    }

    public List<ChestDef> Products
    {
        get
        {
            if (products != null) return products;
            
            var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Chest | PieceTypeFilter.Energy);
            products = new List<ChestDef>();
            
            ids.Sort((a, b) => a.CompareTo(b));
            
            foreach (var id in ids)
            {
                var chestDef = GameDataService.Current.ChestsManager.Chests.Find(def => def.Piece == id);
                
                if(chestDef == null) continue;
                
                products.Add(chestDef);
            }
            
            return products;
        }
    }

    public List<int> SelectedPieces
    {
        get { return selectedPieces ?? (selectedPieces = PieceType.GetIdsByFilter(PieceTypeFilter.Energy)); }
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
