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


public class GameDataManager
{
    public readonly ChestsDataManager ChestsManager = new ChestsDataManager();
    public readonly EnemiesDataManager EnemiesManager = new EnemiesDataManager();
    public readonly HeroesDataManager HeroesManager = new HeroesDataManager();
    public readonly PiecesDataManager PiecesManager = new PiecesDataManager();
    public readonly ObstaclesDataManager ObstaclesManager = new ObstaclesDataManager();
    public readonly SimpleObstaclesDataManager SimpleObstaclesManager = new SimpleObstaclesDataManager();
    public readonly QuestsDataManager QuestsManager = new QuestsDataManager();
    public readonly FogsDataManager FogsManager = new FogsDataManager();
    public readonly CollectionDataManager CollectionManager = new CollectionDataManager();
    public readonly LevelsDataManager LevelsManager = new LevelsDataManager();
    public readonly TasksDataManager TasksManager = new TasksDataManager();
    public readonly ProductionDataManager ProductionManager = new ProductionDataManager();

    public void Load()
    {
        ChestsManager.LoadData(new ResourceConfigDataMapper<List<ChestDef>>("configs/chests.data", NSConfigsSettings.Instance.IsUseEncryption));
        EnemiesManager.LoadData(new ResourceConfigDataMapper<List<EnemyDef>>("configs/enemies.data", NSConfigsSettings.Instance.IsUseEncryption));
        HeroesManager.LoadData(new ResourceConfigDataMapper<List<HeroDef>>("configs/heroes.data", NSConfigsSettings.Instance.IsUseEncryption));
        PiecesManager.LoadData(new ResourceConfigDataMapper<List<PieceDef>>("configs/pieces.data", NSConfigsSettings.Instance.IsUseEncryption));
        ObstaclesManager.LoadData(new ResourceConfigDataMapper<List<ObstacleDef>>("configs/obstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
        SimpleObstaclesManager.LoadData(new ResourceConfigDataMapper<List<SimpleObstaclesDef>>("configs/simpleObstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
        QuestsManager.LoadData(new ResourceConfigDataMapper<List<QuestDef>>("configs/quests.data", NSConfigsSettings.Instance.IsUseEncryption));
        FogsManager.LoadData(new ResourceConfigDataMapper<FogsDataManager>("configs/fogs.data", NSConfigsSettings.Instance.IsUseEncryption));
        CollectionManager.LoadData(new ResourceConfigDataMapper<CollectionDataManager>("configs/collection.data", NSConfigsSettings.Instance.IsUseEncryption));
        LevelsManager.LoadData(new ResourceConfigDataMapper<List<LevelsDef>>("configs/levels.data", NSConfigsSettings.Instance.IsUseEncryption));
        TasksManager.LoadData(new ResourceConfigDataMapper<TasksDataManager>("configs/tasks.data", NSConfigsSettings.Instance.IsUseEncryption));
        ProductionManager.LoadData(new ResourceConfigDataMapper<List<ProductionDef>>("configs/production.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
}