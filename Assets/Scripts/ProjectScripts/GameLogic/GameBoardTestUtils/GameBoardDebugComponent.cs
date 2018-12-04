using UnityEngine;

/// <summary>
/// GameBoardDebugComponent used for unit tests and debug operations with field
/// </summary>
public class GameBoardDebugComponent : ECSEntity, IECSSystem 
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid
    {
        get { return ComponentGuid; }
    }

    private BoardController context;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
        
        base.OnRegisterEntity(entity);
    }

    public bool IsExecuteable()
    {
        return true;
    }

    public void Execute()
    {
        if (Input.GetKey(KeyCode.D) && Input.GetKeyUp(KeyCode.C))
        {
            ClearField();
        }
        
        if (Input.GetKey(KeyCode.D) && Input.GetKeyUp(KeyCode.S))
        {
            SpawnAllPieces();
        }
        
        if (Input.GetKey(KeyCode.R) && Input.GetKeyUp(KeyCode.T))
        {
            AddUIViews();
        }
    }

    public object GetDependency()
    {
        return context;
    }

    public virtual void AddUIViews()
    {
        var targetPiece = context.BoardLogic.GetPieceAt(new BoardPosition(19, 11, BoardLayer.Piece.Layer));
        
        if (targetPiece == null) return;
        
        if (targetPiece.ViewDefinition == null) return;

        targetPiece.ViewDefinition.AddView(ViewType.Lock);
    }

    public virtual void ClearField()
    {
        var board = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;
        var boardRender = board.RendererContext;
       
        var action = new ClearBoardAction();
        board.ActionExecutor.AddAction(action, BoardActionMode.SingleMode, 100);
    }
    
    public virtual void SpawnAllPieces()
    {
        var board = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;
        var boardRender = board.RendererContext;
       
        var action = new SpawnAllPiecesAction();
        board.ActionExecutor.AddAction(action, BoardActionMode.SingleMode, -1);
    }
}
