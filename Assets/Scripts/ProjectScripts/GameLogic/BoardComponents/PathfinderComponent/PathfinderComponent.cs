using System;
using System.Collections.Generic;
using System.Linq;

public class PathfinderComponent:ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private BoardController board;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        board = entity as BoardController;
    }

    private bool Check(Piece context, BoardPosition position)
    {
        var boardLogic = board.BoardLogic;
        
        var tileDefId = GameDataService.Current.FieldManager.GetTileId(position.X, position.Y);
        var isRelief = tileDefId != BoardTiles.WATER_TILE_ID && BoardTiles.GetDefs()[tileDefId].IsLock;
        if (isRelief)
        {
            return true;
        }
        
        if (!position.IsValidFor(board.BoardDef.Width, board.BoardDef.Height) || boardLogic.IsLockedCell(position, new List<Type>
        {
            typeof(DragAndCheckMatchAction),
            typeof(ModificationPiecesAction),
            typeof(ManaCangeAction)
        })) return false;
        
        var pieceInCurrentPos = boardLogic.GetPieceAt(position);

        if (pieceInCurrentPos == null || pieceInCurrentPos == context)
        {
            return true;
        }

        var ignorablePieceTypes = PathfindIgnores.GetIgnoreList(context.PieceType);
        if (context.PieceType == PieceType.Fog.Id)
        {
            return !boardLogic.IsLockedCell(position) && ignorablePieceTypes.Contains(pieceInCurrentPos.PieceType);
        }

        return ignorablePieceTypes.Contains(pieceInCurrentPos.PieceType);
    }
    
    public Predicate<BoardPosition> GetCondition(Piece piece)
    {
        return (pos) => Check(piece, pos);
    }
    
    //A* pathfinding algorithm
    public virtual bool HasPath(BoardPosition from, HashSet<BoardPosition> to, out List<BoardPosition> blockagePositions,
        Piece piece = null, Predicate<BoardPosition> condition = null)
    {
        blockagePositions = new List<BoardPosition>();
        var blockagePositionsHash = new HashSet<BoardPosition>();
        
        if (to.Contains(from))
            return true;
        
        to = new HashSet<BoardPosition>(to.Where(elem => from.Z == elem.Z));
        if (to.Count == 0)
            return false;
        
        //Init locals
        var checkedPositions = new HashSet<BoardPosition>();
        var uncheckedPositions = new HashSet<BoardPosition>();

        var costMap = new Dictionary<BoardPosition, int>();
        var predictionCosts = new Dictionary<BoardPosition, int>();

        Predicate<BoardPosition> fieldCondition = condition ?? GetCondition(piece);
        
        //Init start node data
        uncheckedPositions.Add(from);

        costMap[from] = 0;
        predictionCosts[from] = Heuristic(from, to.First());
        
        //Begin pathfinding
        while (uncheckedPositions.Count > 0)
        {
            var current = FindPosWithMinimalCost(predictionCosts, uncheckedPositions);
            if (to.Contains(current))
            {
                blockagePositions = new List<BoardPosition>(blockagePositionsHash);
                return true;                
            }
            
            uncheckedPositions.Remove(current);
            checkedPositions.Add(current);

            var availiablePositions = AvailiablePositions(current, checkedPositions, fieldCondition, ref blockagePositionsHash);
            
            //Init neighbour positions data
            for (int i = 0; i < availiablePositions.Count; i++)
            {
                var currentNeghbour = availiablePositions[i];
                
                int distance = 1;
                var tempCost = costMap[current] + distance;

                if (!uncheckedPositions.Contains(currentNeghbour) || tempCost < costMap[currentNeghbour])
                {
                    costMap[currentNeghbour] = tempCost;
                    predictionCosts[currentNeghbour] = tempCost + Heuristic(currentNeghbour, to.First());
                }
                
                if(!uncheckedPositions.Contains(currentNeghbour))
                    uncheckedPositions.Add(currentNeghbour);
            }
        }
        
        blockagePositions = new List<BoardPosition>(blockagePositionsHash);
        return false;
    }
    
    public virtual bool HasPath(BoardPosition from, BoardPosition to, out List<BoardPosition> blockagePositions, Piece piece = null, Predicate<BoardPosition> condition = null)
    {
        return HasPath(from, new HashSet<BoardPosition> {to}, out blockagePositions, piece, condition);
    }
    
    protected BoardPosition FindPosWithMinimalCost(Dictionary<BoardPosition, int> costs,
        HashSet<BoardPosition> uncheckedPositions)
    {
        var currentPos = uncheckedPositions.First();
        var minimalCost = costs[currentPos];
        
        foreach (var position in uncheckedPositions)
        {
            if (costs[position] < minimalCost)
            {
                minimalCost = costs[position];
                currentPos = position;
            }  
        }

        return currentPos;
    }

    protected virtual int Heuristic(BoardPosition from, BoardPosition to)
    {
        return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
    }
    
    protected List<BoardPosition> AvailiablePositions(BoardPosition position, HashSet<BoardPosition> checkedPositions, Predicate<BoardPosition> predicate, ref HashSet<BoardPosition> unavailiable)
    {
        var uncheckedNeigbours = position.Neighbors();
        
        var baseTileDefId = GameDataService.Current.FieldManager.GetTileId(position.X, position.Y);
        var baseIsRelief = baseTileDefId != BoardTiles.WATER_TILE_ID && BoardTiles.GetDefs()[baseTileDefId].IsLock;

        if (baseIsRelief)
        {
            return new List<BoardPosition>();
        }
        
        var checkedNeigbours = new List<BoardPosition>();
        var count = uncheckedNeigbours.Count;
        for (var i = 0; i < count; i++)
        {
            var currentNeighbour = uncheckedNeigbours[i];
            
            var currentTileDefId = GameDataService.Current.FieldManager.GetTileId(currentNeighbour.X, currentNeighbour.Y);
            var currentIsRelief = currentTileDefId != BoardTiles.WATER_TILE_ID && BoardTiles.GetDefs()[currentTileDefId].IsLock;

            if (baseIsRelief == false && currentIsRelief)
            {
                if (currentNeighbour.X > position.X)
                {
                    currentNeighbour.X++;
                } 
                else if (currentNeighbour.X < position.X)
                {
                    currentNeighbour.X--;
                }
                else if (currentNeighbour.Y > position.Y)
                {
                    currentNeighbour.Y++;
                }
                else if (currentNeighbour.Y < position.Y)
                {
                    currentNeighbour.Y--;
                }
            }
            
            var targetPiece = board.BoardLogic.GetPieceAt(currentNeighbour);
            var isValid = predicate.Invoke(currentNeighbour);
            if(!checkedPositions.Contains(currentNeighbour) && isValid)
                checkedNeigbours.Add(currentNeighbour); 
            if(targetPiece != null && isValid == false)
                unavailiable.Add(targetPiece.CachedPosition);
        }
        
        return checkedNeigbours;
    }
}

