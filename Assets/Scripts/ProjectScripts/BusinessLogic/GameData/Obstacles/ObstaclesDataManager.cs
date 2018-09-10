using System.Collections.Generic;
using UnityEngine;

public class ObstaclesDataManager : IECSComponent, IDataManager, IDataLoader<List<ObstacleDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public Dictionary<int, ObstacleDef> Obstacles;

    private ObstacleDef defaultDef;
    private Dictionary<int, List<ObstacleDef>> branches;
    
    private MatchDefinitionComponent matchDefinition;
    
    public void Reload()
    {
        Obstacles = null;
        LoadData(new ResourceConfigDataMapper<List<ObstacleDef>>("configs/obstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadData(IDataMapper<List<ObstacleDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            Obstacles = new Dictionary<int, ObstacleDef>();
            branches = new Dictionary<int, List<ObstacleDef>>();
            
            matchDefinition = new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());
            
            if (string.IsNullOrEmpty(error))
            {
                data.Sort((a, b) => a.Piece.CompareTo(b.Piece));
                defaultDef = data[0];

                foreach (var next in data)
                {
                    var previousType = matchDefinition.GetPrevious(next.Piece);
                    
                    if (previousType != PieceType.None.Id)
                    {
                        var previous = data.Find(def => def.Piece == previousType);
                        
                        next.PieceWeights = ItemWeight.ReplaseWeights(previous.PieceWeights, next.PieceWeights);
                        next.ChestWeights = ItemWeight.ReplaseWeights(previous.ChestWeights, next.ChestWeights);
                    }
                    
                    Obstacles.Add(next.Piece, next);
                    AddToBranch(matchDefinition.GetFirst(next.Piece), next);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    private void AddToBranch(int key, ObstacleDef def)
    {
        List<ObstacleDef> list;

        if (branches.TryGetValue(key, out list) == false)
        {
            list = new List<ObstacleDef>();
            branches.Add(key, list);
        }
        
        list.Add(def);
        list.Sort((a, b) => a.Piece.CompareTo(b.Piece));
    }

    private ObstacleDef GetStep(int piece, int step)
    {
        var key = matchDefinition.GetFirst(piece);
        
        List<ObstacleDef> list;

        if (branches.TryGetValue(key, out list) == false)
        {
            return defaultDef;
        }

        step = Mathf.Clamp(step, 0, list.Count - 1);

        return list[step];
    }
    
    public int GetReward(int piece)
    {
        ObstacleDef def;

        if (Obstacles.TryGetValue(piece, out def) == false) return PieceType.None.Id;
        
        var item = ItemWeight.GetRandomItem(def.ChestWeights);
        
        return item?.Piece ?? PieceType.None.Id;
    }
    
    public int GetDelayByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        return def.Delay;
    }

    public List<CurrencyPair> GetRewardByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        return def.StepRewards;
    }

    public Dictionary<int, int> GetPiecesByStep(int piece, int step)
    {
        var result = new Dictionary<int, int>();
        var def = GetStep(piece, step);
        
        for (var i = def.PieceAmount - 1; i >= 0; i--)
        {
            var item = ItemWeight.GetRandomItem(def.PieceWeights);

            if (item == null) continue;

            if (result.ContainsKey(item.Piece) == false)
            {
                result.Add(item.Piece, 1);
                continue;
            }
            
            result[item.Piece]++;
        }
        
        return result;
    }

    public CurrencyPair GetPriceByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        return def.Price;
    }

    public CurrencyPair GetFastPriceByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        return def.FastPrice;
    }
}