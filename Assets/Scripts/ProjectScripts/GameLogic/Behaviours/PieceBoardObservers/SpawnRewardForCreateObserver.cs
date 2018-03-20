using UnityEngine;

public class SpawnRewardForCreateObserver : IECSComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid
    {
        get { return ComponentGuid; }
    }

    private Piece contextPiece;
    private PieceBoardObserversComponent observer;

    private CurrencyPair reward;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
        
        if (contextPiece == null) return;
        
        var def = GameDataService.Current.PiecesManager.GetPieceDef(contextPiece.PieceType);
        
        if (def == null || def.CreateReward == null)
        {
            contextPiece.UnRegisterComponent(this);
            return;
        }
        
        reward = def.CreateReward;
        
        observer = contextPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
        
        if (observer != null)
        {
            observer.RegisterObserver(this);
        }
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        if (contextPiece == null) return;
        
        if (observer != null)
        {
            observer.UnRegisterObserver(this);
        }
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if (contextPiece == null) return;

        CreateResource(contextPiece.Context, position, reward);
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }
    
    private bool CreateResource(BoardController gameBoard, BoardPosition position, CurrencyPair resources)
    {
        if (resources == null)
        {
            return false;
        }
        
        var view = gameBoard.RendererContext.CreateBoardElementAt<AddResourceView>(R.AddResourceView, new BoardPosition(100,100,3));
        view.CachedTransform.position = gameBoard.BoardDef.GetSectorCenterWorldPosition(position.X, position.Y, position.Z) + Vector3.up;
        view.AddResource(resources, view.CachedTransform.position + Vector3.up);
		
        return true;
    }
}