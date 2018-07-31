using System.Collections.Generic;

public class UIMarketWindowModel : IWWindowModel 
{
    public int SelectIndex { get; set; }
    
    public string Title
    {
        get { return "City Orders"; }
    }
    
    public string Message
    {
        get { return "Peasants from a neighburing village are asked to give them some logs."; }
    }
    
    public Task Selected
    {
        get { return GameDataService.Current.TasksManager.Tasks[SelectIndex]; }
    }
    
    public List<Task> Tasks
    {
        get { return GameDataService.Current.TasksManager.Tasks; }
    }
    
    public bool Upgrade()
    {
        var board = BoardService.Current.GetBoardById(0);
        var piece = board.BoardLogic.GetPieceAt(GameDataService.Current.PiecesManager.KingPosition);
        var reaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
        if(reaction == null) return false;
        
        var menu = reaction.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
        
        if(menu == null) return false;

        var upgrade = menu.GetDefinition<TouchReactionDefinitionUpgrade>();
        
        if(upgrade == null) return false;
        
        return upgrade.Make(piece.CachedPosition, piece);
    }
}