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

    public Predicate<BoardPosition> GetCondition(Piece piece)
    {
        var boardCondition = piece?.BoardCondition;
        if (boardCondition == null)
            return (_) => true;
        return boardCondition.Check;
    }
    
    
    //A* pathfinding algorithm
    public virtual bool HasPath(BoardPosition from, HashSet<BoardPosition> to, out List<BoardPosition> blockagePositions,
        Piece piece = null, Predicate<BoardPosition> condition = null)
    {
        blockagePositions = new List<BoardPosition>();
        
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
                return true;
            
            uncheckedPositions.Remove(current);
            checkedPositions.Add(current);

            var availiablePositions = AvailiablePositions(current, checkedPositions, fieldCondition, ref blockagePositions);
            
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
    
    protected List<BoardPosition> AvailiablePositions(BoardPosition position, HashSet<BoardPosition> checkedPositions, Predicate<BoardPosition> predicate, ref List<BoardPosition> unavailiable)
    {
        var uncheckedNeigbours = position.Neighbors();
        
        var checkedNeigbours = new List<BoardPosition>();

        for (var i = 0; i < uncheckedNeigbours.Count; i++)
        {
            var currentNeighbour = uncheckedNeigbours[i];
            var targetPiece = board.BoardLogic.GetPieceAt(currentNeighbour);
            if(!checkedPositions.Contains(currentNeighbour) && predicate.Invoke(currentNeighbour))
                checkedNeigbours.Add(currentNeighbour); 
            else if(targetPiece != null && !unavailiable.Contains(targetPiece.CachedPosition))
                unavailiable.Add(targetPiece.CachedPosition);
        }
        
        return checkedNeigbours;
    }
}

