using System.Collections.Generic;

public class UIEnergyShopWindowModel : IWWindowModel
{
    private List<int> selectedPieces;
    private List<ChestDef> products;
    
    public int ChestReward = -1;
    
    public string Title => "Out of Energy";

    public string Message => "Need more energy? Make it yourself or buy more.";

    public string SecondMessage => "Get energy for free from:";

    public string ButtonText => "Show";

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

    public List<int> SelectedPieces => selectedPieces ?? (selectedPieces = PieceType.GetIdsByFilter(PieceTypeFilter.Energy));

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
        return spawn.Make(piece.CachedPosition, piece);
    }
}
