﻿using System;
using System.Collections.Generic;
using System.Linq;

public class PathfinderComponent:ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    private BoardConditionComponent boardCondition;
    public BoardConditionComponent BoardCondition
    {
        get
        {
            return boardCondition ?? 
                   (boardCondition = GetComponent<BoardConditionComponent>(BoardConditionComponent.ComponentGuid));
        }
    }
    
    public override int Guid
    {
        get { return ComponentGuid; }
    }

    //A* pathfinding algorithm
    public virtual bool HasPath(BoardPosition from, IList<BoardPosition> to)
    {
        //Init locals
        var checkedPositions = new List<BoardPosition>();
        var uncheckedPositions = new List<BoardPosition>();
        
        var costMap = new Dictionary<BoardPosition, int>();
        var predictionCosts = new Dictionary<BoardPosition, int>();
        
        //Init start node data
        uncheckedPositions.Add(from);

        costMap[from] = 0;
        predictionCosts[from] = Heuristic(from, to[0]);

        //Begin pathfinding
        while (uncheckedPositions.Count > 0)
        {
            var current = FindPosWithMinimalCost(predictionCosts, uncheckedPositions);
            if (to.Contains(current))
                return true;
            
            uncheckedPositions.Remove(current);
            checkedPositions.Add(current);

            var availiablePositions = AvailiablePositions(current, checkedPositions);
            
            //Init neighbour positions data
            for (int i = 0; i < availiablePositions.Count; i++)
            {
                var currentNeghbour = availiablePositions[i];
                
                int distance = 1;
                var tempCost = costMap[current] + distance;

                if (!uncheckedPositions.Contains(currentNeghbour) || tempCost < costMap[currentNeghbour])
                {
                    costMap[currentNeghbour] = tempCost;
                    predictionCosts[currentNeghbour] = tempCost + Heuristic(currentNeghbour, to[0]);
                }
                
                if(!uncheckedPositions.Contains(currentNeghbour))
                    uncheckedPositions.Add(currentNeghbour);
            }
        }
        
        return false;
    }
    
    public virtual bool HasPath(BoardPosition from, BoardPosition to)
    {
        return HasPath(from, new List<BoardPosition> {to});
    }

    private BoardPosition FindPosWithMinimalCost(Dictionary<BoardPosition, int> costs,
        IList<BoardPosition> uncheckedPositions)
    {
        var currentPos = uncheckedPositions[0];
        var minimalCost = costs[currentPos];
        
        for (var i = 0; i < uncheckedPositions.Count; i++)
        {
            var position = uncheckedPositions[i];
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
    
    private List<BoardPosition> AvailiablePositions(BoardPosition position, IList<BoardPosition> checkedPositions)
    {
        var uncheckedNeigbours = new List<BoardPosition>
        {
            new BoardPosition(position.X + 1, position.Y, position.Z),
            new BoardPosition(position.X - 1, position.Y, position.Z),
            new BoardPosition(position.X, position.Y + 1, position.Z),
            new BoardPosition(position.X, position.Y - 1, position.Z) 
        };
        
        var checkedNeigbours = new List<BoardPosition>();

        for (var i = 0; i < uncheckedNeigbours.Count; i++)
        {
            var currentNeighbour = uncheckedNeigbours[i];
            if(!checkedPositions.Contains(currentNeighbour) && BoardCondition.Check(currentNeighbour))
                checkedNeigbours.Add(currentNeighbour); 
        }
        
        return checkedNeigbours;
    }
}