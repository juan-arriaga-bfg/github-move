using System.Collections.Generic;

public class CastleUpgradeComponent : IECSComponent, IPieceBoardObserver, IBoardEventListener
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid { get { return ComponentGuid; } }
    
    private Piece thisContext;
    private ViewDefinitionComponent viewDef;
    public List<Quest> Prices = new List<Quest>();
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        thisContext = entity as Piece;
        
        var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(thisContext.PieceType);

        var isLast = thisContext.Context.BoardLogic.MatchDefinition.GetNext(thisContext.PieceType) == PieceType.None.Id;

        if (def == null || isLast) return;
        
        var up = def.UpgradePrices;

        foreach (var pair in up)
        {
            Prices.Add(new Quest(new QuestDef{Price = pair}));
        }
        
        viewDef = thisContext.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        thisContext.Context.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        thisContext.Context.BoardEvents.RemoveListener(this, GameEventsCodes.CreatePiece);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.CreatePiece) return;

        var piece = (int)context;
        
        foreach (var quest in Prices)
        {
            if(piece != quest.WantedPiece) continue;
            
            quest.CurrentAmount++;
        }

        if(Check() == false) return;
        
        thisContext.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);

        var view = viewDef.AddView(ViewType.CastleUpgrade);
        
        view.Change(true);
    }

    public bool Check()
    {
        foreach (var quest in Prices)
        {
            if(quest.Check()) continue;
            
            return false;
        }

        return true;
    }
}