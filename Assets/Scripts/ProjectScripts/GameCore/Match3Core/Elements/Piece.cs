using Newtonsoft.Json;

public class Piece : ECSEntity, IBoardStatesComponent, IPieceActorView, IMatchablePiece, IDraggablePiece, IMulticellularPiece, IBoardConditionComponent,
    IPieceStateComponent, IViewDefinitionComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected PieceBoardElementView actorView;

    protected BoardPosition cachedPosition;

    public BoardPosition CachedPosition
    {
        get { return cachedPosition; }
        set { cachedPosition = value; }
    }

    private LayerPieceComponent layer;
    public virtual LayerPieceComponent Layer => layer ?? (layer = GetComponent<LayerPieceComponent>(LayerPieceComponent.ComponentGuid));
    
    private BoardStatesComponent states;
    public virtual BoardStatesComponent States => states ?? (states = GetComponent<BoardStatesComponent>(BoardStatesComponent.ComponentGuid));
    
    private MatchablePieceComponent matchable;
    public MatchablePieceComponent Matchable => matchable ?? (matchable = GetComponent<MatchablePieceComponent>(MatchablePieceComponent.ComponentGuid));
    
    private DraggablePieceComponent draggable;
    public DraggablePieceComponent Draggable => draggable ?? (draggable = GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid));
    
    private MulticellularPieceBoardObserver multicellular;
    public MulticellularPieceBoardObserver Multicellular => multicellular ?? (multicellular = GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid));

    private BoardConditionComponent boardCondition;
    public BoardConditionComponent BoardCondition => boardCondition ?? (boardCondition = GetComponent<BoardConditionComponent>(BoardConditionComponent.ComponentGuid));
    
    private PieceStateComponent pieceState;
    public PieceStateComponent PieceState => pieceState ?? (pieceState = GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid));
    
    private ViewDefinitionComponent viewDefinition;
    public ViewDefinitionComponent ViewDefinition => viewDefinition ?? (viewDefinition = GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid));
    
    public PieceBoardElementView ActorView
    {
        get { return actorView; }
        set { actorView = value; }
    }
    
    public int PieceType { get; set; }

    [JsonIgnore]
    public BoardController Context { get; set; }

    public Piece(int pieceType, BoardController context)
    {
        PieceType = pieceType;
        Context = context;
    }

    public virtual bool IsValidPieceAt(BoardPosition piecePosition)
    {
        var currentPiece = Context.BoardLogic.GetPieceAt(piecePosition);
        return currentPiece == this;
    }

    
}