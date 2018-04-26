using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstaclesLogicComponent : IECSComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
    public int Guid
    {
        get { return ComponentGuid; }
    }
    
    protected BoardController context;
    
    private Dictionary<int, Obstacle> obstaclesUidKey = new Dictionary<int, Obstacle>();
    private Dictionary<BoardPosition, Obstacle> obstaclesPositionKey = new Dictionary<BoardPosition, Obstacle>();
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Init()
    {
        CreateFog();
        
        var data = GameDataService.Current.ObstaclesManager.Obstacles;
        var positions = new List<BoardPosition>();

        foreach (var def in data)
        {
            var obstacle = new Obstacle(def);

            def.Position = new BoardPosition(def.Position.X, def.Position.Y, context.BoardDef.PieceLayer);
            
            obstaclesUidKey.Add(def.Uid, obstacle);
            obstaclesPositionKey.Add(def.Position, obstacle);
            positions.Add(def.Position);
        }
        
        /*context.ActionExecutor.PerformAction(new FillBoardAction
        {
            Piece = PieceType.Quest.Id,
            Positions = positions
        });*/
    }

    private void CreateFog()
    {
        var data = GameDataService.Current.FogsManager.Fogs;
        var positions = new List<BoardPosition>();

        foreach (var fog in data)
        {
            var pos = fog.Position;
            
            pos.Z = context.BoardDef.PieceLayer;
            positions.Add(pos);
        }
        
        context.ActionExecutor.PerformAction(new FillBoardAction
        {
            Piece = PieceType.Fog.Id,
            Positions = positions
        });
    }
    
    public void Execute()
    {
        foreach (var obstacle in obstaclesUidKey.Values)
        {
            obstacle.Check(this);
        }
    }
    
    public bool Check(BoardPosition position)
    {
        var obstacle = GetObstacle(position);

        return obstacle != null && obstacle.Check(this);
    }
    
    public Obstacle GetObstacle(int index)
    {
        Obstacle obstacle;
        
        if (obstaclesUidKey.TryGetValue(index, out obstacle) == false)
        {
            return null;
        }

        return obstacle;
    }

    public Obstacle GetObstacle(BoardPosition position)
    {
        Obstacle obstacle;
        
        if (obstaclesPositionKey.TryGetValue(position, out obstacle) == false)
        {
            return null;
        }

        return obstacle;
    }

    public bool RemoveObstacle(BoardPosition position)
    {
        Obstacle obstacle;

        if (obstaclesPositionKey.TryGetValue(position, out obstacle) == false)
        {
            return false;
        }

        obstaclesPositionKey.Remove(position);
        obstaclesUidKey.Remove(obstacle.GetUid());
        return true;
    }

    public BoardPosition GetPositionCurrentObstacle()
    {
        var min = int.MaxValue;

        foreach (var pair in obstaclesUidKey)
        {
            min = Mathf.Min(min, pair.Key);
        }

        var current = obstaclesUidKey[min];
        var obstacle = obstaclesPositionKey.First(p => p.Value == current);

        return obstacle.Value == null ? BoardPosition.Default() : obstacle.Key;
    }
    
    public bool IsPersistence
    {
        get { return false; }
    }

    public bool IsExecuteable()
    {
        return true;
    }
}