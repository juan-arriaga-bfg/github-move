using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class BoardLogicComponent : ECSEntity,
    IMatchDefinitionComponent, IFieldFinderComponent, IEmptyCellsFinderComponent, IMatchActionBuilderComponent, IPiecePositionsCacheComponent,
    IPieceFlyerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    protected BoardController context;
    
    public BoardController Context
    {
        get { return context; }
    }

    private int width;

    private int height;

    private int depth;

    private int[,,] logicMatrix;

    private Dictionary<BoardPosition, Piece> boardEntities = new Dictionary<BoardPosition, Piece>();
    
    private BoardCell[,,] boardCells;

    protected int[,,] LogicMatrix { get { return logicMatrix; } }

    public Dictionary<BoardPosition, Piece> BoardEntities { get { return boardEntities; } }

    public BoardCell[,,] BoardCells { get { return boardCells; } }

    public virtual int CurrentWidth { get; set; }

    public virtual int CurrentHeight { get; set; }

    public virtual int CurrentOffsetX { get; set; }

    public virtual int CurrentOffsetY { get; set; }

    public virtual int Depth { get { return depth; } }
    
    protected MatchDefinitionComponent matchDefinition;
    public MatchDefinitionComponent MatchDefinition => matchDefinition ?? (matchDefinition = GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid));
    
    protected FieldFinderComponent fieldFinder;
    public FieldFinderComponent FieldFinder => fieldFinder ?? (fieldFinder = GetComponent<FieldFinderComponent>(FieldFinderComponent.ComponentGuid));
    
    protected PiecePositionsCacheComponent positionsCache;
    public PiecePositionsCacheComponent PositionsCache => positionsCache ?? (positionsCache = GetComponent<PiecePositionsCacheComponent>(PiecePositionsCacheComponent.ComponentGuid));
    
    protected EmptyCellsFinderComponent emptyCellsFinder;
    public EmptyCellsFinderComponent EmptyCellsFinder => emptyCellsFinder ?? (emptyCellsFinder = GetComponent<EmptyCellsFinderComponent>(EmptyCellsFinderComponent.ComponentGuid));
    
    protected MatchActionBuilderComponent matchActionBuilder;
    public MatchActionBuilderComponent MatchActionBuilder => matchActionBuilder ?? (matchActionBuilder = GetComponent<MatchActionBuilderComponent>(MatchActionBuilderComponent.ComponentGuid));
    
    protected PieceFlyerComponent pieceFlyer;
    public PieceFlyerComponent PieceFlyer => pieceFlyer ?? (pieceFlyer = GetComponent<PieceFlyerComponent>(PieceFlyerComponent.ComponentGuid));

    public override void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as BoardController;
    }

    public override void OnUnRegisterEntity(ECSEntity entity) { }
    
    public virtual int[,,] GetMatrix()
    {
        return logicMatrix;
    }
    
    public virtual bool IsPointValid(BoardPosition pos)
    {
        return IsPointValid(pos.X, pos.Y);
    }
    
    public virtual bool IsPointValid(int x, int y)
    {
        return IsXValid(x) && IsYValid(y);
    }

    public virtual bool IsXValid(int x, out int near)
    {
        near = Mathf.Clamp(x, 0, width - 1);
        return IsXValid(x);
    }
    
    public virtual bool IsXValid(int x)
    {
        int w = width;
        
        if (x < 0 || x >= w)
        {
            return false;
        }
        
        return true;
    }
    
    public virtual bool IsYValid(int y, out int near)
    {
        near = Mathf.Clamp(y, 0, width - 1);
        return IsXValid(y);
    }

    public virtual bool IsYValid(int y)
    {
        int h = height;
        
        if (y < 0 || y >= h)
        {
            return false;
        }
        
        return true;
    }

    public virtual BoardPosition GetEmptyPointDown(BoardPosition point, BoardLogicComponent matrix, bool isIgnoreLock = false)
    {
        if (matrix.IsEmpty(point))
        {
            bool isEmpty = matrix.IsEmpty(point.Down);
            bool isValid = point.Down.IsValid;
            if (point.Down.IsValid && matrix.IsEmpty(point.Down) && (matrix.IsLockedCell(point.Down) == false || isIgnoreLock))
            {
                return GetEmptyPointDown(point.Down, matrix, isIgnoreLock);
            }
            return point;
        }

        return point;
    }

    public virtual BoardPosition GetEmptyPointUp(BoardPosition point, BoardLogicComponent matrix)
    {
        if (matrix.IsEmpty(point) == false)
        {
            if (point.Up.IsValid && matrix.IsEmpty(point.Up) == false)
            {
                return GetEmptyPointUp(point.Up, matrix);
            }
            else
            {
                return point;
            }
        }

        return point;
    }
    
    public virtual void Init(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;

        this.CurrentWidth = width;
        this.CurrentHeight = height;
        this.CurrentOffsetX = 0;
        this.CurrentOffsetY = 0;

        logicMatrix = new int[width, height, depth];

        boardCells = new BoardCell[width, height, depth];
        for (int d = 0; d < logicMatrix.GetLength(2); d++)
        {
            for (int y = logicMatrix.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < logicMatrix.GetLength(0); x++)
                {
                    boardCells[x, y, d] = new BoardCell();
                }
            }
        }
    }
    
    public virtual void LockCells(List<BoardPosition> boardPositions, object locker)
    {
        for (int i = 0; i < boardPositions.Count; i++)
        {
            LockCell(boardPositions[i], locker);
        }
    }

    public virtual void LockCell(BoardPosition boardPosition, object locker)
    {
        if (IsPointValid(boardPosition) == false) return;

        boardCells[boardPosition.X, boardPosition.Y, boardPosition.Z].Lock(locker);
    }
    
    public virtual void UnlockCells(List<BoardPosition> boardPositions, object locker)
    {
        for (int i = 0; i < boardPositions.Count; i++)
        {
            UnlockCell(boardPositions[i], locker);
        }
    }

    public virtual void UnlockCell(BoardPosition boardPosition, object locker)
    {
        if (IsPointValid(boardPosition) == false) return;

        boardCells[boardPosition.X, boardPosition.Y, boardPosition.Z].Unlock(locker);
    }

    public virtual bool IsLockedCell(BoardPosition boardPosition)
    {
        if (IsPointValid(boardPosition) == false) return false;

        return boardCells[boardPosition.X, boardPosition.Y, boardPosition.Z].IsLocked;
    }

    public virtual bool IsPieceAtPos(BoardPosition position)
    {
        if (boardEntities.ContainsKey(position) && boardEntities[position] != null)
        {
            return true;
        }
        return false;
    }

    public virtual bool IsEmpty(BoardPosition position)
    {
        return IsPieceAtPos(position) == false;
    }

    public virtual Piece GetPieceAt(BoardPosition position)
    {
        Piece piece;
        if (boardEntities.TryGetValue(position, out piece))
        {
            return piece;
        }

        return null;
    }

    public virtual BoardPosition GetPiecePositionDebug(Piece piece)
    {
        for (int d = 0; d < logicMatrix.GetLength(2); d++)
        {
            for (int y = logicMatrix.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < logicMatrix.GetLength(0); x++)
                {
                    var point = new BoardPosition(x, y, d);

                    Piece currentPiece;
                    if (boardEntities.TryGetValue(point, out currentPiece))
                    {
                        if (currentPiece.Equals(piece))
                        {
                            return point;
                        }
                    }
                }
            }
        }
        
        return BoardPosition.Default();
    }

    public virtual int GetElementTypeAt(BoardPosition position)
    {
        if (IsPointValid(position) == false) return -100;

        return logicMatrix[position.X, position.Y, position.Z];
    }

    public virtual bool AddPieceToBoardSilent(int x, int y, Piece piece)
    {
        if (piece == null) return false;

        BoardPosition position = new BoardPosition(x, y, piece.Layer.Index);

        if (IsPointValid(position) == false) return false;

        if (IsEmpty(position) == false) return false;

        SetPieceToBoard(x, y, piece);
        return true;
    }
    
    public virtual bool AddPieceToBoard(int x, int y, Piece piece)
    {
        if (piece == null) return false;

        var position = new BoardPosition(x, y, piece.Layer.Index);

        if (IsPointValid(position) == false) return false;

        if (IsEmpty(position) == false) return false;

        SetPieceToBoard(x, y, piece);

        var observer = piece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);

        observer?.OnAddToBoard(position, piece);
        
//        PieceFlyer.Fly(piece.PieceType, x, y);
//        PieceFlyer.FlyTo(piece.PieceType, x, y, Currency.Piece.Name);
        
        GameDataService.Current.CodexManager.OnPieceBuilded(piece.PieceType);
        
        return true;
    }

    private void RevertMulticellularMove(Piece piece, List<BoardPosition> mask, BoardPosition from, BoardPosition to,
        int currentElement)
    {
        for (int i = currentElement-1; i > 0; i--)
        {
            var maskPos = mask[i];
            var targetPos = maskPos + to;
            var sourcePos = maskPos + from;

            RemovePieceFromBoardSilent(targetPos);
            AddPieceToBoardSilent(sourcePos.X, sourcePos.Y, piece);
        }
    }

    public virtual bool MovePieceFromTo(BoardPosition from, BoardPosition to)
    {
        var fromPiece = GetPieceAt(from);
        var toPiece = GetPieceAt(to);
        
        if (fromPiece == null)
            return false;
        

        var multicellular =
            fromPiece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
        
        if (multicellular != null)
        {
            if(fromPiece != toPiece && toPiece != null)
                return false;
            
            var targetPositions = new List<BoardPosition>();
            var sourcePositions = new List<BoardPosition>();
            for (int i = 0; i < multicellular.Mask.Count; i++)
            {
                var maskPos = multicellular.Mask[i];
                var targetPos = maskPos + to;
                var sourcePos = maskPos + from;
                
                targetPositions.Add(targetPos);
                sourcePositions.Add(sourcePos);
                RemovePieceFromBoardSilent(sourcePos);
            }
            
            for (int i = 0; i < multicellular.Mask.Count; i++)
            {
                var targetPos = targetPositions[i];
                AddPieceToBoardSilent(targetPos.X, targetPos.Y, fromPiece);
            }
            
            var observer = fromPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
            if (observer != null)
            {
                observer.OnMovedFromTo(from, to);
            }

            return true;
        }

        if (toPiece != null)
            return false;
        
        if (AddPieceToBoardSilent(to.X, to.Y, fromPiece) && RemovePieceFromBoardSilent(from))
        {
            var observer = fromPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
            if (observer != null)
            {
                observer.OnMovedFromTo(from, to);
            }
            return true;
        }

        return false;
    }

    public virtual bool SwapPieces(BoardPosition from, BoardPosition to)
    {
        var fromPiece = GetPieceAt(from);
        var toPiece = GetPieceAt(to);

        if (fromPiece == null || toPiece == null) return false;

        if (fromPiece.Layer.Index != toPiece.Layer.Index) return false;

        RemovePieceFromBoardSilent(from);
        RemovePieceFromBoardSilent(to);

        AddPieceToBoardSilent(to.X, to.Y, fromPiece);
        AddPieceToBoardSilent(from.X, from.Y, toPiece);
        
        var observerFrom = fromPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
        if (observerFrom != null)
        {
            observerFrom.OnMovedFromTo(from, to);
        }
        
        var observerTo = toPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
        if (observerTo != null)
        {
            observerTo.OnMovedFromTo(to, from);
        }

        return true;
    }

    public virtual List<T> GetBehavioursAtLayers<T>(BoardPosition point) where T : class
    {
        List<T> behaviours = new List<T>();
        for (int layer = 0; layer < depth; layer++)
        {
            var beh = GetBehaviourAt<T>(new BoardPosition(point.X, point.Y, layer));
            if (beh != null)
            {
                behaviours.Add(beh);
            }
        }

        return behaviours;
    }

    public virtual T GetBehaviourAt<T>(BoardPosition point) where T : class
    {
        var piece = GetPieceAt(point);

        return piece as T;
    }

    public virtual bool IsHasBehaviourAt<T>(BoardPosition point) where T : class
    {
        var piece = GetPieceAt(point);

        return piece is T;
    }

    public virtual bool RemovePieceFromBoardSilent(BoardPosition pos)
    {
        if (pos.IsValid)
        {
            if (boardEntities.Remove(pos))
            {
                logicMatrix[pos.X, pos.Y, pos.Z] = -1;

                return true;
            }
        }

        return false;
    }

    protected virtual Piece RemoveAndGetPieceFromBoard(BoardPosition pos)
    {
        Piece piece;
        if (pos.IsValid && boardEntities.TryGetValue(pos, out piece))
        {
            if (boardEntities.Remove(pos))
            {
                logicMatrix[pos.X, pos.Y, pos.Z] = -1;
                
                var observer = piece == null ? null : piece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
                if (observer != null)
                {
                    observer.OnRemoveFromBoard(pos, piece);
                }
                return piece;
            }
        }

        return null;
    }

    public virtual void ClearMatrix()
    {
        boardEntities.Clear();
        logicMatrix = new int[width, height, depth];

        boardCells = new BoardCell[width, height, depth];
        for (int d = 0; d < logicMatrix.GetLength(2); d++)
        {
            for (int y = logicMatrix.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < logicMatrix.GetLength(0); x++)
                {
                    boardCells[x, y, d] = new BoardCell();
                }
            }
        }
    }

    public virtual bool IsUnlockedFromTo(BoardPosition from, BoardPosition to)
    {
        while (from.Equals(to) == false)
        {
            if (IsLockedCell(from))
            {
                return false;
            }

            from = from.NextTo(to);
        }

        return IsLockedCell(from) == false;
    }

    protected virtual void SetPieceToBoard(int x, int y, Piece piece)
    {
        BoardPosition position = new BoardPosition(x, y, piece.Layer.Index);

        if (IsPointValid(position) == false) return;

        logicMatrix[position.X, position.Y, position.Z] = piece.PieceType;

        if (boardEntities.ContainsKey(position) == false)
        {
            boardEntities.Add(position, piece);
        }
        else
        {
            boardEntities[position] = piece;
        }
    }

    public virtual void ClearBoard()
    {
        ClearMatrix();
    }

    public static string LogMatrix(int[,,] matrix)
    {
        StringBuilder log = new StringBuilder();

        log.AppendLine("matrix");

        for (int d = 1; d < 2; d++)
        {
            for (int y = matrix.GetLength(1) - 1; y >= 0; y--)
            {
                if (y != matrix.GetLength(1) - 1)
                {
                    log.AppendLine();
                }
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    int pieceType = matrix[x, y, d];
                    log.Append(pieceType.ToString().PadRight(3, '#'));
                    log.Append("; ");
                }
            }
            log.AppendLine();
        }

        return log.ToString();
    }

    public virtual string LogMatrix()
    {
        if (context.Logger.IsLoggingEnabled == false) return "";

        StringBuilder log = new StringBuilder();

        log.AppendLine("matrix");

        for (int d = 1; d < 2; d++)
        {
            for (int y = logicMatrix.GetLength(1) - 1; y >= 0; y--)
            {
                if (y != logicMatrix.GetLength(1) - 1)
                {
                    log.AppendLine();
                }
                for (int x = 0; x < logicMatrix.GetLength(0); x++)
                {
                    int pieceType = logicMatrix[x, y, d];
                    log.Append(pieceType.ToString().PadRight(3, '#'));
                    var boardPosition = new BoardPosition(x, y, d);
                    if (boardEntities.ContainsKey(boardPosition))
                    {
                        var piece = boardEntities[boardPosition];
                        log.Append("/");
                        log.Append(IsLockedCell(boardPosition) ? "1" : "0");

                        log.Append("/");
                        log.Append("#");
                    }
                    else
                    {
                        log.Append("/");
                        log.Append("#");
                        log.Append("/");
                        log.Append(IsLockedCell(boardPosition) ? "1" : "0");
                    }
                    log.Append("; ");
                }
            }
            log.AppendLine();
        }

        return log.ToString();
    }

    public virtual void LogPieceStates()
    {
        if (context.Logger.IsLoggingEnabled == false) return;

        var st = new System.Text.StringBuilder();
        st.AppendLine();
        foreach (var element in boardEntities)
        {
            if (element.Value is Piece)
            {
                var piece = element.Value as Piece;
                // st.AppendLine(string.Format("{0} => {1}", element.Key, piece.StateMachine.ActiveState.GetType().ToString().PadRight(10, ' ')));
            }
        }

        context.Logger.Log(st.ToString());
    }

    public virtual bool RemovePiecesAt(List<BoardPosition> positions)
    {
        var state = true;
        
        for (int i = 0; i < positions.Count; i++)
        {
            if (RemovePieceAt(positions[i]) == false) state = false;
        }
        
        return state;
    }

    /// <summary>
    /// Fast remove but need to check that piece is at position
    /// </summary>
    public virtual bool RemovePieceAt(BoardPosition position)
    {
        var piece = RemoveAndGetPieceFromBoard(position);
        if (piece == null) return false;

        return true;
    }

    public virtual bool IsNearPoint(BoardPosition from, BoardPosition to)
    {
        int r = Mathf.Abs(from.X - to.X) + Mathf.Abs(from.Y - to.Y);
        return r == 1;
    }

    public virtual int[,] GetDefMatrix(int[,,] logicMatrix, int w, int h, int layer)
    {
        int[,] defMatrix = new int[w, h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                var point = new BoardPosition(x, y, layer);
                bool isMatchable = IsMatchableAt(point);

                if (isMatchable)
                {
                    defMatrix[x, y] = 1;
                }
                else
                {
                    defMatrix[x, y] = 0;
                }
            }
        }

        return defMatrix;
    }

    public virtual bool IsMatchableAt(BoardPosition point)
    {
        var piece = GetPieceAt(point);
        var matchablePiece = piece == null ? null : piece.GetComponent<MatchableCorePieceComponent>(MatchableCorePieceComponent.ComponentGuid);

        if (piece == null
            || this.IsLockedCell(point)
            || matchablePiece == null
            || matchablePiece.IsMatchable(point) == false)
        {
            return false;
        }

        return true;
    }

    public virtual List<BoardPosition> GetRandomPoints(int fromType, int toType, int count, int layer, List<int> ignoreComponents)
    {
        int w = this.CurrentWidth;
        int h = this.CurrentHeight;

        var defMatrix = GetDefMatrix(GetMatrix(), w, h, layer);
        var allowedList = new List<BoardPosition>();

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                var point = new BoardPosition(x, y, layer);
                var piece = GetPieceAt(point);

                if (defMatrix[point.X, point.Y] == 1)
                {
                    bool isHasComponent = false;
                    if (ignoreComponents != null)
                    {
                        for (int i = 0; i < ignoreComponents.Count; i++)
                        {
                            int componentGuid = ignoreComponents[i];
                            if (piece.IsHasComponent(componentGuid))
                            {
                                isHasComponent = true;
                            }
                        }
                    }

                    if (piece.PieceType >= fromType && piece.PieceType <= toType && isHasComponent == false)
                    {
                        allowedList.Add(point);
                    }
                }

            }
        }

        // shuffle
        allowedList.Shuffle();
        if (allowedList.Count > count)
        {
            allowedList.RemoveRange(count - 1, allowedList.Count - count);
        }

        return allowedList;

    }

    public virtual void LockAllAtLayer(object locker, int layer)
    {
        int w = this.CurrentWidth;
        int h = this.CurrentHeight;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                var point = new BoardPosition(x, y, layer);
                this.LockCell(point, locker);
            }
        }
    }

    public virtual void UnLockAllAtLayer(object locker, int layer)
    {
        int w = this.CurrentWidth;
        int h = this.CurrentHeight;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                var point = new BoardPosition(x, y, layer);
                this.UnlockCell(point, locker);
            }
        }
    }

    public List<MatchDef> GetMatches(int[,,] logicMatrix, int w, int h, int layer)
    {
        var defMatrix = GetDefMatrix(logicMatrix, w, h, layer);

        IMatchPattern rowMatchPattern = new RowMatchPattern();
        var matchesRow = rowMatchPattern.GetMatches(logicMatrix, defMatrix, w, h, layer);

        IMatchPattern columnMatchPattern = new ColumnMatchPattern();
        var matchesColumn = columnMatchPattern.GetMatches(logicMatrix, defMatrix, w, h, layer);

        var clasters = GetClasters(matchesRow, matchesColumn);

        var matches = new List<MatchDef>();
        matches.AddRange(clasters);

        //square
        IMatchPattern squareMatchPattern = new SquareMatchPattern();
        var matchesSquare = squareMatchPattern.GetMatches(logicMatrix, defMatrix, w, h, layer);

        for (int i = 0; i < clasters.Count; i++)
        {
            var matchDef = clasters[i];

            if (matchDef.MatchPoints.Count <= 3)
            {
                for (int j = 0; j < matchesSquare.Count; j++)
                {
                    var squareDef = matchesSquare[j];
                    for (int p = 0; p < matchDef.MatchPoints.Count; p++)
                    {
                        var point = matchDef.MatchPoints[p];

                        if (squareDef.MatchPoints.Contains(point))
                        {
                            matches.Remove(matchDef);
                        }
                    }
                }
            }
        }

        // filter by priority
        matches.AddRange(matchesSquare);

        return matches;
    }

    public virtual List<MatchDef> GetSquareMatches()
    {
        var matches = new List<MatchDef>();

        return matches;
    }

    public List<MatchDef> GetClasters(List<MatchDef> rowMatches, List<MatchDef> columnMatches)
    {
        List<MatchDef> clasters = new List<MatchDef>();

        List<MatchDef> processedDefs = new List<MatchDef>();

        var crossedMatchPattern = new CrossedMatchPattern();

        foreach (var rowDef in rowMatches)
        {
            foreach (var colDef in columnMatches)
            {
                foreach (var rowPoint in rowDef.MatchPoints)
                {
                    bool isHasIntersection = colDef.MatchPoints.Contains(rowPoint);

                    if (isHasIntersection
                        && processedDefs.Contains(rowDef) == false
                        && processedDefs.Contains(colDef) == false)
                    {
                        processedDefs.Add(rowDef);
                        processedDefs.Add(colDef);

                        var claster = new MatchClaster()
                        .AddMatchDef(rowDef)
                        .AddMatchDef(colDef)
                        .AddIntersection(rowPoint);

                        var matchDef = new MatchDef(crossedMatchPattern)
                        .AddPoints(rowDef.MatchPoints)
                        .AddPoints(colDef.MatchPoints)
                        .SetClaster(claster);

                        clasters.Add(matchDef);
                    }
                }
            }
        }

        foreach (var def in rowMatches)
        {
            if (processedDefs.Contains(def) == false)
            {
                clasters.Add(def);
            }
        }

        foreach (var def in columnMatches)
        {
            if (processedDefs.Contains(def) == false)
            {
                clasters.Add(def);
            }
        }

        return clasters;
    }
}