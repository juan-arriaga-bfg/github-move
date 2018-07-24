using Newtonsoft.Json;

public class Piece : ECSEntity, IBoardStatesComponent, IPieceActorView, IMatchablePiece
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    protected PieceBoardElementView actorView;

    protected BoardPosition cachedPosition;

    public BoardPosition CachedPosition
    {
        get { return cachedPosition; }
        set { cachedPosition = value; }
    }

    private LayerPieceComponent layer;
    public virtual LayerPieceComponent Layer
    {
        get
        {
            if (layer == null)
            {
                layer = GetComponent<LayerPieceComponent>(LayerPieceComponent.ComponentGuid);
            }
            
            return layer;
        }
    }

    private BoardStatesComponent states;
    public virtual BoardStatesComponent States
    {
        get
        {
            if (states == null)
            {
                states = GetComponent<BoardStatesComponent>(BoardStatesComponent.ComponentGuid);
            }
            return states;
        }
    }

    private MatchablePieceComponent matchable;
    public MatchablePieceComponent Matchable
    {
        get
        {
            if (matchable == null)
            {
                matchable = GetComponent<MatchablePieceComponent>(MatchablePieceComponent.ComponentGuid);
            }
            return matchable;
        }
    }
    
    public PieceBoardElementView ActorView
    {
        get
        {
            return actorView;
        }
        set
        {
            actorView = value;
        }
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
        if (currentPiece != this)
        {
            return false;
        }

        return true;
    }
}
