using System.Collections.Generic;
using UnityEngine;

public class ObstaclesDataManager : SequenceData, IDataLoader<List<ObstacleDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public Dictionary<int, ObstacleDef> Obstacles;

    private ObstacleDef defaultDef;
    private Dictionary<int, List<ObstacleDef>> branches;
    
    private MatchDefinitionComponent matchDefinition;
    
    public override void Reload()
    {
        base.Reload();
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

                foreach (var def in data)
                {
                    Obstacles.Add(def.Piece, def);
                    AddToBranch(matchDefinition.GetFirst(def.Piece), def);
                    AddSequence(def.Uid, def.PieceWeights);
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

        return Obstacles.TryGetValue(piece, out def) ? def.Chest : PieceType.None.Id;
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
        var def = GetStep(piece, step);
        
        var hard = GameDataService.Current.LevelsManager.GetSequence(Currency.Level.Name);
        var sequence = GetSequence(def.Uid);
        
        var reward = hard.GetNextDict(def.ProductionAmount.Range());
        reward = sequence.GetNextDict(def.PieceAmount, reward);
        
        return reward;
    }

    public CurrencyPair GetPriceByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        return def.Price;
    }
}