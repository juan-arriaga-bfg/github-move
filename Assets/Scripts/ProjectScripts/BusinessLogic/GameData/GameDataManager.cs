﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemWeight
{
    private int pieceType = -1;
    
    public string Uid { get; set; }
    public int Weight { get; set; }
    public bool Override { get; set; }
    
    public int Piece
    {
        get
        {
            if (pieceType == -1)
            {
                pieceType = PieceType.Parse(Uid);
            }
            
            return pieceType;
        }
    }

    public ItemWeight Copy()
    {
        return new ItemWeight{Uid = this.Uid, Weight = this.Weight};
    }

    public override string ToString()
    {
        return string.Format("Uid: {0} - Weight: {1} - Override: {2}", Uid, Weight, Override);
    }

    public static ItemWeight GetRandomItem(List<ItemWeight> weights, bool isExclude = false)
    {
        var sum = weights.Sum(w => w.Weight);
        var current = 0;
        var random = Random.Range(0, sum + 1);
        
        weights.Sort((a, b) => a.Weight.CompareTo(b.Weight));
        
        foreach (var item in weights)
        {
            current += item.Weight;
            
            if (current < random) continue;

            if (isExclude) weights.Remove(item);
            
            return item;
        }
        
        return null;
    }

    public static List<ItemWeight> ReplaseWeights(List<ItemWeight> oldWeights, List<ItemWeight> nextWeights)
    {
        var weights = new List<ItemWeight>();

        foreach (var weight in oldWeights)
        {
            var next = nextWeights.Find(w => w.Uid == weight.Uid);

            if (next == null)
            {
                weights.Add(weight.Copy());
                continue;
            }
            
            if (next.Override)
            {
                weights.Add(next.Copy());
                continue;
            }
            
            var newWeight = weight.Copy();

            newWeight.Weight += next.Weight;

            weights.Add(newWeight);
        }
        
        foreach (var weight in nextWeights)
        {
            var old = oldWeights.Find(w => w.Uid == weight.Uid);
            
            if(old != null) continue;
            
            weights.Add(weight.Copy());
        }
        
        return weights;
    }
}


public class GameDataManager : ECSEntity,
    IChestsDataManager, IEnemiesDataManager, IHeroesDataManager, IPiecesDataManager, IFogsDataManager, IMinesDataManager,
    IQuestsDataManager, IObstaclesDataManager, ICollectionDataManager, ILevelsDataManager, ITasksDataManager, IProductionDataManager
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid
    {
        get { return ComponentGuid; }
    }
    
    private ChestsDataManager chestsManager;
    public ChestsDataManager ChestsManager
    {
        get
        {
            return chestsManager ?? (chestsManager = GetComponent<ChestsDataManager>(ChestsDataManager.ComponentGuid));
        }
    }

    private EnemiesDataManager enemiesManager;
    public EnemiesDataManager EnemiesManager
    {
        get
        {
            return enemiesManager ??
                   (enemiesManager = GetComponent<EnemiesDataManager>(EnemiesDataManager.ComponentGuid));
        }
    }

    private HeroesDataManager heroesManager;
    public HeroesDataManager HeroesManager
    {
        get
        {
            return heroesManager ?? (heroesManager = GetComponent<HeroesDataManager>(HeroesDataManager.ComponentGuid));
        }
    }

    private PiecesDataManager piecesManager;
    public PiecesDataManager PiecesManager
    {
        get
        {
            return piecesManager ?? (piecesManager = GetComponent<PiecesDataManager>(PiecesDataManager.ComponentGuid));
        }
    }

    private CollectionDataManager collectionManager;
    public CollectionDataManager CollectionManager
    {
        get
        {
            return collectionManager ?? (collectionManager = GetComponent<CollectionDataManager>(CollectionDataManager.ComponentGuid));
        }
    }

    private LevelsDataManager levelsManager;
    public LevelsDataManager LevelsManager
    {
        get
        {
            return levelsManager ?? (levelsManager = GetComponent<LevelsDataManager>(LevelsDataManager.ComponentGuid));
        }
    }

    private TasksDataManager tasksManager;
    public TasksDataManager TasksManager
    {
        get { return tasksManager ?? (tasksManager = GetComponent<TasksDataManager>(TasksDataManager.ComponentGuid)); }
    }

    private ProductionDataManager productionManager;
    public ProductionDataManager ProductionManager
    {
        get
        {
            return productionManager ?? (productionManager = GetComponent<ProductionDataManager>(ProductionDataManager.ComponentGuid));
        }
    }
    
    private FogsDataManager fogsManager;
    public FogsDataManager FogsManager
    {
        get
        {
            return fogsManager ?? (fogsManager = GetComponent<FogsDataManager>(FogsDataManager.ComponentGuid));
        }
    }
    
    private QuestsDataManager questsManager;
    public QuestsDataManager QuestsManager
    {
        get
        {
            return questsManager ?? (questsManager = GetComponent<QuestsDataManager>(QuestsDataManager.ComponentGuid));
        }
    }
    
    private ObstaclesDataManager obstaclesManager;
    public ObstaclesDataManager ObstaclesManager
    {
        get
        {
            return obstaclesManager ?? (obstaclesManager = GetComponent<ObstaclesDataManager>(ObstaclesDataManager.ComponentGuid));
        }
    }
    
    private MinesDataManager minesManager;
    public MinesDataManager MinesManager
    {
        get
        {
            return minesManager ?? (minesManager = GetComponent<MinesDataManager>(MinesDataManager.ComponentGuid));
        }
    }
    
    public void SetupComponents()
    {
        RegisterComponent(new ChestsDataManager());
//        RegisterComponent(new EnemiesDataManager());
//        RegisterComponent(new HeroesDataManager());
        RegisterComponent(new PiecesDataManager());
        RegisterComponent(new ObstaclesDataManager());
        RegisterComponent(new QuestsDataManager());
        RegisterComponent(new FogsDataManager());
        RegisterComponent(new CollectionDataManager());
        RegisterComponent(new LevelsDataManager());
        RegisterComponent(new TasksDataManager());
        RegisterComponent(new MinesDataManager());
//        RegisterComponent(new ProductionDataManager());
    }

    public void Reload()
    {
        foreach (var component in componentsCache.Values)
        {
            var manager = component as IDataManager;

            if (manager == null) continue;

            manager.Reload();
        }
    }
}