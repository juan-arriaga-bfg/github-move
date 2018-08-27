using System.Collections.Generic;
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
        return $"Uid: {Uid} - Weight: {Weight} - Override: {Override}";
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
        if (oldWeights == null) return nextWeights;
        
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
    IQuestsDataManager, IObstaclesDataManager, ICollectionDataManager, ILevelsDataManager, ITasksDataManager, IProductionDataManager,
    IShopDataManager, IFieldDataManager, ICodexDataManager
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private ChestsDataManager chestsManager;
    public ChestsDataManager ChestsManager => chestsManager ?? (chestsManager = GetComponent<ChestsDataManager>(ChestsDataManager.ComponentGuid));

    private EnemiesDataManager enemiesManager;
    public EnemiesDataManager EnemiesManager => enemiesManager ?? (enemiesManager = GetComponent<EnemiesDataManager>(EnemiesDataManager.ComponentGuid));

    private HeroesDataManager heroesManager;
    public HeroesDataManager HeroesManager => heroesManager ?? (heroesManager = GetComponent<HeroesDataManager>(HeroesDataManager.ComponentGuid));

    private PiecesDataManager piecesManager;
    public PiecesDataManager PiecesManager => piecesManager ?? (piecesManager = GetComponent<PiecesDataManager>(PiecesDataManager.ComponentGuid));

    private CollectionDataManager collectionManager;
    public CollectionDataManager CollectionManager => collectionManager ?? (collectionManager = GetComponent<CollectionDataManager>(CollectionDataManager.ComponentGuid));

    private LevelsDataManager levelsManager;
    public LevelsDataManager LevelsManager => levelsManager ?? (levelsManager = GetComponent<LevelsDataManager>(LevelsDataManager.ComponentGuid));

    private TasksDataManager tasksManager;
    public TasksDataManager TasksManager => tasksManager ?? (tasksManager = GetComponent<TasksDataManager>(TasksDataManager.ComponentGuid));

    private ProductionDataManager productionManager;
    public ProductionDataManager ProductionManager => productionManager ?? (productionManager = GetComponent<ProductionDataManager>(ProductionDataManager.ComponentGuid));

    private FogsDataManager fogsManager;
    public FogsDataManager FogsManager => fogsManager ?? (fogsManager = GetComponent<FogsDataManager>(FogsDataManager.ComponentGuid));

    private QuestsDataManager questsManager;
    public QuestsDataManager QuestsManager => questsManager ?? (questsManager = GetComponent<QuestsDataManager>(QuestsDataManager.ComponentGuid));

    private ObstaclesDataManager obstaclesManager;
    public ObstaclesDataManager ObstaclesManager => obstaclesManager ?? (obstaclesManager = GetComponent<ObstaclesDataManager>(ObstaclesDataManager.ComponentGuid));

    private MinesDataManager minesManager;
    public MinesDataManager MinesManager => minesManager ?? (minesManager = GetComponent<MinesDataManager>(MinesDataManager.ComponentGuid));

    private ShopDataManager shopManager;
    public ShopDataManager ShopManager => shopManager ?? (shopManager = GetComponent<ShopDataManager>(ShopDataManager.ComponentGuid));

    private FieldDataManager fieldManager;
    public FieldDataManager FieldManager => fieldManager ?? (fieldManager = GetComponent<FieldDataManager>(FieldDataManager.ComponentGuid));

    private CodexDataManager codexManager;
    public CodexDataManager CodexManager => codexManager ?? (codexManager = GetComponent<CodexDataManager>(CodexDataManager.ComponentGuid));

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
        RegisterComponent(new ShopDataManager());
        RegisterComponent(new FieldDataManager());
//        RegisterComponent(new ProductionDataManager());
        RegisterComponent(new CodexDataManager());
    }

    public void Reload()
    {
        foreach (var component in componentsCache.Values)
        {
            var manager = component as IDataManager;

            manager?.Reload();
        }
    }
}